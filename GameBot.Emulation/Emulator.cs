/*
 * Game Boy Emulator
 * Copyright (C) 2008 Michael Birken
 * 
 * This file is part of Game Boy Emulator.
 *
 * Game Boy Emulator is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published 
 * by the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * Game Boy Emulator is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using GameBot.Core;
using GameBot.Core.Data;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GameBot.Emulation
{
    public class Emulator : IActuator
    {
        private const int FramesPerSecond = 60;
        private const int MaxFramesSkipped = 10;

        private const int DisplayWidth = 160;
        private const int DisplayHeight = 144;

        private const int FramesAfterButton = 2;

        private readonly X80 cpu;
        private double scanLineTicks;
        private uint[] pixels = new uint[DisplayWidth * DisplayHeight];

        private Graphics graphics;
        private Bitmap bitmap;

        public bool Running { get; private set; }
        public int Frames { get; private set; }
        public TimeSpan Time { get { return TimeSpan.FromSeconds((double)Frames / FramesPerSecond); } }
        public Bitmap Display { get { return bitmap; } }
        public Game Game { get; private set; }
        private Size DisplaySize { get { return new Size(DisplayWidth, DisplayHeight); } }
        
        private bool anyButtonsPressed = false;

        public Emulator()
        {
            cpu = new X80();

            graphics = Graphics.FromImage(new Bitmap(DisplayWidth, DisplayHeight));
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            // init image
            for (int i = 0; i < pixels.Length; i++) { pixels[i] = 0xFF000000; }
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(pixels, 0);
            bitmap = new Bitmap(DisplayWidth, DisplayHeight, DisplayWidth * 4, PixelFormat.Format32bppPArgb, pointer);
        }

        public void Load(Game game)
        {
            Game = game;
            cpu.cartridge = Game.cartridge;
            cpu.PowerUp();

            Running = true;

            // strange effects appear in the first few frames , skip them
            Execute(4);
        }

        private void UpdateModel(bool updateBitmap)
        {
            Frames++;
            if (updateBitmap)
            {
                uint[] backgroundPalette = cpu.backgroundPalette;
                uint[] objectPalette0 = cpu.objectPalette0;
                uint[] objectPalette1 = cpu.objectPalette1;
                uint[,] backgroundBuffer = cpu.backgroundBuffer;
                uint[,] windowBuffer = cpu.windowBuffer;
                byte[] oam = cpu.oam;

                for (int y = 0, pixelIndex = 0; y < 144; y++)
                {
                    cpu.ly = y;
                    cpu.lcdcMode = LcdcModeType.SearchingOamRam;
                    if (cpu.lcdcInterruptEnabled && (cpu.lcdcOamInterruptEnabled || (cpu.lcdcLycLyCoincidenceInterruptEnabled && cpu.lyCompare == y)))
                    {
                        cpu.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    cpu.lcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);

                    cpu.UpdateWindow();
                    cpu.UpdateBackground();
                    cpu.UpdateSpriteTiles();

                    bool backgroundDisplayed = cpu.backgroundDisplayed;
                    bool backgroundAndWindowTileDataSelect = cpu.backgroundAndWindowTileDataSelect;
                    bool backgroundTileMapDisplaySelect = cpu.backgroundTileMapDisplaySelect;
                    int scrollX = cpu.scrollX;
                    int scrollY = cpu.scrollY;
                    bool windowDisplayed = cpu.windowDisplayed;
                    bool windowTileMapDisplaySelect = cpu.windowTileMapDisplaySelect;
                    int windowX = cpu.windowX - 7;
                    int windowY = cpu.windowY;

                    int windowPointY = windowY + y;

                    for (int x = 0; x < 160; x++, pixelIndex++)
                    {
                        uint intensity = 0;

                        if (backgroundDisplayed)
                        {
                            intensity = backgroundBuffer[0xFF & (scrollY + y), 0xFF & (scrollX + x)];
                        }

                        if (windowDisplayed && y >= windowY && y < windowY + 144 && x >= windowX && x < windowX + 160 && windowX >= -7 && windowX <= 159 && windowY >= 0 && windowY <= 143)
                        {
                            intensity = windowBuffer[y - windowY, x - windowX];
                        }

                        pixels[pixelIndex] = intensity;
                    }

                    if (cpu.spritesDisplayed)
                    {
                        uint[,,,] spriteTile = cpu.spriteTile;
                        if (cpu.largeSprites)
                        {
                            for (int address = 0; address < 160; address += 4)
                            {
                                int spriteY = oam[address];
                                int spriteX = oam[address + 1];
                                if (spriteY == 0 || spriteX == 0 || spriteY >= 160 || spriteX >= 168)
                                {
                                    continue;
                                }
                                spriteY -= 16;
                                if (spriteY > y || spriteY + 15 < y)
                                {
                                    continue;
                                }
                                spriteX -= 8;

                                int spriteTileIndex0 = 0xFE & oam[address + 2];
                                int spriteTileIndex1 = spriteTileIndex0 | 0x01;
                                int spriteFlags = oam[address + 3];
                                bool spritePriority = (0x80 & spriteFlags) == 0x80;
                                bool spriteYFlipped = (0x40 & spriteFlags) == 0x40;
                                bool spriteXFlipped = (0x20 & spriteFlags) == 0x20;
                                int spritePalette = (0x10 & spriteFlags) == 0x10 ? 1 : 0;

                                if (spriteYFlipped)
                                {
                                    int temp = spriteTileIndex0;
                                    spriteTileIndex0 = spriteTileIndex1;
                                    spriteTileIndex1 = temp;
                                }

                                int spriteRow = y - spriteY;
                                if (spriteRow >= 0 && spriteRow < 8)
                                {
                                    int screenAddress = (y << 7) + (y << 5) + spriteX;
                                    for (int x = 0; x < 8; x++, screenAddress++)
                                    {
                                        int screenX = spriteX + x;
                                        if (screenX >= 0 && screenX < 160)
                                        {
                                            uint color = spriteTile[spriteTileIndex0,
                                                spriteYFlipped ? 7 - spriteRow : spriteRow,
                                                spriteXFlipped ? 7 - x : x, spritePalette];
                                            if (color > 0)
                                            {
                                                if (spritePriority)
                                                {
                                                    if (pixels[screenAddress] == 0xFFFFFFFF)
                                                    {
                                                        pixels[screenAddress] = color;
                                                    }
                                                }
                                                else
                                                {
                                                    pixels[screenAddress] = color;
                                                }
                                            }
                                        }
                                    }
                                    continue;
                                }

                                spriteY += 8;

                                spriteRow = y - spriteY;
                                if (spriteRow >= 0 && spriteRow < 8)
                                {
                                    int screenAddress = (y << 7) + (y << 5) + spriteX;
                                    for (int x = 0; x < 8; x++, screenAddress++)
                                    {
                                        int screenX = spriteX + x;
                                        if (screenX >= 0 && screenX < 160)
                                        {
                                            uint color = spriteTile[spriteTileIndex1, spriteYFlipped ? 7 - spriteRow : spriteRow, spriteXFlipped ? 7 - x : x, spritePalette];
                                            if (color > 0)
                                            {
                                                if (spritePriority)
                                                {
                                                    if (pixels[screenAddress] == 0xFFFFFFFF)
                                                    {
                                                        pixels[screenAddress] = color;
                                                    }
                                                }
                                                else
                                                {
                                                    pixels[screenAddress] = color;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int address = 0; address < 160; address += 4)
                            {
                                int spriteY = oam[address];
                                int spriteX = oam[address + 1];
                                if (spriteY == 0 || spriteX == 0 || spriteY >= 160 || spriteX >= 168)
                                {
                                    continue;
                                }
                                spriteY -= 16;
                                if (spriteY > y || spriteY + 7 < y)
                                {
                                    continue;
                                }
                                spriteX -= 8;

                                int spriteTileIndex = oam[address + 2];
                                int spriteFlags = oam[address + 3];
                                bool spritePriority = (0x80 & spriteFlags) == 0x80;
                                bool spriteYFlipped = (0x40 & spriteFlags) == 0x40;
                                bool spriteXFlipped = (0x20 & spriteFlags) == 0x20;
                                int spritePalette = (0x10 & spriteFlags) == 0x10 ? 1 : 0;

                                int spriteRow = y - spriteY;
                                int screenAddress = (y << 7) + (y << 5) + spriteX;
                                for (int x = 0; x < 8; x++, screenAddress++)
                                {
                                    int screenX = spriteX + x;
                                    if (screenX >= 0 && screenX < 160)
                                    {
                                        uint color = spriteTile[spriteTileIndex, spriteYFlipped ? 7 - spriteRow : spriteRow, spriteXFlipped ? 7 - x : x, spritePalette];
                                        if (color > 0)
                                        {
                                            if (spritePriority)
                                            {
                                                if (pixels[screenAddress] == 0xFFFFFFFF)
                                                {
                                                    pixels[screenAddress] = color;
                                                }
                                            }
                                            else
                                            {
                                                pixels[screenAddress] = color;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    cpu.lcdcMode = LcdcModeType.HBlank;
                    if (cpu.lcdcInterruptEnabled && cpu.lcdcHBlankInterruptEnabled)
                    {
                        cpu.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }
            else
            {
                for (int y = 0; y < 144; y++)
                {
                    cpu.ly = y;
                    cpu.lcdcMode = LcdcModeType.SearchingOamRam;
                    if (cpu.lcdcInterruptEnabled && (cpu.lcdcOamInterruptEnabled || (cpu.lcdcLycLyCoincidenceInterruptEnabled && cpu.lyCompare == y)))
                    {
                        cpu.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    cpu.lcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);
                    cpu.lcdcMode = LcdcModeType.HBlank;
                    if (cpu.lcdcInterruptEnabled && cpu.lcdcHBlankInterruptEnabled)
                    {
                        cpu.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }

            cpu.lcdcMode = LcdcModeType.VBlank;
            if (cpu.vBlankInterruptEnabled)
            {
                cpu.vBlankInterruptRequested = true;
            }
            if (cpu.lcdcInterruptEnabled && cpu.lcdcVBlankInterruptEnabled)
            {
                cpu.lcdcInterruptRequested = true;
            }
            for (int y = 144; y <= 153; y++)
            {
                cpu.ly = y;
                if (cpu.lcdcInterruptEnabled && cpu.lcdcLycLyCoincidenceInterruptEnabled
                    && cpu.lyCompare == y)
                {
                    cpu.lcdcInterruptRequested = true;
                }
                ExecuteProcessor(4560);
                AddTicksPerScanLine();
            }
        }

        private void AddTicksPerScanLine()
        {
            switch (cpu.timerFrequency)
            {
                case TimerFrequencyType.hz4096:
                    scanLineTicks += 0.44329004329004329004329004329004;
                    break;
                case TimerFrequencyType.hz16384:
                    scanLineTicks += 1.7731601731601731601731601731602;
                    break;
                case TimerFrequencyType.hz65536:
                    scanLineTicks += 7.0926406926406926406926406926407;
                    break;
                case TimerFrequencyType.hz262144:
                    scanLineTicks += 28.370562770562770562770562770563;
                    break;
            }
            while (scanLineTicks >= 1.0)
            {
                scanLineTicks -= 1.0;
                if (cpu.timerCounter == 0xFF)
                {
                    cpu.timerCounter = cpu.timerModulo;
                    if (cpu.lcdcInterruptEnabled && cpu.timerOverflowInterruptEnabled)
                    {
                        cpu.timerOverflowInterruptRequested = true;
                    }
                }
                else
                {
                    cpu.timerCounter++;
                }
            }
        }

        private void ExecuteProcessor(int maxTicks)
        {
            do
            {
                cpu.Step();
                if (cpu.halted)
                {
                    cpu.ticks = ((maxTicks - cpu.ticks) & 0x03);
                    return;
                }
            } while (cpu.ticks < maxTicks);
            cpu.ticks -= maxTicks;
        }

        private void RenderFrame()
        {
            graphics.DrawImage(Display, 0, 0, DisplayWidth, DisplayHeight);
        }

        public void ExecuteFrame()
        {
            Execute(1);
        }

        public void ExecuteFrames(int n)
        {
            Execute(n);
        }

        public void Execute()
        {
            if (!anyButtonsPressed)
            {
                ExecuteFrames(2);
            }
            anyButtonsPressed = false;
        }

        public void Execute(TimeSpan time)
        {
            int frames = (int)(time.TotalSeconds * FramesPerSecond);
            Execute(frames);
        }

        private void Execute(int frames)
        {
            if (frames > 0)
            {
                for (int i = 0; i < frames - 1; i++)
                {
                    UpdateModel(false);
                }
                UpdateModel(true);
            }
            RenderFrame();
        }

        private void ExecuteWithoutRendering(int frames)
        {
            if (frames > 0)
            {
                for (int i = 0; i < frames; i++)
                {
                    UpdateModel(false);
                }
            }
        }

        private void PressButtonInternal(Button button)
        {
            cpu.KeyChanged(button, true);
        }

        private void ReleaseButtonInternal(Button button)
        {
            cpu.KeyChanged(button, false);
        }

        public void Hit(Button button)
        {
            if (Running)
            {
                anyButtonsPressed = true;

                PressButtonInternal(button);
                Execute(FramesAfterButton);
                ReleaseButtonInternal(button);
                Execute(FramesAfterButton);
            }
        }

        public void Press(Button button)
        {
            if (Running)
            {
                anyButtonsPressed = true;

                PressButtonInternal(button);
                Execute(FramesAfterButton);
            }
        }

        public void Release(Button button)
        {
            if (Running)
            {
                anyButtonsPressed = true;

                ReleaseButtonInternal(button);
                Execute(FramesAfterButton);
            }
        }
    }
}
