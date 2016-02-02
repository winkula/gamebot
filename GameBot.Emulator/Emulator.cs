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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameBot.Emulator
{
    public class Emulator
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PeekMsg
        {
            public IntPtr hWnd;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        public bool Running;

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        const int FRAMES_PER_SECOND = 60;
        const int MAX_FRAMES_SKIPPED = 10;

        const int WIDTH = 160;
        const int HEIGHT = 144;

        private string output;
        public long FREQUENCY = Stopwatch.Frequency;
        public long TICKS_PER_FRAME = Stopwatch.Frequency / FRAMES_PER_SECOND;
        private Bitmap bitmap;
        public Stopwatch stopwatch = new Stopwatch();
        public long nextFrameStart;
        private X80 x80;
        private Rectangle rect;
        private double scanLineTicks;
        private uint[] pixels = new uint[WIDTH * HEIGHT];
        private Game game;
        private Size ClientSize = new Size(WIDTH, HEIGHT);
        public Graphics graphics;

        public Emulator()
        {
            rect = new Rectangle(0, 0, 160, 144);
        }

        private void UpdateModel(bool updateBitmap)
        {
            if (updateBitmap)
            {
                uint[] backgroundPalette = x80.backgroundPalette;
                uint[] objectPalette0 = x80.objectPalette0;
                uint[] objectPalette1 = x80.objectPalette1;
                uint[,] backgroundBuffer = x80.backgroundBuffer;
                uint[,] windowBuffer = x80.windowBuffer;
                byte[] oam = x80.oam;

                for (int y = 0, pixelIndex = 0; y < 144; y++)
                {
                    x80.ly = y;
                    x80.lcdcMode = LcdcModeType.SearchingOamRam;
                    if (x80.lcdcInterruptEnabled
                        && (x80.lcdcOamInterruptEnabled
                            || (x80.lcdcLycLyCoincidenceInterruptEnabled && x80.lyCompare == y)))
                    {
                        x80.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    x80.lcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);

                    x80.UpdateWindow();
                    x80.UpdateBackground();
                    x80.UpdateSpriteTiles();

                    bool backgroundDisplayed = x80.backgroundDisplayed;
                    bool backgroundAndWindowTileDataSelect = x80.backgroundAndWindowTileDataSelect;
                    bool backgroundTileMapDisplaySelect = x80.backgroundTileMapDisplaySelect;
                    int scrollX = x80.scrollX;
                    int scrollY = x80.scrollY;
                    bool windowDisplayed = x80.windowDisplayed;
                    bool windowTileMapDisplaySelect = x80.windowTileMapDisplaySelect;
                    int windowX = x80.windowX - 7;
                    int windowY = x80.windowY;

                    int windowPointY = windowY + y;

                    for (int x = 0; x < 160; x++, pixelIndex++)
                    {

                        uint intensity = 0;

                        if (backgroundDisplayed)
                        {
                            intensity = backgroundBuffer[0xFF & (scrollY + y), 0xFF & (scrollX + x)];
                        }

                        if (windowDisplayed && y >= windowY && y < windowY + 144 && x >= windowX && x < windowX + 160
                            && windowX >= -7 && windowX <= 159 && windowY >= 0 && windowY <= 143)
                        {
                            intensity = windowBuffer[y - windowY, x - windowX];
                        }

                        pixels[pixelIndex] = intensity;
                    }

                    if (x80.spritesDisplayed)
                    {
                        uint[,,,] spriteTile = x80.spriteTile;
                        if (x80.largeSprites)
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
                                            uint color = spriteTile[spriteTileIndex1,
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
                                        uint color = spriteTile[spriteTileIndex,
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
                            }
                        }
                    }

                    x80.lcdcMode = LcdcModeType.HBlank;
                    if (x80.lcdcInterruptEnabled && x80.lcdcHBlankInterruptEnabled)
                    {
                        x80.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }
            else
            {
                for (int y = 0; y < 144; y++)
                {

                    x80.ly = y;
                    x80.lcdcMode = LcdcModeType.SearchingOamRam;
                    if (x80.lcdcInterruptEnabled
                        && (x80.lcdcOamInterruptEnabled
                            || (x80.lcdcLycLyCoincidenceInterruptEnabled && x80.lyCompare == y)))
                    {
                        x80.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    x80.lcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);
                    x80.lcdcMode = LcdcModeType.HBlank;
                    if (x80.lcdcInterruptEnabled && x80.lcdcHBlankInterruptEnabled)
                    {
                        x80.lcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }

            x80.lcdcMode = LcdcModeType.VBlank;
            if (x80.vBlankInterruptEnabled)
            {
                x80.vBlankInterruptRequested = true;
            }
            if (x80.lcdcInterruptEnabled && x80.lcdcVBlankInterruptEnabled)
            {
                x80.lcdcInterruptRequested = true;
            }
            for (int y = 144; y <= 153; y++)
            {
                x80.ly = y;
                if (x80.lcdcInterruptEnabled && x80.lcdcLycLyCoincidenceInterruptEnabled
                    && x80.lyCompare == y)
                {
                    x80.lcdcInterruptRequested = true;
                }
                ExecuteProcessor(4560);
                AddTicksPerScanLine();
            }
        }

        private void AddTicksPerScanLine()
        {
            switch (x80.timerFrequency)
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
                if (x80.timerCounter == 0xFF)
                {
                    x80.timerCounter = x80.timerModulo;
                    if (x80.lcdcInterruptEnabled && x80.timerOverflowInterruptEnabled)
                    {
                        x80.timerOverflowInterruptRequested = true;
                    }
                }
                else
                {
                    x80.timerCounter++;
                }
            }
        }

        private void ExecuteProcessor(int maxTicks)
        {
            do
            {
                x80.Step();
                if (x80.halted)
                {
                    x80.ticks = ((maxTicks - x80.ticks) & 0x03);
                    return;
                }
            } while (x80.ticks < maxTicks);
            x80.ticks -= maxTicks;
        }

        private void RenderFrame(Graphics g)
        {
            g.DrawImage(bitmap, 0, 0, WIDTH, HEIGHT);
        }

        private void Init(Graphics g)
        {
            graphics = g;

            // init image
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = 0xFF000000;
            }
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(pixels, 0);
            bitmap = new Bitmap(160, 144, 160 * 4, PixelFormat.Format32bppPArgb, pointer);

            // start loop
            stopwatch.Start();
            nextFrameStart = stopwatch.ElapsedTicks;
            Running = true;
        }

        public void Init(string output)
        {
            this.output = output;

            Bitmap bitmap = new Bitmap(160, 144);

            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Init(g);
        }

        public void Run(Action callback)
        {
            if (x80 == null || !Running)
            {
                return;
            }
            PeekMsg msg;
            while (!PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
            {
                int updates = 0;
                bool updateBitmap = true;
                do
                {
                    if (callback != null)
                    {
                        callback();
                    }

                    UpdateModel(updateBitmap);
                    updateBitmap = false;
                    nextFrameStart += TICKS_PER_FRAME;
                } while (nextFrameStart < stopwatch.ElapsedTicks && ++updates < MAX_FRAMES_SKIPPED);
                RenderFrame(graphics);
                bitmap.Save(output);
                long remainingTicks = nextFrameStart - stopwatch.ElapsedTicks;
                if (remainingTicks > 0)
                {
                    Thread.Sleep((int)(1000 * remainingTicks / FREQUENCY));
                }
                else if (updates == MAX_FRAMES_SKIPPED)
                {
                    nextFrameStart = stopwatch.ElapsedTicks;
                }
            }
        }

        public void KeyDown(Keys key)
        {
            x80.KeyChanged(key, true);
        }

        public void KeyUp(Keys key)
        {
            x80.KeyChanged(key, false);
        }

        public void Open(string filename)
        {
            RomLoader romLoader = new RomLoader();
            game = romLoader.Load(filename);
            x80 = new X80();
            x80.cartridge = game.cartridge;
            x80.PowerUp();
        }

        public string GetCatridgeInfo()
        {
            return game.ToString();
        }
    }
}
