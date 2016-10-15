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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameBot.Emulation
{
    public class EmulatorAsync
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

        const int FramesPerSecond = 60;
        const int MaxFramesSkipped = 10;

        const int Width = GameBoyConstants.ScreenWidth;
        const int Height = GameBoyConstants.ScreenHeight;

        private string _output;
        public long Frequency = Stopwatch.Frequency;
        public long TicksPerFrame = Stopwatch.Frequency / FramesPerSecond;
        private Bitmap _bitmap;
        public Stopwatch Stopwatch = new Stopwatch();
        public long NextFrameStart;
        private X80 _x80;
        private Rectangle _rect;
        private double _scanLineTicks;
        private uint[] _pixels = new uint[Width * Height];
        private Game _game;
        private Size _clientSize = new Size(Width, Height);
        public Graphics Graphics;

        public EmulatorAsync()
        {
            _rect = new Rectangle(0, 0, 160, 144);
        }

        private void UpdateModel(bool updateBitmap)
        {
            if (updateBitmap)
            {
                uint[] backgroundPalette = _x80.BackgroundPalette;
                uint[] objectPalette0 = _x80.ObjectPalette0;
                uint[] objectPalette1 = _x80.ObjectPalette1;
                uint[,] backgroundBuffer = _x80.BackgroundBuffer;
                uint[,] windowBuffer = _x80.WindowBuffer;
                byte[] oam = _x80.Oam;

                for (int y = 0, pixelIndex = 0; y < 144; y++)
                {
                    _x80.Ly = y;
                    _x80.LcdcMode = LcdcModeType.SearchingOamRam;
                    if (_x80.LcdcInterruptEnabled
                        && (_x80.LcdcOamInterruptEnabled
                            || (_x80.LcdcLycLyCoincidenceInterruptEnabled && _x80.LyCompare == y)))
                    {
                        _x80.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    _x80.LcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);

                    _x80.UpdateWindow();
                    _x80.UpdateBackground();
                    _x80.UpdateSpriteTiles();

                    bool backgroundDisplayed = _x80.BackgroundDisplayed;
                    bool backgroundAndWindowTileDataSelect = _x80.BackgroundAndWindowTileDataSelect;
                    bool backgroundTileMapDisplaySelect = _x80.BackgroundTileMapDisplaySelect;
                    int scrollX = _x80.ScrollX;
                    int scrollY = _x80.ScrollY;
                    bool windowDisplayed = _x80.WindowDisplayed;
                    bool windowTileMapDisplaySelect = _x80.WindowTileMapDisplaySelect;
                    int windowX = _x80.WindowX - 7;
                    int windowY = _x80.WindowY;

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

                        _pixels[pixelIndex] = intensity;
                    }

                    if (_x80.SpritesDisplayed)
                    {
                        uint[,,,] spriteTile = _x80.SpriteTile;
                        if (_x80.LargeSprites)
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
                                                    if (_pixels[screenAddress] == 0xFFFFFFFF)
                                                    {
                                                        _pixels[screenAddress] = color;
                                                    }
                                                }
                                                else
                                                {
                                                    _pixels[screenAddress] = color;
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
                                                    if (_pixels[screenAddress] == 0xFFFFFFFF)
                                                    {
                                                        _pixels[screenAddress] = color;
                                                    }
                                                }
                                                else
                                                {
                                                    _pixels[screenAddress] = color;
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
                                                if (_pixels[screenAddress] == 0xFFFFFFFF)
                                                {
                                                    _pixels[screenAddress] = color;
                                                }
                                            }
                                            else
                                            {
                                                _pixels[screenAddress] = color;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _x80.LcdcMode = LcdcModeType.HBlank;
                    if (_x80.LcdcInterruptEnabled && _x80.LcdcHBlankInterruptEnabled)
                    {
                        _x80.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }
            else
            {
                for (int y = 0; y < 144; y++)
                {

                    _x80.Ly = y;
                    _x80.LcdcMode = LcdcModeType.SearchingOamRam;
                    if (_x80.LcdcInterruptEnabled
                        && (_x80.LcdcOamInterruptEnabled
                            || (_x80.LcdcLycLyCoincidenceInterruptEnabled && _x80.LyCompare == y)))
                    {
                        _x80.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    _x80.LcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);
                    _x80.LcdcMode = LcdcModeType.HBlank;
                    if (_x80.LcdcInterruptEnabled && _x80.LcdcHBlankInterruptEnabled)
                    {
                        _x80.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }

            _x80.LcdcMode = LcdcModeType.VBlank;
            if (_x80.VBlankInterruptEnabled)
            {
                _x80.VBlankInterruptRequested = true;
            }
            if (_x80.LcdcInterruptEnabled && _x80.LcdcVBlankInterruptEnabled)
            {
                _x80.LcdcInterruptRequested = true;
            }
            for (int y = 144; y <= 153; y++)
            {
                _x80.Ly = y;
                if (_x80.LcdcInterruptEnabled && _x80.LcdcLycLyCoincidenceInterruptEnabled
                    && _x80.LyCompare == y)
                {
                    _x80.LcdcInterruptRequested = true;
                }
                ExecuteProcessor(4560);
                AddTicksPerScanLine();
            }
        }

        private void AddTicksPerScanLine()
        {
            switch (_x80.TimerFrequency)
            {
                case TimerFrequencyType.Hz4096:
                    _scanLineTicks += 0.44329004329004329004329004329004;
                    break;
                case TimerFrequencyType.Hz16384:
                    _scanLineTicks += 1.7731601731601731601731601731602;
                    break;
                case TimerFrequencyType.Hz65536:
                    _scanLineTicks += 7.0926406926406926406926406926407;
                    break;
                case TimerFrequencyType.Hz262144:
                    _scanLineTicks += 28.370562770562770562770562770563;
                    break;
            }
            while (_scanLineTicks >= 1.0)
            {
                _scanLineTicks -= 1.0;
                if (_x80.TimerCounter == 0xFF)
                {
                    _x80.TimerCounter = _x80.TimerModulo;
                    if (_x80.LcdcInterruptEnabled && _x80.TimerOverflowInterruptEnabled)
                    {
                        _x80.TimerOverflowInterruptRequested = true;
                    }
                }
                else
                {
                    _x80.TimerCounter++;
                }
            }
        }

        private void ExecuteProcessor(int maxTicks)
        {
            do
            {
                _x80.Step();
                if (_x80.Halted)
                {
                    _x80.Ticks = ((maxTicks - _x80.Ticks) & 0x03);
                    return;
                }
            } while (_x80.Ticks < maxTicks);
            _x80.Ticks -= maxTicks;
        }

        private void RenderFrame(Graphics g)
        {
            g.DrawImage(_bitmap, 0, 0, Width, Height);
        }

        private void Init(Graphics g)
        {
            Graphics = g;

            // init image
            for (int i = 0; i < _pixels.Length; i++)
            {
                _pixels[i] = 0xFF000000;
            }
            GCHandle handle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(_pixels, 0);
            _bitmap = new Bitmap(160, 144, 160 * 4, PixelFormat.Format32bppPArgb, pointer);

            // start loop
            Stopwatch.Start();
            NextFrameStart = Stopwatch.ElapsedTicks;
            Running = true;
        }

        public void Init(string output)
        {
            _output = output;

            Bitmap bitmap = new Bitmap(160, 144);

            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            Init(g);
        }

        public void Run(Action callback)
        {
            if (_x80 == null || !Running)
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
                    NextFrameStart += TicksPerFrame;
                } while (NextFrameStart < Stopwatch.ElapsedTicks && ++updates < MaxFramesSkipped);
                RenderFrame(Graphics);
                _bitmap.Save(_output);
                long remainingTicks = NextFrameStart - Stopwatch.ElapsedTicks;
                if (remainingTicks > 0)
                {
                    Thread.Sleep((int)(1000 * remainingTicks / Frequency));
                }
                else if (updates == MaxFramesSkipped)
                {
                    NextFrameStart = Stopwatch.ElapsedTicks;
                }
            }
        }

        public void KeyDown(Button button)
        {
            _x80.KeyChanged(button, true);
        }

        public void KeyUp(Button button)
        {
            _x80.KeyChanged(button, false);
        }

        public void Open(string filename)
        {
            RomLoader romLoader = new RomLoader();
            _game = romLoader.Load(filename);
            _x80 = new X80();
            _x80.Cartridge = _game.Cartridge;
            _x80.PowerUp();
        }

        public string GetCatridgeInfo()
        {
            return _game.ToString();
        }
    }
}
