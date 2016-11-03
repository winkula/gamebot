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
    public class Emulator
    {
        private const int _framesPerSecond = 60;
        private const int _maxFramesSkipped = 10;

        private const int _displayWidth = GameBoyConstants.ScreenWidth;
        private const int _displayHeight = GameBoyConstants.ScreenHeight;

        private const int _framesAfterButton = 2;

        private readonly Random _random = new Random();
        private readonly X80 _cpu;
        private double _scanLineTicks;
        private readonly uint[] _pixels = new uint[_displayWidth * _displayHeight];

        private readonly Graphics _graphics;
        private readonly Bitmap _bitmap;

        public bool Running { get; private set; }
        public int Frames { get; private set; }
        public TimeSpan Time => TimeSpan.FromSeconds((double)Frames / _framesPerSecond);
        public Bitmap Display => _bitmap;
        public Game Game { get; private set; }
        private Size DisplaySize => new Size(_displayWidth, _displayHeight);

        private bool _anyButtonsPressed = false;
        private readonly double _errorProbability;

        public Emulator(double errorProbability = 0)
        {
            _cpu = new X80();
            _errorProbability = errorProbability;

            _graphics = Graphics.FromImage(new Bitmap(_displayWidth, _displayHeight));
            _graphics.CompositingQuality = CompositingQuality.HighSpeed;
            _graphics.CompositingMode = CompositingMode.SourceCopy;
            _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            // init image
            for (int i = 0; i < _pixels.Length; i++) { _pixels[i] = 0xFF000000; }
            GCHandle handle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
            IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(_pixels, 0);
            _bitmap = new Bitmap(_displayWidth, _displayHeight, _displayWidth * 4, PixelFormat.Format32bppPArgb, pointer);
        }

        public void Load(Game game)
        {
            Game = game;
            _cpu.Cartridge = Game.Cartridge;
            _cpu.PowerUp();

            Running = true;

            // strange effects appear in the first few frames , skip them
            Execute(4);
        }

        private void UpdateModel(bool updateBitmap)
        {
            Frames++;
            if (updateBitmap)
            {
                uint[] backgroundPalette = _cpu.BackgroundPalette;
                uint[] objectPalette0 = _cpu.ObjectPalette0;
                uint[] objectPalette1 = _cpu.ObjectPalette1;
                uint[,] backgroundBuffer = _cpu.BackgroundBuffer;
                uint[,] windowBuffer = _cpu.WindowBuffer;
                byte[] oam = _cpu.Oam;

                for (int y = 0, pixelIndex = 0; y < 144; y++)
                {
                    _cpu.Ly = y;
                    _cpu.LcdcMode = LcdcModeType.SearchingOamRam;
                    if (_cpu.LcdcInterruptEnabled && (_cpu.LcdcOamInterruptEnabled || (_cpu.LcdcLycLyCoincidenceInterruptEnabled && _cpu.LyCompare == y)))
                    {
                        _cpu.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    _cpu.LcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);

                    _cpu.UpdateWindow();
                    _cpu.UpdateBackground();
                    _cpu.UpdateSpriteTiles();

                    bool backgroundDisplayed = _cpu.BackgroundDisplayed;
                    bool backgroundAndWindowTileDataSelect = _cpu.BackgroundAndWindowTileDataSelect;
                    bool backgroundTileMapDisplaySelect = _cpu.BackgroundTileMapDisplaySelect;
                    int scrollX = _cpu.ScrollX;
                    int scrollY = _cpu.ScrollY;
                    bool windowDisplayed = _cpu.WindowDisplayed;
                    bool windowTileMapDisplaySelect = _cpu.WindowTileMapDisplaySelect;
                    int windowX = _cpu.WindowX - 7;
                    int windowY = _cpu.WindowY;

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

                        _pixels[pixelIndex] = intensity;
                    }

                    if (_cpu.SpritesDisplayed)
                    {
                        uint[,,,] spriteTile = _cpu.SpriteTile;
                        if (_cpu.LargeSprites)
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
                                            uint color = spriteTile[spriteTileIndex1, spriteYFlipped ? 7 - spriteRow : spriteRow, spriteXFlipped ? 7 - x : x, spritePalette];
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
                                        uint color = spriteTile[spriteTileIndex, spriteYFlipped ? 7 - spriteRow : spriteRow, spriteXFlipped ? 7 - x : x, spritePalette];
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

                    _cpu.LcdcMode = LcdcModeType.HBlank;
                    if (_cpu.LcdcInterruptEnabled && _cpu.LcdcHBlankInterruptEnabled)
                    {
                        _cpu.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }
            else
            {
                for (int y = 0; y < 144; y++)
                {
                    _cpu.Ly = y;
                    _cpu.LcdcMode = LcdcModeType.SearchingOamRam;
                    if (_cpu.LcdcInterruptEnabled && (_cpu.LcdcOamInterruptEnabled || (_cpu.LcdcLycLyCoincidenceInterruptEnabled && _cpu.LyCompare == y)))
                    {
                        _cpu.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(800);
                    _cpu.LcdcMode = LcdcModeType.TransferingData;
                    ExecuteProcessor(1720);
                    _cpu.LcdcMode = LcdcModeType.HBlank;
                    if (_cpu.LcdcInterruptEnabled && _cpu.LcdcHBlankInterruptEnabled)
                    {
                        _cpu.LcdcInterruptRequested = true;
                    }
                    ExecuteProcessor(2040);
                    AddTicksPerScanLine();
                }
            }

            _cpu.LcdcMode = LcdcModeType.VBlank;
            if (_cpu.VBlankInterruptEnabled)
            {
                _cpu.VBlankInterruptRequested = true;
            }
            if (_cpu.LcdcInterruptEnabled && _cpu.LcdcVBlankInterruptEnabled)
            {
                _cpu.LcdcInterruptRequested = true;
            }
            for (int y = 144; y <= 153; y++)
            {
                _cpu.Ly = y;
                if (_cpu.LcdcInterruptEnabled && _cpu.LcdcLycLyCoincidenceInterruptEnabled
                    && _cpu.LyCompare == y)
                {
                    _cpu.LcdcInterruptRequested = true;
                }
                ExecuteProcessor(4560);
                AddTicksPerScanLine();
            }
        }

        private void AddTicksPerScanLine()
        {
            switch (_cpu.TimerFrequency)
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
                if (_cpu.TimerCounter == 0xFF)
                {
                    _cpu.TimerCounter = _cpu.TimerModulo;
                    if (_cpu.LcdcInterruptEnabled && _cpu.TimerOverflowInterruptEnabled)
                    {
                        _cpu.TimerOverflowInterruptRequested = true;
                    }
                }
                else
                {
                    _cpu.TimerCounter++;
                }
            }
        }

        private void ExecuteProcessor(int maxTicks)
        {
            do
            {
                _cpu.Step();
                if (_cpu.Halted)
                {
                    _cpu.Ticks = ((maxTicks - _cpu.Ticks) & 0x03);
                    return;
                }
            } while (_cpu.Ticks < maxTicks);
            _cpu.Ticks -= maxTicks;
        }

        private void RenderFrame()
        {
            _graphics.DrawImage(Display, 0, 0, _displayWidth, _displayHeight);
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
            if (!_anyButtonsPressed)
            {
                ExecuteFrames(2);
            }
            _anyButtonsPressed = false;
        }

        public int GetExecutionDurationInFrames(TimeSpan time)
        {
            return (int)(time.TotalSeconds * _framesPerSecond);
        }

        public void Execute(TimeSpan time)
        {
            int frames = (int)(time.TotalSeconds * _framesPerSecond);
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
            _cpu.KeyChanged(button, true);
        }

        private void ReleaseButtonInternal(Button button)
        {
            _cpu.KeyChanged(button, false);
        }

        private bool IsError()
        {
            var value = _random.NextDouble();
            return value < _errorProbability;
        }

        public void Hit(Button button)
        {
            if (Running && !IsError())
            {
                _anyButtonsPressed = true;

                PressButtonInternal(button);
                Execute(_framesAfterButton);
                ReleaseButtonInternal(button);
                Execute(_framesAfterButton);
            }
        }

        public void Press(Button button)
        {
            if (Running && !IsError())
            {
                _anyButtonsPressed = true;

                PressButtonInternal(button);
                Execute(_framesAfterButton);
            }
        }

        public void Release(Button button)
        {
            if (Running && !IsError())
            {
                _anyButtonsPressed = true;

                ReleaseButtonInternal(button);
                Execute(_framesAfterButton);
            }
        }
    }
}
