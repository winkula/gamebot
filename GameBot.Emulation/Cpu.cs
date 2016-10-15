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

using GameBot.Core.Data;
using System;

namespace GameBot.Emulation
{
    public enum LcdcModeType
    {
        HBlank = 0, VBlank = 1,
        SearchingOamRam = 2,
        TransferingData = 3
    }

    public enum TimerFrequencyType
    {
        Hz4096 = 0,
        Hz262144 = 1, Hz65536 = 2,
        Hz16384 = 3
    }

    public sealed class X80
    {
        public const uint White = 0xFFFFFFFF;
        public const uint LightGray = 0xFFAAAAAA;
        public const uint DarkGray = 0xFF555555;
        public const uint Black = 0xFF000000;

        public ICartridge Cartridge;
        private int _a, _b, _c, _d, _e, _h, _l, _pc, _sp;
        private bool _fz, _fc, _fh, _fn;
        public bool Halted;
        public bool Stopped;
        public bool InterruptsEnabled = true;
        private bool _stopCounting;
        public bool LeftKeyPressed;
        public bool RightKeyPressed;
        public bool UpKeyPressed;
        public bool DownKeyPressed;
        public bool AButtonPressed;
        public bool BButtonPressed;
        public bool StartButtonPressed;
        public bool SelectButtonPressed;
        public bool KeyP14, KeyP15;
        public bool KeyPressedInterruptRequested;
        public bool SerialIoTransferCompleteInterruptRequested;
        public bool TimerOverflowInterruptRequested;
        public bool LcdcInterruptRequested;
        public bool VBlankInterruptRequested;
        public bool KeyPressedInterruptEnabled;
        public bool SerialIoTransferCompleteInterruptEnabled;
        public bool TimerOverflowInterruptEnabled;
        public bool LcdcInterruptEnabled;
        public bool VBlankInterruptEnabled;
        public bool LcdControlOperationEnabled;
        public bool WindowTileMapDisplaySelect;
        public bool WindowDisplayed;
        public bool BackgroundAndWindowTileDataSelect;
        public bool BackgroundTileMapDisplaySelect;
        public bool LargeSprites;
        public bool SpritesDisplayed;
        public bool BackgroundDisplayed;
        public int ScrollX, ScrollY;
        public int WindowX, WindowY;
        public int LyCompare, Ly;
        public uint[] BackgroundPalette = { White, LightGray, DarkGray, Black };
        public uint[] ObjectPalette0 = { White, LightGray, DarkGray, Black };
        public uint[] ObjectPalette1 = { White, LightGray, DarkGray, Black };
        public bool LcdcLycLyCoincidenceInterruptEnabled;
        public bool LcdcOamInterruptEnabled;
        public bool LcdcVBlankInterruptEnabled;
        public bool LcdcHBlankInterruptEnabled;
        public LcdcModeType LcdcMode;
        public bool TimerRunning;
        public int TimerCounter;
        public int TimerModulo;
        public TimerFrequencyType TimerFrequency;
        public int Ticks;

        private byte[] _highRam = new byte[256];
        private byte[] _videoRam = new byte[8 * 1024];
        private byte[] _workRam = new byte[8 * 1024];
        public byte[] Oam = new byte[256];

        public uint[,] BackgroundBuffer = new uint[256, 256];
        private bool[,] _backgroundTileInvalidated = new bool[32, 32];
        private bool _invalidateAllBackgroundTilesRequest;
        public uint[,,,] SpriteTile = new uint[256, 8, 8, 2];
        private bool[] _spriteTileInvalidated = new bool[256];
        private bool _invalidateAllSpriteTilesRequest;
        public uint[,] WindowBuffer = new uint[144, 168];

        public X80()
        {
        }

        public void Step()
        {

            //CheckForBadState();

            if (InterruptsEnabled)
            {
                if (VBlankInterruptEnabled && VBlankInterruptRequested)
                {
                    VBlankInterruptRequested = false;
                    Interrupt(0x0040);
                }
                else if (LcdcInterruptEnabled && LcdcInterruptRequested)
                {
                    LcdcInterruptRequested = false;
                    Interrupt(0x0048);
                }
                else if (TimerOverflowInterruptEnabled && TimerOverflowInterruptRequested)
                {
                    TimerOverflowInterruptRequested = false;
                    Interrupt(0x0050);
                }
                else if (SerialIoTransferCompleteInterruptEnabled && SerialIoTransferCompleteInterruptRequested)
                {
                    SerialIoTransferCompleteInterruptRequested = false;
                    Interrupt(0x0058);
                }
                else if (KeyPressedInterruptEnabled && KeyPressedInterruptRequested)
                {
                    KeyPressedInterruptRequested = false;
                    Interrupt(0x0060);
                }
            }

            _pc &= 0xFFFF;

            int opCode = 0x00;
            if (!Halted)
            {
                opCode = ReadByte(_pc);
                if (_stopCounting)
                {
                    _stopCounting = false;
                }
                else
                {
                    _pc++;
                }
            }

            switch (opCode)
            {
                case 0x00: // NOP
                case 0xD3:
                case 0xDB:
                case 0xDD:
                case 0xE3:
                case 0xE4:
                case 0xEB:
                case 0xEC:
                case 0xF4:
                case 0xFC:
                case 0xFD:
                    NoOperation();
                    break;
                case 0x01: // LD BC,NN
                    LoadImmediate(ref _b, ref _c);
                    break;
                case 0x02: // LD (BC),A
                    WriteByte(_b, _c, _a);
                    break;
                case 0x03: // INC BC
                    Increment(ref _b, ref _c);
                    break;
                case 0x04: // INC B
                    Increment(ref _b);
                    break;
                case 0x05: // DEC B 
                    Decrement(ref _b);
                    break;
                case 0x06: // LD B,N
                    LoadImmediate(ref _b);
                    break;
                case 0x07: // RLCA
                    RotateALeft();
                    break;
                case 0x08: // LD (word),SP
                    WriteWordToImmediateAddress(_sp);
                    break;
                case 0x09: // ADD HL,BC
                    Add(ref _h, ref _l, _b, _c);
                    break;
                case 0x0A: // LD A,(BC)
                    ReadByte(ref _a, _b, _c);
                    break;
                case 0x0B: // DEC BC
                    Decrement(ref _b, ref _c);
                    break;
                case 0x0C: // INC C
                    Increment(ref _c);
                    break;
                case 0x0D: // DEC C
                    Decrement(ref _c);
                    break;
                case 0x0E: // LD C,N
                    LoadImmediate(ref _c);
                    break;
                case 0x0F: // RRCA
                    RotateARight();
                    break;
                case 0x10: // STOP
                    Stopped = true;
                    Ticks += 4;
                    break;
                case 0x11: // LD DE,NN
                    LoadImmediate(ref _d, ref _e);
                    break;
                case 0x12: // LD (DE),A
                    WriteByte(_d, _e, _a);
                    break;
                case 0x13: // INC DE
                    Increment(ref _d, ref _e);
                    break;
                case 0x14: // INC D
                    Increment(ref _d);
                    break;
                case 0x15: // DEC D
                    Decrement(ref _d);
                    break;
                case 0x16: // LD D,N
                    LoadImmediate(ref _d);
                    break;
                case 0x17: // RLA
                    RotateALeftThroughCarry();
                    break;
                case 0x18: // JR N
                    JumpRelative();
                    break;
                case 0x19: // ADD HL,DE
                    Add(ref _h, ref _l, _d, _e);
                    break;
                case 0x1A: // LD A,(DE)
                    ReadByte(ref _a, _d, _e);
                    break;
                case 0x1B: // DEC DE
                    Decrement(ref _d, ref _e);
                    break;
                case 0x1C: // INC E
                    Increment(ref _e);
                    break;
                case 0x1D: // DEC E
                    Decrement(ref _e);
                    break;
                case 0x1E: // LD E,N
                    LoadImmediate(ref _e);
                    break;
                case 0x1F: // RRA
                    RotateARightThroughCarry();
                    break;
                case 0x20: // JR NZ,N
                    JumpRelativeIfNotZero();
                    break;
                case 0x21: // LD HL,NN
                    LoadImmediate(ref _h, ref _l);
                    break;
                case 0x22: // LD (HLI),A
                    WriteByte(_h, _l, _a);
                    Increment(ref _h, ref _l);
                    break;
                case 0x23: // INC HL
                    Increment(ref _h, ref _l);
                    break;
                case 0x24: // INC H
                    Increment(ref _h);
                    break;
                case 0x25: // DEC H
                    Decrement(ref _h);
                    break;
                case 0x26: // LD H,N
                    LoadImmediate(ref _h);
                    break;
                case 0x27: // DAA
                    DecimallyAdjustA();
                    break;
                case 0x28: // JR Z,N
                    JumpRelativeIfZero();
                    break;
                case 0x29: // ADD HL,HL
                    Add(ref _h, ref _l, _h, _l);
                    break;
                case 0x2A: // LD A,(HLI)
                    ReadByte(ref _a, _h, _l);
                    Increment(ref _h, ref _l);
                    break;
                case 0x2B: // DEC HL
                    Decrement(ref _h, ref _l);
                    break;
                case 0x2C: // INC L
                    Increment(ref _l);
                    break;
                case 0x2D: // DEC L
                    Decrement(ref _l);
                    break;
                case 0x2E: // LD L,N
                    LoadImmediate(ref _l);
                    break;
                case 0x2F: // CPL
                    ComplementA();
                    break;
                case 0x30: // JR NC,N
                    JumpRelativeIfNotCarry();
                    break;
                case 0x31: // LD SP,NN
                    LoadImmediateWord(ref _sp);
                    break;
                case 0x32: // LD (HLD),A
                    WriteByte(_h, _l, _a);
                    Decrement(ref _h, ref _l);
                    break;
                case 0x33: // INC SP
                    IncrementWord(ref _sp);
                    break;
                case 0x34: // INC (HL)
                    IncrementMemory(_h, _l);
                    break;
                case 0x35: // DEC (HL)
                    DecrementMemory(_h, _l);
                    break;
                case 0x36: // LD (HL),N
                    LoadImmediateIntoMemory(_h, _l);
                    break;
                case 0x37: // SCF
                    SetCarryFlag();
                    break;
                case 0x38: // JR C,N
                    JumpRelativeIfCarry();
                    break;
                case 0x39: // ADD HL,SP
                    AddSptoHl();
                    break;
                case 0x3A: // LD A,(HLD)
                    ReadByte(ref _a, _h, _l);
                    Decrement(ref _h, ref _l);
                    break;
                case 0x3B: // DEC SP
                    DecrementWord(ref _sp);
                    break;
                case 0x3C: // INC A
                    Increment(ref _a);
                    break;
                case 0x3D: // DEC A
                    Decrement(ref _a);
                    break;
                case 0x3E: // LD A,N
                    LoadImmediate(ref _a);
                    break;
                case 0x3F: // CCF
                    ComplementCarryFlag();
                    break;
                case 0x40: // LD B,B
                    Load(ref _b, _b);
                    break;
                case 0x41: // LD B,C
                    Load(ref _b, _c);
                    break;
                case 0x42: // LD B,D
                    Load(ref _b, _d);
                    break;
                case 0x43: // LD B,E
                    Load(ref _b, _e);
                    break;
                case 0x44: // LD B,H
                    Load(ref _b, _h);
                    break;
                case 0x45: // LD B,L
                    Load(ref _b, _l);
                    break;
                case 0x46: // LD B,(HL)
                    ReadByte(ref _b, _h, _l);
                    break;
                case 0x47: // LD B,A
                    Load(ref _b, _a);
                    break;
                case 0x48: // LD C,B
                    Load(ref _c, _b);
                    break;
                case 0x49: // LD C,C
                    Load(ref _c, _c);
                    break;
                case 0x4A: // LD C,D
                    Load(ref _c, _d);
                    break;
                case 0x4B: // LD C,E
                    Load(ref _c, _e);
                    break;
                case 0x4C: // LD C,H
                    Load(ref _c, _h);
                    break;
                case 0x4D: // LD C,L
                    Load(ref _c, _l);
                    break;
                case 0x4E: // LD C,(HL)
                    ReadByte(ref _c, _h, _l);
                    break;
                case 0x4F: // LD C,A
                    Load(ref _c, _a);
                    break;
                case 0x50: // LD D,B
                    Load(ref _d, _b);
                    break;
                case 0x51: // LD D,C
                    Load(ref _d, _c);
                    break;
                case 0x52: // LD D,D
                    Load(ref _d, _d);
                    break;
                case 0x53: // LD D,E
                    Load(ref _d, _e);
                    break;
                case 0x54: // LD D,H
                    Load(ref _d, _h);
                    break;
                case 0x55: // LD D,L
                    Load(ref _d, _l);
                    break;
                case 0x56: // LD D,(HL)
                    ReadByte(ref _d, _h, _l);
                    break;
                case 0x57: // LD D,A
                    Load(ref _d, _a);
                    break;
                case 0x58: // LD E,B
                    Load(ref _e, _b);
                    break;
                case 0x59: // LD E,C
                    Load(ref _e, _c);
                    break;
                case 0x5A: // LD E,D
                    Load(ref _e, _d);
                    break;
                case 0x5B: // LD E,E
                    Load(ref _e, _e);
                    break;
                case 0x5C: // LD E,H
                    Load(ref _e, _h);
                    break;
                case 0x5D: // LD E,L
                    Load(ref _e, _l);
                    break;
                case 0x5E: // LD E,(HL)
                    ReadByte(ref _e, _h, _l);
                    break;
                case 0x5F: // LD E,A
                    Load(ref _e, _a);
                    break;
                case 0x60: // LD H,B
                    Load(ref _h, _b);
                    break;
                case 0x61: // LD H,C
                    Load(ref _h, _c);
                    break;
                case 0x62: // LD H,D
                    Load(ref _h, _d);
                    break;
                case 0x63: // LD H,E
                    Load(ref _h, _e);
                    break;
                case 0x64: // LD H,H
                    Load(ref _h, _h);
                    break;
                case 0x65: // LD H,L
                    Load(ref _h, _l);
                    break;
                case 0x66: // LD H,(HL)
                    ReadByte(ref _h, _h, _l);
                    break;
                case 0x67: // LD H,A
                    Load(ref _h, _a);
                    break;
                case 0x68: // LD L,B
                    Load(ref _l, _b);
                    break;
                case 0x69: // LD L,C
                    Load(ref _l, _c);
                    break;
                case 0x6A: // LD L,D
                    Load(ref _l, _d);
                    break;
                case 0x6B: // LD L,E
                    Load(ref _l, _e);
                    break;
                case 0x6C: // LD L,H
                    Load(ref _l, _h);
                    break;
                case 0x6D: // LD L,L
                    Load(ref _l, _l);
                    break;
                case 0x6E: // LD L,(HL)
                    ReadByte(ref _l, _h, _l);
                    break;
                case 0x6F: // LD L,A
                    Load(ref _l, _a);
                    break;
                case 0x70: // LD (HL),B
                    WriteByte(_h, _l, _b);
                    break;
                case 0x71: // LD (HL),C
                    WriteByte(_h, _l, _c);
                    break;
                case 0x72: // LD (HL),D
                    WriteByte(_h, _l, _d);
                    break;
                case 0x73: // LD (HL),E
                    WriteByte(_h, _l, _e);
                    break;
                case 0x74: // LD (HL),H
                    WriteByte(_h, _l, _h);
                    break;
                case 0x75: // LD (HL),L
                    WriteByte(_h, _l, _l);
                    break;
                case 0x76: // HALT
                    Halt();
                    break;
                case 0x77: // LD (HL),A
                    WriteByte(_h, _l, _a);
                    break;
                case 0x78: // LD A,B
                    Load(ref _a, _b);
                    break;
                case 0x79: // LD A,C
                    Load(ref _a, _c);
                    break;
                case 0x7A: // LD A,D
                    Load(ref _a, _d);
                    break;
                case 0x7B: // LD A,E
                    Load(ref _a, _e);
                    break;
                case 0x7C: // LD A,H
                    Load(ref _a, _h);
                    break;
                case 0x7D: // LD A,L
                    Load(ref _a, _l);
                    break;
                case 0x7E: // LD A,(HL)
                    ReadByte(ref _a, _h, _l);
                    break;
                case 0x7F: // LD A,A
                    Load(ref _a, _a);
                    break;
                case 0x80: // ADD A,B
                    Add(_b);
                    break;
                case 0x81: // ADD A,C
                    Add(_c);
                    break;
                case 0x82: // ADD A,D
                    Add(_d);
                    break;
                case 0x83: // ADD A,E
                    Add(_e);
                    break;
                case 0x84: // ADD A,H
                    Add(_h);
                    break;
                case 0x85: // ADD A,L
                    Add(_l);
                    break;
                case 0x86: // ADD A,(HL)
                    Add(_h, _l);
                    break;
                case 0x87: // ADD A,A
                    Add(_a);
                    break;
                case 0x88: // ADC A,B
                    AddWithCarry(_b);
                    break;
                case 0x89: // ADC A,C
                    AddWithCarry(_c);
                    break;
                case 0x8A: // ADC A,D
                    AddWithCarry(_d);
                    break;
                case 0x8B: // ADC A,E
                    AddWithCarry(_e);
                    break;
                case 0x8C: // ADC A,H
                    AddWithCarry(_h);
                    break;
                case 0x8D: // ADC A,L
                    AddWithCarry(_l);
                    break;
                case 0x8E: // ADC A,(HL)
                    AddWithCarry(_h, _l);
                    break;
                case 0x8F: // ADC A,A
                    AddWithCarry(_a);
                    break;
                case 0x90: // SUB B
                    Sub(_b);
                    break;
                case 0x91: // SUB C
                    Sub(_c);
                    break;
                case 0x92: // SUB D
                    Sub(_d);
                    break;
                case 0x93: // SUB E
                    Sub(_e);
                    break;
                case 0x94: // SUB H
                    Sub(_h);
                    break;
                case 0x95: // SUB L
                    Sub(_l);
                    break;
                case 0x96: // SUB (HL)
                    Sub(_h, _l);
                    break;
                case 0x97: // SUB A
                    Sub(_a);
                    break;
                case 0x98: // SBC B
                    SubWithBorrow(_b);
                    break;
                case 0x99: // SBC C
                    SubWithBorrow(_c);
                    break;
                case 0x9A: // SBC D
                    SubWithBorrow(_d);
                    break;
                case 0x9B: // SBC E
                    SubWithBorrow(_e);
                    break;
                case 0x9C: // SBC H
                    SubWithBorrow(_h);
                    break;
                case 0x9D: // SBC L
                    SubWithBorrow(_l);
                    break;
                case 0x9E: // SBC (HL)
                    SubWithBorrow(_h, _l);
                    break;
                case 0x9F: // SBC A
                    SubWithBorrow(_a);
                    break;
                case 0xA0: // AND B
                    And(_b);
                    break;
                case 0xA1: // AND C
                    And(_c);
                    break;
                case 0xA2: // AND D
                    And(_d);
                    break;
                case 0xA3: // AND E
                    And(_e);
                    break;
                case 0xA4: // AND H
                    And(_h);
                    break;
                case 0xA5: // AND L
                    And(_l);
                    break;
                case 0xA6: // AND (HL)
                    And(_h, _l);
                    break;
                case 0xA7: // AND A
                    And(_a);
                    break;
                case 0xA8: // XOR B
                    Xor(_b);
                    break;
                case 0xA9: // XOR C
                    Xor(_c);
                    break;
                case 0xAA: // XOR D
                    Xor(_d);
                    break;
                case 0xAB: // XOR E
                    Xor(_e);
                    break;
                case 0xAC: // XOR H
                    Xor(_h);
                    break;
                case 0xAD: // XOR L
                    Xor(_l);
                    break;
                case 0xAE: // XOR (HL)
                    Xor(_h, _l);
                    break;
                case 0xAF: // XOR A
                    Xor(_a);
                    break;
                case 0xB0: // OR B
                    Or(_b);
                    break;
                case 0xB1: // OR C
                    Or(_c);
                    break;
                case 0xB2: // OR D
                    Or(_d);
                    break;
                case 0xB3: // OR E
                    Or(_e);
                    break;
                case 0xB4: // OR H
                    Or(_h);
                    break;
                case 0xB5: // OR L
                    Or(_l);
                    break;
                case 0xB6: // OR (HL)
                    Or(_h, _l);
                    break;
                case 0xB7: // OR A
                    Or(_a);
                    break;
                case 0xB8: // CP B
                    Compare(_b);
                    break;
                case 0xB9: // CP C
                    Compare(_c);
                    break;
                case 0xBA: // CP D
                    Compare(_d);
                    break;
                case 0xBB: // CP E
                    Compare(_e);
                    break;
                case 0xBC: // CP H
                    Compare(_h);
                    break;
                case 0xBD: // CP L
                    Compare(_l);
                    break;
                case 0xBE: // CP (HL)
                    Compare(_h, _l);
                    break;
                case 0xBF: // CP A
                    Compare(_a);
                    break;
                case 0xC0: // RET NZ
                    ReturnIfNotZero();
                    break;
                case 0xC1: // POP BC
                    Pop(ref _b, ref _c);
                    break;
                case 0xC2: // JP NZ,N
                    JumpIfNotZero();
                    break;
                case 0xC3: // JP N
                    Jump();
                    break;
                case 0xC4: // CALL NZ,NN
                    CallIfNotZero();
                    break;
                case 0xC5: // PUSH BC
                    Push(_b, _c);
                    break;
                case 0xC6: // ADD A,N
                    AddImmediate();
                    break;
                case 0xC7: // RST 00H
                    Restart(0);
                    break;
                case 0xC8: // RET Z
                    ReturnIfZero();
                    break;
                case 0xC9: // RET
                    Return();
                    break;
                case 0xCA: // JP Z,N
                    JumpIfZero();
                    break;
                case 0xCB:
                    switch (ReadByte(_pc++))
                    {
                        case 0x00: // RLC B
                            RotateLeft(ref _b);
                            break;
                        case 0x01: // RLC C
                            RotateLeft(ref _c);
                            break;
                        case 0x02: // RLC D
                            RotateLeft(ref _d);
                            break;
                        case 0x03: // RLC E
                            RotateLeft(ref _e);
                            break;
                        case 0x04: // RLC H
                            RotateLeft(ref _h);
                            break;
                        case 0x05: // RLC L
                            RotateLeft(ref _l);
                            break;
                        case 0x06: // RLC (HL)
                            RotateLeft(_h, _l);
                            break;
                        case 0x07: // RLC A
                            RotateLeft(ref _a);
                            break;
                        case 0x08: // RRC B
                            RotateRight(ref _b);
                            break;
                        case 0x09: // RRC C
                            RotateRight(ref _c);
                            break;
                        case 0x0A: // RRC D
                            RotateRight(ref _d);
                            break;
                        case 0x0B: // RRC E
                            RotateRight(ref _e);
                            break;
                        case 0x0C: // RRC H
                            RotateRight(ref _h);
                            break;
                        case 0x0D: // RRC L
                            RotateRight(ref _l);
                            break;
                        case 0x0E: // RRC (HL)
                            RotateRight(_h, _l);
                            break;
                        case 0x0F: // RRC A
                            RotateRight(ref _a);
                            break;
                        case 0x10: // RL  B
                            RotateLeftThroughCarry(ref _b);
                            break;
                        case 0x11: // RL  C
                            RotateLeftThroughCarry(ref _c);
                            break;
                        case 0x12: // RL  D
                            RotateLeftThroughCarry(ref _d);
                            break;
                        case 0x13: // RL  E
                            RotateLeftThroughCarry(ref _e);
                            break;
                        case 0x14: // RL  H
                            RotateLeftThroughCarry(ref _h);
                            break;
                        case 0x15: // RL  L
                            RotateLeftThroughCarry(ref _l);
                            break;
                        case 0x16: // RL  (HL)
                            RotateLeftThroughCarry(_h, _l);
                            break;
                        case 0x17: // RL  A
                            RotateLeftThroughCarry(ref _a);
                            break;
                        case 0x18: // RR  B
                            RotateRightThroughCarry(ref _b);
                            break;
                        case 0x19: // RR  C
                            RotateRightThroughCarry(ref _c);
                            break;
                        case 0x1A: // RR  D
                            RotateRightThroughCarry(ref _d);
                            break;
                        case 0x1B: // RR  E
                            RotateRightThroughCarry(ref _e);
                            break;
                        case 0x1C: // RR  H
                            RotateRightThroughCarry(ref _h);
                            break;
                        case 0x1D: // RR  L
                            RotateRightThroughCarry(ref _l);
                            break;
                        case 0x1E: // RR  (HL)
                            RotateRightThroughCarry(_h, _l);
                            break;
                        case 0x1F: // RR  A
                            RotateRightThroughCarry(ref _a);
                            break;
                        case 0x20: // SLA B
                            ShiftLeft(ref _b);
                            break;
                        case 0x21: // SLA C
                            ShiftLeft(ref _c);
                            break;
                        case 0x22: // SLA D
                            ShiftLeft(ref _d);
                            break;
                        case 0x23: // SLA E
                            ShiftLeft(ref _e);
                            break;
                        case 0x24: // SLA H
                            ShiftLeft(ref _h);
                            break;
                        case 0x25: // SLA L
                            ShiftLeft(ref _l);
                            break;
                        case 0x26: // SLA (HL)
                            ShiftLeft(_h, _l);
                            break;
                        case 0x27: // SLA A
                            ShiftLeft(ref _a);
                            break;
                        case 0x28: // SRA B
                            SignedShiftRight(ref _b);
                            break;
                        case 0x29: // SRA C
                            SignedShiftRight(ref _c);
                            break;
                        case 0x2A: // SRA D
                            SignedShiftRight(ref _d);
                            break;
                        case 0x2B: // SRA E
                            SignedShiftRight(ref _e);
                            break;
                        case 0x2C: // SRA H
                            SignedShiftRight(ref _h);
                            break;
                        case 0x2D: // SRA L
                            SignedShiftRight(ref _l);
                            break;
                        case 0x2E: // SRA (HL)
                            SignedShiftRight(_h, _l);
                            break;
                        case 0x2F: // SRA A
                            SignedShiftRight(ref _a);
                            break;
                        case 0x30: // SWAP B
                            Swap(ref _b);
                            break;
                        case 0x31: // SWAP C
                            Swap(ref _c);
                            break;
                        case 0x32: // SWAP D
                            Swap(ref _d);
                            break;
                        case 0x33: // SWAP E
                            Swap(ref _e);
                            break;
                        case 0x34: // SWAP H
                            Swap(ref _h);
                            break;
                        case 0x35: // SWAP L
                            Swap(ref _l);
                            break;
                        case 0x36: // SWAP (HL)
                            Swap(_h, _l);
                            break;
                        case 0x37: // SWAP A
                            Swap(ref _a);
                            break;
                        case 0x38: // SRL B
                            UnsignedShiftRight(ref _b);
                            break;
                        case 0x39: // SRL C
                            UnsignedShiftRight(ref _c);
                            break;
                        case 0x3A: // SRL D
                            UnsignedShiftRight(ref _d);
                            break;
                        case 0x3B: // SRL E
                            UnsignedShiftRight(ref _e);
                            break;
                        case 0x3C: // SRL H
                            UnsignedShiftRight(ref _h);
                            break;
                        case 0x3D: // SRL L
                            UnsignedShiftRight(ref _l);
                            break;
                        case 0x3E: // SRL (HL)
                            UnsignedShiftRight(_h, _l);
                            break;
                        case 0x3F: // SRL A
                            UnsignedShiftRight(ref _a);
                            break;
                        case 0x40: // BIT 0,B
                            TestBit(0, _b);
                            break;
                        case 0x41: // BIT 0,C
                            TestBit(0, _c);
                            break;
                        case 0x42: // BIT 0,D
                            TestBit(0, _d);
                            break;
                        case 0x43: // BIT 0,E
                            TestBit(0, _e);
                            break;
                        case 0x44: // BIT 0,H
                            TestBit(0, _h);
                            break;
                        case 0x45: // BIT 0,L
                            TestBit(0, _l);
                            break;
                        case 0x46: // BIT 0,(HL)
                            TestBit(0, _h, _l);
                            break;
                        case 0x47: // BIT 0,A
                            TestBit(0, _a);
                            break;
                        case 0x48: // BIT 1,B
                            TestBit(1, _b);
                            break;
                        case 0x49: // BIT 1,C
                            TestBit(1, _c);
                            break;
                        case 0x4A: // BIT 1,D
                            TestBit(1, _d);
                            break;
                        case 0x4B: // BIT 1,E
                            TestBit(1, _e);
                            break;
                        case 0x4C: // BIT 1,H
                            TestBit(1, _h);
                            break;
                        case 0x4D: // BIT 1,L
                            TestBit(1, _l);
                            break;
                        case 0x4E: // BIT 1,(HL)
                            TestBit(1, _h, _l);
                            break;
                        case 0x4F: // BIT 1,A
                            TestBit(1, _a);
                            break;
                        case 0x50: // BIT 2,B
                            TestBit(2, _b);
                            break;
                        case 0x51: // BIT 2,C
                            TestBit(2, _c);
                            break;
                        case 0x52: // BIT 2,D
                            TestBit(2, _d);
                            break;
                        case 0x53: // BIT 2,E
                            TestBit(2, _e);
                            break;
                        case 0x54: // BIT 2,H
                            TestBit(2, _h);
                            break;
                        case 0x55: // BIT 2,L
                            TestBit(2, _l);
                            break;
                        case 0x56: // BIT 2,(HL)
                            TestBit(2, _h, _l);
                            break;
                        case 0x57: // BIT 2,A
                            TestBit(2, _a);
                            break;
                        case 0x58: // BIT 3,B
                            TestBit(3, _b);
                            break;
                        case 0x59: // BIT 3,C
                            TestBit(3, _c);
                            break;
                        case 0x5A: // BIT 3,D
                            TestBit(3, _d);
                            break;
                        case 0x5B: // BIT 3,E
                            TestBit(3, _e);
                            break;
                        case 0x5C: // BIT 3,H
                            TestBit(3, _h);
                            break;
                        case 0x5D: // BIT 3,L
                            TestBit(3, _l);
                            break;
                        case 0x5E: // BIT 3,(HL)
                            TestBit(3, _h, _l);
                            break;
                        case 0x5F: // BIT 3,A
                            TestBit(3, _a);
                            break;
                        case 0x60: // BIT 4,B
                            TestBit(4, _b);
                            break;
                        case 0x61: // BIT 4,C
                            TestBit(4, _c);
                            break;
                        case 0x62: // BIT 4,D
                            TestBit(4, _d);
                            break;
                        case 0x63: // BIT 4,E
                            TestBit(4, _e);
                            break;
                        case 0x64: // BIT 4,H
                            TestBit(4, _h);
                            break;
                        case 0x65: // BIT 4,L
                            TestBit(4, _l);
                            break;
                        case 0x66: // BIT 4,(HL)
                            TestBit(4, _h, _l);
                            break;
                        case 0x67: // BIT 4,A
                            TestBit(4, _a);
                            break;
                        case 0x68: // BIT 5,B
                            TestBit(5, _b);
                            break;
                        case 0x69: // BIT 5,C
                            TestBit(5, _c);
                            break;
                        case 0x6A: // BIT 5,D
                            TestBit(5, _d);
                            break;
                        case 0x6B: // BIT 5,E
                            TestBit(5, _e);
                            break;
                        case 0x6C: // BIT 5,H
                            TestBit(5, _h);
                            break;
                        case 0x6D: // BIT 5,L
                            TestBit(5, _l);
                            break;
                        case 0x6E: // BIT 5,(HL)
                            TestBit(5, _h, _l);
                            break;
                        case 0x6F: // BIT 5,A
                            TestBit(5, _a);
                            break;
                        case 0x70: // BIT 6,B
                            TestBit(6, _b);
                            break;
                        case 0x71: // BIT 6,C
                            TestBit(6, _c);
                            break;
                        case 0x72: // BIT 6,D
                            TestBit(6, _d);
                            break;
                        case 0x73: // BIT 6,E
                            TestBit(6, _e);
                            break;
                        case 0x74: // BIT 6,H
                            TestBit(6, _h);
                            break;
                        case 0x75: // BIT 6,L
                            TestBit(6, _l);
                            break;
                        case 0x76: // BIT 6,(HL)
                            TestBit(6, _h, _l);
                            break;
                        case 0x77: // BIT 6,A
                            TestBit(6, _a);
                            break;
                        case 0x78: // BIT 7,B
                            TestBit(7, _b);
                            break;
                        case 0x79: // BIT 7,C
                            TestBit(7, _c);
                            break;
                        case 0x7A: // BIT 7,D
                            TestBit(7, _d);
                            break;
                        case 0x7B: // BIT 7,E
                            TestBit(7, _e);
                            break;
                        case 0x7C: // BIT 7,H
                            TestBit(7, _h);
                            break;
                        case 0x7D: // BIT 7,L
                            TestBit(7, _l);
                            break;
                        case 0x7E: // BIT 7,(HL)
                            TestBit(7, _h, _l);
                            break;
                        case 0x7F: // BIT 7,A
                            TestBit(7, _a);
                            break;
                        case 0x80: // RES 0,B
                            ResetBit(0, ref _b);
                            break;
                        case 0x81: // RES 0,C
                            ResetBit(0, ref _c);
                            break;
                        case 0x82: // RES 0,D
                            ResetBit(0, ref _d);
                            break;
                        case 0x83: // RES 0,E
                            ResetBit(0, ref _e);
                            break;
                        case 0x84: // RES 0,H
                            ResetBit(0, ref _h);
                            break;
                        case 0x85: // RES 0,L
                            ResetBit(0, ref _l);
                            break;
                        case 0x86: // RES 0,(HL)
                            ResetBit(0, _h, _l);
                            break;
                        case 0x87: // RES 0,A
                            ResetBit(0, ref _a);
                            break;
                        case 0x88: // RES 1,B
                            ResetBit(1, ref _b);
                            break;
                        case 0x89: // RES 1,C
                            ResetBit(1, ref _c);
                            break;
                        case 0x8A: // RES 1,D
                            ResetBit(1, ref _d);
                            break;
                        case 0x8B: // RES 1,E
                            ResetBit(1, ref _e);
                            break;
                        case 0x8C: // RES 1,H
                            ResetBit(1, ref _h);
                            break;
                        case 0x8D: // RES 1,L
                            ResetBit(1, ref _l);
                            break;
                        case 0x8E: // RES 1,(HL)
                            ResetBit(1, _h, _l);
                            break;
                        case 0x8F: // RES 1,A
                            ResetBit(1, ref _a);
                            break;
                        case 0x90: // RES 2,B
                            ResetBit(2, ref _b);
                            break;
                        case 0x91: // RES 2,C
                            ResetBit(2, ref _c);
                            break;
                        case 0x92: // RES 2,D
                            ResetBit(2, ref _d);
                            break;
                        case 0x93: // RES 2,E
                            ResetBit(2, ref _e);
                            break;
                        case 0x94: // RES 2,H
                            ResetBit(2, ref _h);
                            break;
                        case 0x95: // RES 2,L
                            ResetBit(2, ref _l);
                            break;
                        case 0x96: // RES 2,(HL)
                            ResetBit(2, _h, _l);
                            break;
                        case 0x97: // RES 2,A
                            ResetBit(2, ref _a);
                            break;
                        case 0x98: // RES 3,B
                            ResetBit(3, ref _b);
                            break;
                        case 0x99: // RES 3,C
                            ResetBit(3, ref _c);
                            break;
                        case 0x9A: // RES 3,D
                            ResetBit(3, ref _d);
                            break;
                        case 0x9B: // RES 3,E
                            ResetBit(3, ref _e);
                            break;
                        case 0x9C: // RES 3,H
                            ResetBit(3, ref _h);
                            break;
                        case 0x9D: // RES 3,L
                            ResetBit(3, ref _l);
                            break;
                        case 0x9E: // RES 3,(HL)
                            ResetBit(3, _h, _l);
                            break;
                        case 0x9F: // RES 3,A
                            ResetBit(3, ref _a);
                            break;
                        case 0xA0: // RES 4,B
                            ResetBit(4, ref _b);
                            break;
                        case 0xA1: // RES 4,C
                            ResetBit(4, ref _c);
                            break;
                        case 0xA2: // RES 4,D
                            ResetBit(4, ref _d);
                            break;
                        case 0xA3: // RES 4,E
                            ResetBit(4, ref _e);
                            break;
                        case 0xA4: // RES 4,H
                            ResetBit(4, ref _h);
                            break;
                        case 0xA5: // RES 4,L
                            ResetBit(4, ref _l);
                            break;
                        case 0xA6: // RES 4,(HL)
                            ResetBit(4, _h, _l);
                            break;
                        case 0xA7: // RES 4,A
                            ResetBit(4, ref _a);
                            break;
                        case 0xA8: // RES 5,B
                            ResetBit(5, ref _b);
                            break;
                        case 0xA9: // RES 5,C
                            ResetBit(5, ref _c);
                            break;
                        case 0xAA: // RES 5,D
                            ResetBit(5, ref _d);
                            break;
                        case 0xAB: // RES 5,E
                            ResetBit(5, ref _e);
                            break;
                        case 0xAC: // RES 5,H
                            ResetBit(5, ref _h);
                            break;
                        case 0xAD: // RES 5,L
                            ResetBit(5, ref _l);
                            break;
                        case 0xAE: // RES 5,(HL)
                            ResetBit(5, _h, _l);
                            break;
                        case 0xAF: // RES 5,A
                            ResetBit(5, ref _a);
                            break;
                        case 0xB0: // RES 6,B
                            ResetBit(6, ref _b);
                            break;
                        case 0xB1: // RES 6,C
                            ResetBit(6, ref _c);
                            break;
                        case 0xB2: // RES 6,D
                            ResetBit(6, ref _d);
                            break;
                        case 0xB3: // RES 6,E
                            ResetBit(6, ref _e);
                            break;
                        case 0xB4: // RES 6,H
                            ResetBit(6, ref _h);
                            break;
                        case 0xB5: // RES 6,L
                            ResetBit(6, ref _l);
                            break;
                        case 0xB6: // RES 6,(HL)
                            ResetBit(6, _h, _l);
                            break;
                        case 0xB7: // RES 6,A
                            ResetBit(6, ref _a);
                            break;
                        case 0xB8: // RES 7,B
                            ResetBit(7, ref _b);
                            break;
                        case 0xB9: // RES 7,C
                            ResetBit(7, ref _c);
                            break;
                        case 0xBA: // RES 7,D
                            ResetBit(7, ref _d);
                            break;
                        case 0xBB: // RES 7,E
                            ResetBit(7, ref _e);
                            break;
                        case 0xBC: // RES 7,H
                            ResetBit(7, ref _h);
                            break;
                        case 0xBD: // RES 7,L
                            ResetBit(7, ref _l);
                            break;
                        case 0xBE: // RES 7,(HL)
                            ResetBit(7, _h, _l);
                            break;
                        case 0xBF: // RES 7,A
                            ResetBit(7, ref _a);
                            break;
                        case 0xC0: // SET 0,B
                            SetBit(0, ref _b);
                            break;
                        case 0xC1: // SET 0,C
                            SetBit(0, ref _c);
                            break;
                        case 0xC2: // SET 0,D
                            SetBit(0, ref _d);
                            break;
                        case 0xC3: // SET 0,E
                            SetBit(0, ref _e);
                            break;
                        case 0xC4: // SET 0,H
                            SetBit(0, ref _h);
                            break;
                        case 0xC5: // SET 0,L
                            SetBit(0, ref _l);
                            break;
                        case 0xC6: // SET 0,(HL)
                            SetBit(0, _h, _l);
                            break;
                        case 0xC7: // SET 0,A
                            SetBit(0, ref _a);
                            break;
                        case 0xC8: // SET 1,B
                            SetBit(1, ref _b);
                            break;
                        case 0xC9: // SET 1,C
                            SetBit(1, ref _c);
                            break;
                        case 0xCA: // SET 1,D
                            SetBit(1, ref _d);
                            break;
                        case 0xCB: // SET 1,E
                            SetBit(1, ref _e);
                            break;
                        case 0xCC: // SET 1,H
                            SetBit(1, ref _h);
                            break;
                        case 0xCD: // SET 1,L
                            SetBit(1, ref _l);
                            break;
                        case 0xCE: // SET 1,(HL)
                            SetBit(1, _h, _l);
                            break;
                        case 0xCF: // SET 1,A
                            SetBit(1, ref _a);
                            break;
                        case 0xD0: // SET 2,B
                            SetBit(2, ref _b);
                            break;
                        case 0xD1: // SET 2,C
                            SetBit(2, ref _c);
                            break;
                        case 0xD2: // SET 2,D
                            SetBit(2, ref _d);
                            break;
                        case 0xD3: // SET 2,E
                            SetBit(2, ref _e);
                            break;
                        case 0xD4: // SET 2,H
                            SetBit(2, ref _h);
                            break;
                        case 0xD5: // SET 2,L
                            SetBit(2, ref _l);
                            break;
                        case 0xD6: // SET 2,(HL)
                            SetBit(2, _h, _l);
                            break;
                        case 0xD7: // SET 2,A
                            SetBit(2, ref _a);
                            break;
                        case 0xD8: // SET 3,B
                            SetBit(3, ref _b);
                            break;
                        case 0xD9: // SET 3,C
                            SetBit(3, ref _c);
                            break;
                        case 0xDA: // SET 3,D
                            SetBit(3, ref _d);
                            break;
                        case 0xDB: // SET 3,E
                            SetBit(3, ref _e);
                            break;
                        case 0xDC: // SET 3,H
                            SetBit(3, ref _h);
                            break;
                        case 0xDD: // SET 3,L
                            SetBit(3, ref _l);
                            break;
                        case 0xDE: // SET 3,(HL)
                            SetBit(3, _h, _l);
                            break;
                        case 0xDF: // SET 3,A
                            SetBit(3, ref _a);
                            break;
                        case 0xE0: // SET 4,B
                            SetBit(4, ref _b);
                            break;
                        case 0xE1: // SET 4,C
                            SetBit(4, ref _c);
                            break;
                        case 0xE2: // SET 4,D
                            SetBit(4, ref _d);
                            break;
                        case 0xE3: // SET 4,E
                            SetBit(4, ref _e);
                            break;
                        case 0xE4: // SET 4,H
                            SetBit(4, ref _h);
                            break;
                        case 0xE5: // SET 4,L
                            SetBit(4, ref _l);
                            break;
                        case 0xE6: // SET 4,(HL)
                            SetBit(4, _h, _l);
                            break;
                        case 0xE7: // SET 4,A
                            SetBit(4, ref _a);
                            break;
                        case 0xE8: // SET 5,B
                            SetBit(5, ref _b);
                            break;
                        case 0xE9: // SET 5,C
                            SetBit(5, ref _c);
                            break;
                        case 0xEA: // SET 5,D
                            SetBit(5, ref _d);
                            break;
                        case 0xEB: // SET 5,E
                            SetBit(5, ref _e);
                            break;
                        case 0xEC: // SET 5,H
                            SetBit(5, ref _h);
                            break;
                        case 0xED: // SET 5,L
                            SetBit(5, ref _l);
                            break;
                        case 0xEE: // SET 5,(HL)
                            SetBit(5, _h, _l);
                            break;
                        case 0xEF: // SET 5,A
                            SetBit(5, ref _a);
                            break;
                        case 0xF0: // SET 6,B
                            SetBit(6, ref _b);
                            break;
                        case 0xF1: // SET 6,C
                            SetBit(6, ref _c);
                            break;
                        case 0xF2: // SET 6,D
                            SetBit(6, ref _d);
                            break;
                        case 0xF3: // SET 6,E
                            SetBit(6, ref _e);
                            break;
                        case 0xF4: // SET 6,H
                            SetBit(6, ref _h);
                            break;
                        case 0xF5: // SET 6,L
                            SetBit(6, ref _l);
                            break;
                        case 0xF6: // SET 6,(HL)
                            SetBit(6, _h, _l);
                            break;
                        case 0xF7: // SET 6,A
                            SetBit(6, ref _a);
                            break;
                        case 0xF8: // SET 7,B
                            SetBit(7, ref _b);
                            break;
                        case 0xF9: // SET 7,C
                            SetBit(7, ref _c);
                            break;
                        case 0xFA: // SET 7,D
                            SetBit(7, ref _d);
                            break;
                        case 0xFB: // SET 7,E
                            SetBit(7, ref _e);
                            break;
                        case 0xFC: // SET 7,H
                            SetBit(7, ref _h);
                            break;
                        case 0xFD: // SET 7,L
                            SetBit(7, ref _l);
                            break;
                        case 0xFE: // SET 7,(HL)
                            SetBit(7, _h, _l);
                            break;
                        case 0xFF: // SET 7,A
                            SetBit(7, ref _a);
                            break;
                    }
                    break;
                case 0xCC: // CALL Z,NN
                    CallIfZero();
                    break;
                case 0xCD: // CALL NN
                    Call();
                    break;
                case 0xCE: // ADC A,N
                    AddImmediateWithCarry();
                    break;
                case 0xCF: // RST 8H
                    Restart(0x08);
                    break;
                case 0xD0: // RET NC
                    ReturnIfNotCarry();
                    break;
                case 0xD1: // POP DE
                    Pop(ref _d, ref _e);
                    break;
                case 0xD2: // JP NC,N
                    JumpIfNotCarry();
                    break;
                case 0xD4: // CALL NC,NN
                    CallIfNotCarry();
                    break;
                case 0xD5: // PUSH DE
                    Push(_d, _e);
                    break;
                case 0xD6: // SUB N
                    SubImmediate();
                    break;
                case 0xD7: // RST 10H
                    Restart(0x10);
                    break;
                case 0xD8: // RET C
                    ReturnIfCarry();
                    break;
                case 0xD9: // RETI
                    ReturnFromInterrupt();
                    break;
                case 0xDA: // JP C,N
                    JumpIfCarry();
                    break;
                case 0xDC: // CALL C,NN
                    CallIfCarry();
                    break;
                case 0xDE: // SBC A,N
                    SubImmediateWithBorrow();
                    break;
                case 0xDF: // RST 18H
                    Restart(0x18);
                    break;
                case 0xE0: // LD (FF00+byte),A
                    SaveAWithOffset();
                    break;
                case 0xE1: // POP HL
                    Pop(ref _h, ref _l);
                    break;
                case 0xE2: // LD (FF00+C),A
                    SaveAtoC();
                    break;
                case 0xE5: // PUSH HL
                    Push(_h, _l);
                    break;
                case 0xE6: // AND N
                    AndImmediate();
                    break;
                case 0xE7: // RST 20H
                    Restart(0x20);
                    break;
                case 0xE8: // ADD SP,offset
                    OffsetStackPointer();
                    break;
                case 0xE9: // JP (HL)
                    Jump(_h, _l);
                    break;
                case 0xEA: // LD (word),A
                    SaveA();
                    break;
                case 0xEE: // XOR N
                    XorImmediate();
                    break;
                case 0xEF: // RST 28H
                    Restart(0x0028);
                    break;
                case 0xF0: // LD A, (FF00 + n)
                    LoadAFromImmediate();
                    break;
                case 0xF1: // POP AF
                    PopAf();
                    break;
                case 0xF2: // LD A, (FF00 + C)
                    LoadAFromC();
                    break;
                case 0xF3: // DI
                    InterruptsEnabled = false;
                    break;
                case 0xF5: // PUSH AF
                    PushAf();
                    break;
                case 0xF6: // OR N
                    OrImmediate();
                    break;
                case 0xF7: // RST 30H
                    Restart(0x0030);
                    break;
                case 0xF8: // LD HL, SP + dd
                    LoadHlWithSpPlusImmediate();
                    break;
                case 0xF9: // LD SP,HL
                    LoadSpWithHl();
                    break;
                case 0xFA: // LD A, (nn)
                    LoadFromImmediateAddress(ref _a);
                    break;
                case 0xFB: // EI
                    InterruptsEnabled = true;
                    break;
                case 0xFE: // CP N
                    CompareImmediate();
                    break;
                case 0xFF: // RST 38H
                    Restart(0x0038);
                    break;
                default:
                    throw new Exception(string.Format("Unknown instruction: {0:X} at PC={1:X}", opCode, _pc));
            }
        }

        private void Load(ref int a, int b)
        {
            a = b;
            Ticks += 4;
        }

        private void LoadSpWithHl()
        {
            _sp = (_h << 8) | _l;
            Ticks += 6;
        }

        private void LoadAFromImmediate()
        {
            _a = ReadByte(0xFF00 | ReadByte(_pc++));
            Ticks += 19;
        }

        private void LoadAFromC()
        {
            _a = ReadByte(0xFF00 | _c);
            Ticks += 19;
        }

        private void LoadHlWithSpPlusImmediate()
        {
            int offset = ReadByte(_pc++);
            if (offset > 0x7F)
            {
                offset -= 256;
            }
            offset += _sp;
            _h = 0xFF & (offset >> 8);
            _l = 0xFF & offset;
            Ticks += 20;
        }

        private void ReturnFromInterrupt()
        {
            InterruptsEnabled = true;
            Halted = false;
            Return();
            Ticks += 4;
        }

        private void NegateA()
        {
            _fc = _a == 0;
            _fh = (_a & 0x0F) != 0;
            _a = 0xFF & -_a;
            _fz = _a == 0;
            _fn = true;
            Ticks += 8;
        }

        private void NoOperation()
        {
            Ticks += 4;
        }

        private void OffsetStackPointer()
        {
            int value = ReadByte(_pc++);
            if (value > 0x7F)
            {
                value -= 256;
            }
            _sp += value;
            Ticks += 20;
        }

        private void SaveAtoC()
        {
            WriteByte(0xFF00 | _c, _a);
            Ticks += 19;
        }

        private void SaveA()
        {
            WriteByte(ReadWord(_pc), _a);
            _pc += 2;
            Ticks += 13;
        }

        private void SaveAWithOffset()
        {
            WriteByte(0xFF00 | ReadByte(_pc++), _a);
            Ticks += 19;
        }

        private void Swap(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            Swap(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void Swap(ref int r)
        {
            r = 0xFF & ((r << 4) | (r >> 4));
            Ticks += 8;
        }

        private void SetBit(int bit, int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            SetBit(bit, ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void SetBit(int bit, ref int a)
        {
            a |= (1 << bit);
            Ticks += 8;
        }

        private void ResetBit(int bit, int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            ResetBit(bit, ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void ResetBit(int bit, ref int a)
        {
            switch (bit)
            {
                case 0: // 1111 1110
                    a &= 0xFE;
                    break;
                case 1: // 1111 1101
                    a &= 0xFD;
                    break;
                case 2: // 1111 1011
                    a &= 0xFB;
                    break;
                case 3: // 1111 0111
                    a &= 0xF7;
                    break;
                case 4: // 1110 1111
                    a &= 0xEF;
                    break;
                case 5: // 1101 1111
                    a &= 0xDF;
                    break;
                case 6: // 1011 1111
                    a &= 0xBF;
                    break;
                case 7: // 0111 1111
                    a &= 0x7F;
                    break;
            }
            Ticks += 8;
        }

        private void Halt()
        {
            if (InterruptsEnabled)
            {
                Halted = true;
            }
            else
            {
                _stopCounting = true;
            }
            Ticks += 4;
        }

        private void TestBit(int bit, int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            TestBit(bit, value);
            WriteByte(address, value);
            Ticks += 4;
        }

        private void TestBit(int bit, int a)
        {
            _fz = (a & (1 << bit)) == 0;
            _fn = false;
            _fh = true;
            Ticks += 8;
        }

        private void CallIfCarry()
        {
            if (_fc)
            {
                Call();
            }
            else
            {
                _pc += 2;
                Ticks++;
            }
        }

        private void CallIfNotCarry()
        {
            if (_fc)
            {
                _pc += 2;
                Ticks++;
            }
            else
            {
                Call();
            }
        }

        private void CallIfZero()
        {
            if (_fz)
            {
                Call();
            }
            else
            {
                _pc += 2;
                Ticks++;
            }
        }

        private void CallIfNotZero()
        {
            if (_fz)
            {
                _pc += 2;
                Ticks++;
            }
            else
            {
                Call();
            }
        }

        private void Interrupt(int address)
        {
            InterruptsEnabled = false;
            Halted = false;
            Push(_pc);
            _pc = address;
        }

        private void Restart(int address)
        {
            Push(_pc);
            _pc = address;
        }

        private void Call()
        {
            Push(0xFFFF & (_pc + 2));
            _pc = ReadWord(_pc);
            Ticks += 17;
        }

        private void JumpIfCarry()
        {
            if (_fc)
            {
                Jump();
            }
            else
            {
                _pc += 2;
                Ticks++;
            }
        }

        private void JumpIfNotCarry()
        {
            if (_fc)
            {
                _pc += 2;
                Ticks++;
            }
            else
            {
                Jump();
            }
        }

        private void JumpIfZero()
        {
            if (_fz)
            {
                Jump();
            }
            else
            {
                _pc += 2;
                Ticks++;
            }
        }

        private void JumpIfNotZero()
        {
            if (_fz)
            {
                _pc += 2;
                Ticks++;
            }
            else
            {
                Jump();
            }
        }

        private void Jump(int ah, int al)
        {
            _pc = (ah << 8) | al;
            Ticks += 4;
        }

        private void Jump()
        {
            _pc = ReadWord(_pc);
            Ticks += 10;
        }

        private void ReturnIfCarry()
        {
            if (_fc)
            {
                Return();
                Ticks++;
            }
            else
            {
                Ticks += 5;
            }
        }

        private void ReturnIfNotCarry()
        {
            if (_fc)
            {
                Ticks += 5;
            }
            else
            {
                Return();
                Ticks++;
            }
        }

        private void ReturnIfZero()
        {
            if (_fz)
            {
                Return();
                Ticks++;
            }
            else
            {
                Ticks += 5;
            }
        }

        private void ReturnIfNotZero()
        {
            if (_fz)
            {
                Ticks += 5;
            }
            else
            {
                Return();
                Ticks++;
            }
        }

        private void Return()
        {
            Pop(ref _pc);
        }

        private void Pop(ref int a)
        {
            a = ReadWord(_sp);
            _sp += 2;
            Ticks += 10;
        }

        private void PopAf()
        {
            int f = 0;
            Pop(ref _a, ref f);
            _fz = (f & 0x80) == 0x80;
            _fc = (f & 0x40) == 0x40;
            _fh = (f & 0x20) == 0x20;
            _fn = (f & 0x10) == 0x10;
        }

        private void PushAf()
        {
            int f = 0;
            if (_fz)
            {
                f |= 0x80;
            }
            if (_fc)
            {
                f |= 0x40;
            }
            if (_fh)
            {
                f |= 0x20;
            }
            if (_fn)
            {
                f |= 0x10;
            }
            Push(_a, f);
        }

        private void Pop(ref int rh, ref int rl)
        {
            rl = ReadByte(_sp++);
            rh = ReadByte(_sp++);
            Ticks += 10;
        }

        private void Push(int rh, int rl)
        {
            WriteByte(--_sp, rh);
            WriteByte(--_sp, rl);
            Ticks += 11;
        }

        private void Push(int value)
        {
            _sp -= 2;
            WriteWord(_sp, value);
            Ticks += 11;
        }

        private void Or(int addressHigh, int addressLow)
        {
            Or(ReadByte((addressHigh << 8) | addressLow));
            Ticks += 3;
        }

        private void Or(int b)
        {
            _a = 0xFF & (_a | b);
            _fh = false;
            _fn = false;
            _fc = false;
            _fz = _a == 0;
            Ticks += 4;
        }

        private void OrImmediate()
        {
            Or(ReadByte(_pc++));
            Ticks += 3;
        }

        private void XorImmediate()
        {
            Xor(ReadByte(_pc++));
        }

        private void Xor(int addressHigh, int addressLow)
        {
            Xor(ReadByte((addressHigh << 8) | addressLow));
        }

        private void Xor(int b)
        {
            _a = 0xFF & (_a ^ b);
            _fh = false;
            _fn = false;
            _fc = false;
            _fz = _a == 0;
        }

        private void And(int addressHigh, int addressLow)
        {
            And(ReadByte((addressHigh << 8) | addressLow));
            Ticks += 3;
        }

        private void AndImmediate()
        {
            And(ReadByte(_pc++));
            Ticks += 3;
        }

        private void And(int b)
        {
            _a = 0xFF & (_a & b);
            _fh = true;
            _fn = false;
            _fc = false;
            _fz = _a == 0;
            Ticks += 4;
        }

        private void SetCarryFlag()
        {
            _fh = false;
            _fc = true;
            _fn = false;
            Ticks += 4;
        }

        private void ComplementCarryFlag()
        {
            _fh = _fc;
            _fc = !_fc;
            _fn = false;
            Ticks += 4;
        }

        private void LoadImmediateIntoMemory(int ah, int al)
        {
            WriteByte((ah << 8) | al, ReadByte(_pc++));
            Ticks += 10;
        }

        private void ComplementA()
        {
            _a ^= 0xFF;
            _fn = true;
            _fh = true;
            Ticks += 4;
        }

        private void DecimallyAdjustA()
        {
            int highNibble = _a >> 4;
            int lowNibble = _a & 0x0F;
            bool fc = true;
            if (_fn)
            {
                if (_fc)
                {
                    if (_fh)
                    {
                        _a += 0x9A;
                    }
                    else
                    {
                        _a += 0xA0;
                    }
                }
                else
                {
                    fc = false;
                    if (_fh)
                    {
                        _a += 0xFA;
                    }
                    else
                    {
                        _a += 0x00;
                    }
                }
            }
            else if (_fc)
            {
                if (_fh || lowNibble > 9)
                {
                    _a += 0x66;
                }
                else
                {
                    _a += 0x60;
                }
            }
            else if (_fh)
            {
                if (highNibble > 9)
                {
                    _a += 0x66;
                }
                else
                {
                    _a += 0x06;
                    fc = false;
                }
            }
            else if (lowNibble > 9)
            {
                if (highNibble < 9)
                {
                    fc = false;
                    _a += 0x06;
                }
                else
                {
                    _a += 0x66;
                }
            }
            else if (highNibble > 9)
            {
                _a += 0x60;
            }
            else
            {
                fc = false;
            }

            _a &= 0xFF;
            _fc = fc;
            _fz = _a == 0;
            Ticks += 4;
        }

        private void JumpRelativeIfNotCarry()
        {
            if (_fc)
            {
                _pc++;
                Ticks += 7;
            }
            else
            {
                JumpRelative();
            }
        }

        private void JumpRelativeIfCarry()
        {
            if (_fc)
            {
                JumpRelative();
            }
            else
            {
                _pc++;
                Ticks += 7;
            }
        }

        private void JumpRelativeIfNotZero()
        {
            if (_fz)
            {
                _pc++;
                Ticks += 7;
            }
            else
            {
                JumpRelative();
            }
        }

        private void JumpRelativeIfZero()
        {
            if (_fz)
            {
                JumpRelative();
            }
            else
            {
                _pc++;
                Ticks += 7;
            }
        }

        private void JumpRelative()
        {
            int relativeAddress = ReadByte(_pc++);
            if (relativeAddress > 0x7F)
            {
                relativeAddress -= 256;
            }
            _pc += relativeAddress;
            Ticks += 12;
        }

        private void Add(int addressHigh, int addressLow)
        {
            Add(ReadByte((addressHigh << 8) | addressLow));
            Ticks += 3;
        }

        private void AddImmediateWithCarry()
        {
            AddWithCarry(ReadByte(_pc++));
            Ticks += 3;
        }

        private void AddWithCarry(int addressHigh, int addressLow)
        {
            AddWithCarry(ReadByte((addressHigh << 8) | addressLow));
            Ticks += 3;
        }

        private void AddWithCarry(int b)
        {
            int carry = _fc ? 1 : 0;
            _fh = carry + (_a & 0x0F) + (b & 0x0F) > 0x0F;
            _a += b + carry;
            _fc = _a > 255;
            _a &= 0xFF;
            _fn = false;
            _fz = _a == 0;
            Ticks += 4;
        }

        private void SubWithBorrow(int ah, int al)
        {
            SubWithBorrow(ReadByte((ah << 8) | al));
            Ticks += 3;
        }

        private void SubImmediateWithBorrow()
        {
            SubWithBorrow(ReadByte(_pc++));
            Ticks += 3;
        }

        private void SubWithBorrow(int b)
        {
            if (_fc)
            {
                Sub(b + 1);
            }
            else
            {
                Sub(b);
            }
        }

        private void Sub(int ah, int al)
        {
            Sub(ReadByte((ah << 8) | al));
            Ticks += 3;
        }

        private void Compare(int ah, int al)
        {
            Compare(ReadByte((ah << 8) | al));
            Ticks += 3;
        }

        private void CompareImmediate()
        {
            Compare(ReadByte(_pc++));
            Ticks += 3;
        }

        private void Compare(int b)
        {
            _fh = (_a & 0x0F) < (b & 0x0F);
            _fc = b > _a;
            _fn = true;
            _fz = _a == b;
            Ticks += 4;
        }

        private void SubImmediate()
        {
            Sub(ReadByte(_pc++));
            Ticks += 3;
        }

        private void Sub(int b)
        {
            _fh = (_a & 0x0F) < (b & 0x0F);
            _fc = b > _a;
            _a -= b;
            _a &= 0xFF;
            _fn = true;
            _fz = _a == 0;
            Ticks += 4;
        }

        private void AddImmediate()
        {
            Add(ReadByte(_pc++));
            Ticks += 3;
        }

        private void Add(int b)
        {
            _fh = (_a & 0x0F) + (b & 0x0F) > 0x0F;
            _a += b;
            _fc = _a > 255;
            _a &= 0xFF;
            _fn = false;
            _fz = _a == 0;
            Ticks += 4;
        }

        private void AddSptoHl()
        {
            Add(ref _h, ref _l, _sp >> 8, _sp & 0xFF);
        }

        private void Add(ref int ah, ref int al, int bh, int bl)
        {
            al += bl;
            int carry = (al > 0xFF) ? 1 : 0;
            al &= 0xFF;
            _fh = carry + (ah & 0x0F) + (bh & 0x0F) > 0x0F;
            ah += bh + carry;
            _fc = ah > 0xFF;
            ah &= 0xFF;
            _fn = false;
            Ticks += 11;
        }

        private void ShiftLeft(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            ShiftLeft(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void ShiftLeft(ref int a)
        {
            _fc = a > 0x7F;
            a = 0xFF & (a << 1);
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void UnsignedShiftRight(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            UnsignedShiftRight(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void UnsignedShiftRight(ref int a)
        {
            _fc = (a & 0x01) == 1;
            a >>= 1;
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void SignedShiftRight(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            SignedShiftRight(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void SignedShiftRight(ref int a)
        {
            _fc = (a & 0x01) == 1;
            a = (a & 0x80) | (a >> 1);
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void RotateARight()
        {
            int lowBit = _a & 0x01;
            _fc = lowBit == 1;
            _a = (_a >> 1) | (lowBit << 7);
            _fn = false;
            _fh = false;
            Ticks += 4;
        }

        private void RotateARightThroughCarry()
        {
            int highBit = _fc ? 0x80 : 0x00;
            _fc = (_a & 0x01) == 0x01;
            _a = highBit | (_a >> 1);
            _fn = false;
            _fh = false;
            Ticks += 4;
        }

        private void RotateALeftThroughCarry()
        {
            int highBit = _fc ? 1 : 0;
            _fc = _a > 0x7F;
            _a = ((_a << 1) & 0xFF) | highBit;
            _fn = false;
            _fh = false;
            Ticks += 4;
        }

        private void RotateRight(ref int a)
        {
            int lowBit = a & 0x01;
            _fc = lowBit == 1;
            a = (a >> 1) | (lowBit << 7);
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void RotateRightThroughCarry(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            RotateRightThroughCarry(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void RotateRightThroughCarry(ref int a)
        {
            int lowBit = _fc ? 0x80 : 0;
            _fc = (a & 0x01) == 1;
            a = (a >> 1) | lowBit;
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void RotateLeftThroughCarry(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            RotateLeftThroughCarry(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void RotateLeftThroughCarry(ref int a)
        {
            int highBit = _fc ? 1 : 0;
            _fc = (a >> 7) == 1;
            a = ((a << 1) & 0xFF) | highBit;
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void RotateLeft(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            RotateLeft(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void RotateRight(int ah, int al)
        {
            int address = (ah << 8) | al;
            int value = ReadByte(address);
            RotateRight(ref value);
            WriteByte(address, value);
            Ticks += 7;
        }

        private void RotateLeft(ref int a)
        {
            int highBit = a >> 7;
            _fc = highBit == 1;
            a = ((a << 1) & 0xFF) | highBit;
            _fz = a == 0;
            _fn = false;
            _fh = false;
            Ticks += 8;
        }

        private void RotateALeft()
        {
            int highBit = _a >> 7;
            _fc = highBit == 1;
            _a = ((_a << 1) & 0xFF) | highBit;
            _fn = false;
            _fh = false;
            Ticks += 4;
        }

        private void LoadFromImmediateAddress(ref int r)
        {
            r = ReadByte(ReadWord(_pc));
            _pc += 2;
            Ticks += 13;
        }

        private void LoadImmediate(ref int r)
        {
            r = ReadByte(_pc++);
            Ticks += 7;
        }

        private void LoadImmediateWord(ref int r)
        {
            r = ReadWord(_pc);
            _pc += 2;
            Ticks += 10;
        }

        private void LoadImmediate(ref int rh, ref int rl)
        {
            rl = ReadByte(_pc++);
            rh = ReadByte(_pc++);
            Ticks += 10;
        }

        private void ReadByte(ref int r, int ah, int al)
        {
            r = ReadByte((ah << 8) | al);
            Ticks += 7;
        }

        private void WriteByte(int ah, int al, int value)
        {
            WriteByte((ah << 8) | al, value);
            Ticks += 7;
        }

        private void WriteWordToImmediateAddress(int value)
        {
            WriteWord(ReadWord(_pc), value);
            _pc += 2;
            Ticks += 20;
        }

        private void Decrement(ref int rh, ref int rl)
        {
            if (rl == 0)
            {
                rh = 0xFF & (rh - 1);
                rl = 0xFF;
            }
            else
            {
                rl--;
            }
            Ticks += 6;
        }

        private void IncrementWord(ref int r)
        {
            if (r == 0xFFFF)
            {
                r = 0;
            }
            else
            {
                r++;
            }
            Ticks += 6;
        }

        private void DecrementWord(ref int r)
        {
            if (r == 0)
            {
                r = 0xFFFF;
            }
            else
            {
                r--;
            }
            Ticks += 6;
        }

        private void DecrementMemory(int ah, int al)
        {
            int address = (ah << 8) | al;
            int r = ReadByte(address);
            Decrement(ref r);
            WriteByte(address, r);
            Ticks += 7;
        }

        private void IncrementMemory(int ah, int al)
        {
            int address = (ah << 8) | al;
            int r = ReadByte(address);
            Increment(ref r);
            WriteByte(address, r);
            Ticks += 7;
        }

        private void Increment(ref int rh, ref int rl)
        {
            if (rl == 255)
            {
                rh = 0xFF & (rh + 1);
                rl = 0;
            }
            else
            {
                rl++;
            }
            Ticks += 6;
        }

        private void Increment(ref int r)
        {
            _fh = (r & 0x0F) == 0x0F;
            r++;
            r &= 0xFF;
            _fz = r == 0;
            _fn = false;
            Ticks += 4;
        }

        private void Decrement(ref int r)
        {
            _fh = (r & 0x0F) == 0x00;
            r--;
            r &= 0xFF;
            _fz = r == 0;
            _fn = true;
            Ticks += 4;
        }

        public void PowerUp()
        {
            _a = 0x01;
            _b = 0x00;
            _c = 0x13;
            _d = 0x00;
            _e = 0xD8;
            _h = 0x01;
            _l = 0x4D;
            _fz = true;
            _fc = false;
            _fh = true;
            _fn = true;
            _sp = 0xFFFE;
            _pc = 0x0100;

            WriteByte(0xFF05, 0x00); // TIMA
            WriteByte(0xFF06, 0x00); // TMA
            WriteByte(0xFF07, 0x00); // TAC
            WriteByte(0xFF10, 0x80); // NR10
            WriteByte(0xFF11, 0xBF); // NR11
            WriteByte(0xFF12, 0xF3); // NR12
            WriteByte(0xFF14, 0xBF); // NR14
            WriteByte(0xFF16, 0x3F); // NR21
            WriteByte(0xFF17, 0x00); // NR22
            WriteByte(0xFF19, 0xBF); // NR24
            WriteByte(0xFF1A, 0x7F); // NR30
            WriteByte(0xFF1B, 0xFF); // NR31
            WriteByte(0xFF1C, 0x9F); // NR32
            WriteByte(0xFF1E, 0xBF); // NR33
            WriteByte(0xFF20, 0xFF); // NR41
            WriteByte(0xFF21, 0x00); // NR42
            WriteByte(0xFF22, 0x00); // NR43
            WriteByte(0xFF23, 0xBF); // NR30
            WriteByte(0xFF24, 0x77); // NR50
            WriteByte(0xFF25, 0xF3); // NR51
            WriteByte(0xFF26, 0xF1); // NR52
            WriteByte(0xFF40, 0x91); // LCDC
            WriteByte(0xFF42, 0x00); // SCY
            WriteByte(0xFF43, 0x00); // SCX
            WriteByte(0xFF45, 0x00); // LYC
            WriteByte(0xFF47, 0xFC); // BGP
            WriteByte(0xFF48, 0xFF); // OBP0
            WriteByte(0xFF49, 0xFF); // OBP1
            WriteByte(0xFF4A, 0x00); // WY
            WriteByte(0xFF4B, 0x00); // WX
            WriteByte(0xFFFF, 0x00); // IE
        }

        public void WriteWord(int address, int value)
        {
            WriteByte(address, value & 0xFF);
            WriteByte(address + 1, value >> 8);
        }

        public int ReadWord(int address)
        {
            int low = ReadByte(address);
            int high = ReadByte(address + 1);
            return (high << 8) | low;
        }

        public void WriteByte(int address, int value)
        {
            if (address >= 0xC000 && address <= 0xDFFF)
            {
                _workRam[address - 0xC000] = (byte)value;
            }
            else if (address >= 0xFE00 && address <= 0xFEFF)
            {
                Oam[address - 0xFE00] = (byte)value;
            }
            else if (address >= 0xFF80 && address <= 0xFFFE)
            {
                _highRam[0xFF & address] = (byte)value;
            }
            else if (address >= 0x8000 && address <= 0x9FFF)
            {
                int videoRamIndex = address - 0x8000;
                _videoRam[videoRamIndex] = (byte)value;
                if (address < 0x9000)
                {
                    _spriteTileInvalidated[videoRamIndex >> 4] = true;
                }
                if (address < 0x9800)
                {
                    _invalidateAllBackgroundTilesRequest = true;
                }
                else if (address >= 0x9C00)
                {
                    int tileIndex = address - 0x9C00;
                    _backgroundTileInvalidated[tileIndex >> 5, tileIndex & 0x1F] = true;
                }
                else
                {
                    int tileIndex = address - 0x9800;
                    _backgroundTileInvalidated[tileIndex >> 5, tileIndex & 0x1F] = true;
                }
            }
            else if (address <= 0x7FFF || (address >= 0xA000 && address <= 0xBFFF))
            {
                Cartridge.WriteByte(address, value);
            }
            else if (address >= 0xE000 && address <= 0xFDFF)
            {
                _workRam[address - 0xE000] = (byte)value;
            }
            else
            {
                switch (address)
                {
                    case 0xFF00: // key pad
                        KeyP14 = (value & 0x10) != 0x10;
                        KeyP15 = (value & 0x20) != 0x20;
                        break;
                    case 0xFF04: // Timer divider            
                        break;
                    case 0xFF05: // Timer counter
                        TimerCounter = value;
                        break;
                    case 0xFF06: // Timer modulo
                        TimerModulo = value;
                        break;
                    case 0xFF07:  // Time Control
                        TimerRunning = (value & 0x04) == 0x04;
                        TimerFrequency = (TimerFrequencyType)(0x03 & value);
                        break;
                    case 0xFF0F: // Interrupt Flag (an interrupt request)
                        KeyPressedInterruptRequested = (value & 0x10) == 0x10;
                        SerialIoTransferCompleteInterruptRequested = (value & 0x08) == 0x08;
                        TimerOverflowInterruptRequested = (value & 0x04) == 0x04;
                        LcdcInterruptRequested = (value & 0x02) == 0x02;
                        VBlankInterruptRequested = (value & 0x01) == 0x01;
                        break;
                    case 0xFF40:
                        { // LCDC control
                            bool _backgroundAndWindowTileDataSelect = BackgroundAndWindowTileDataSelect;
                            bool _backgroundTileMapDisplaySelect = BackgroundTileMapDisplaySelect;
                            bool _windowTileMapDisplaySelect = WindowTileMapDisplaySelect;

                            LcdControlOperationEnabled = (value & 0x80) == 0x80;
                            WindowTileMapDisplaySelect = (value & 0x40) == 0x40;
                            WindowDisplayed = (value & 0x20) == 0x20;
                            BackgroundAndWindowTileDataSelect = (value & 0x10) == 0x10;
                            BackgroundTileMapDisplaySelect = (value & 0x08) == 0x08;
                            LargeSprites = (value & 0x04) == 0x04;
                            SpritesDisplayed = (value & 0x02) == 0x02;
                            BackgroundDisplayed = (value & 0x01) == 0x01;

                            if (_backgroundAndWindowTileDataSelect != BackgroundAndWindowTileDataSelect
                                || _backgroundTileMapDisplaySelect != BackgroundTileMapDisplaySelect
                                || _windowTileMapDisplaySelect != WindowTileMapDisplaySelect)
                            {
                                _invalidateAllBackgroundTilesRequest = true;
                            }

                            break;
                        }
                    case 0xFF41: // LCDC Status
                        LcdcLycLyCoincidenceInterruptEnabled = (value & 0x40) == 0x40;
                        LcdcOamInterruptEnabled = (value & 0x20) == 0x20;
                        LcdcVBlankInterruptEnabled = (value & 0x10) == 0x10;
                        LcdcHBlankInterruptEnabled = (value & 0x08) == 0x08;
                        LcdcMode = (LcdcModeType)(value & 0x03);
                        break;
                    case 0xFF42: // Scroll Y;
                        ScrollY = value;
                        break;
                    case 0xFF43: // Scroll X;
                        ScrollX = value;
                        break;
                    case 0xFF44: // LY
                        Ly = value;
                        break;
                    case 0xFF45: // LY Compare
                        LyCompare = value;
                        break;
                    case 0xFF46: // Memory Transfer
                        value <<= 8;
                        for (int i = 0; i < 0x8C; i++)
                        {
                            WriteByte(0xFE00 | i, ReadByte(value | i));
                        }
                        break;
                    case 0xFF47: // Background palette
                        //Console.WriteLine("[0xFF47] = {0:X}", value);
                        for (int i = 0; i < 4; i++)
                        {
                            switch (value & 0x03)
                            {
                                case 0:
                                    BackgroundPalette[i] = White;
                                    break;
                                case 1:
                                    BackgroundPalette[i] = LightGray;
                                    break;
                                case 2:
                                    BackgroundPalette[i] = DarkGray;
                                    break;
                                case 3:
                                    BackgroundPalette[i] = Black;
                                    break;
                            }
                            value >>= 2;
                        }
                        _invalidateAllBackgroundTilesRequest = true;
                        break;
                    case 0xFF48: // Object palette 0
                        for (int i = 0; i < 4; i++)
                        {
                            switch (value & 0x03)
                            {
                                case 0:
                                    ObjectPalette0[i] = White;
                                    break;
                                case 1:
                                    ObjectPalette0[i] = LightGray;
                                    break;
                                case 2:
                                    ObjectPalette0[i] = DarkGray;
                                    break;
                                case 3:
                                    ObjectPalette0[i] = Black;
                                    break;
                            }
                            value >>= 2;
                        }
                        _invalidateAllSpriteTilesRequest = true;
                        break;
                    case 0xFF49: // Object palette 1
                        for (int i = 0; i < 4; i++)
                        {
                            switch (value & 0x03)
                            {
                                case 0:
                                    ObjectPalette1[i] = White;
                                    break;
                                case 1:
                                    ObjectPalette1[i] = LightGray;
                                    break;
                                case 2:
                                    ObjectPalette1[i] = DarkGray;
                                    break;
                                case 3:
                                    ObjectPalette1[i] = Black;
                                    break;
                            }
                            value >>= 2;
                        }
                        _invalidateAllSpriteTilesRequest = true;
                        break;
                    case 0xFF4A: // Window Y
                        WindowY = value;
                        break;
                    case 0xFF4B: // Window X
                        WindowX = value;
                        break;
                    case 0xFFFF: // Interrupt Enable
                        KeyPressedInterruptEnabled = (value & 0x10) == 0x10;
                        SerialIoTransferCompleteInterruptEnabled = (value & 0x08) == 0x08;
                        TimerOverflowInterruptEnabled = (value & 0x04) == 0x04;
                        LcdcInterruptEnabled = (value & 0x02) == 0x02;
                        VBlankInterruptEnabled = (value & 0x01) == 0x01;
                        break;
                }
            }
        }

        public int ReadByte(int address)
        {
            if (address <= 0x7FFF || (address >= 0xA000 && address <= 0xBFFF))
            {
                return Cartridge.ReadByte(address);
            }
            else if (address >= 0x8000 && address <= 0x9FFF)
            {
                return _videoRam[address - 0x8000];
            }
            else if (address >= 0xC000 && address <= 0xDFFF)
            {
                return _workRam[address - 0xC000];
            }
            else if (address >= 0xE000 && address <= 0xFDFF)
            {
                return _workRam[address - 0xE000];
            }
            else if (address >= 0xFE00 && address <= 0xFEFF)
            {
                return Oam[address - 0xFE00];
            }
            else if (address >= 0xFF80 && address <= 0xFFFE)
            {
                return _highRam[0xFF & address];
            }
            else
            {
                switch (address)
                {
                    case 0xFF00: // key pad
                        if (KeyP14)
                        {
                            int value = 0;
                            if (!DownKeyPressed)
                            {
                                value |= 0x08;
                            }
                            if (!UpKeyPressed)
                            {
                                value |= 0x04;
                            }
                            if (!LeftKeyPressed)
                            {
                                value |= 0x02;
                            }
                            if (!RightKeyPressed)
                            {
                                value |= 0x01;
                            }
                            return value;
                        }
                        else if (KeyP15)
                        {
                            int value = 0;
                            if (!StartButtonPressed)
                            {
                                value |= 0x08;
                            }
                            if (!SelectButtonPressed)
                            {
                                value |= 0x04;
                            }
                            if (!BButtonPressed)
                            {
                                value |= 0x02;
                            }
                            if (!AButtonPressed)
                            {
                                value |= 0x01;
                            }
                            return value;
                        }
                        break;
                    case 0xFF04: // Timer divider
                        return Ticks & 0xFF;
                    case 0xFF05: // Timer counter
                        return TimerCounter & 0xFF;
                    case 0xFF06: // Timer modulo
                        return TimerModulo & 0xFF;
                    case 0xFF07:
                        { // Time Control
                            int value = 0;
                            if (TimerRunning)
                            {
                                value |= 0x04;
                            }
                            value |= (int)TimerFrequency;
                            return value;
                        }
                    case 0xFF0F:
                        { // Interrupt Flag (an interrupt request)
                            int value = 0;
                            if (KeyPressedInterruptRequested)
                            {
                                value |= 0x10;
                            }
                            if (SerialIoTransferCompleteInterruptRequested)
                            {
                                value |= 0x08;
                            }
                            if (TimerOverflowInterruptRequested)
                            {
                                value |= 0x04;
                            }
                            if (LcdcInterruptRequested)
                            {
                                value |= 0x02;
                            }
                            if (VBlankInterruptRequested)
                            {
                                value |= 0x01;
                            }
                            return value;
                        }
                    case 0xFF40:
                        { // LCDC control
                            int value = 0;
                            if (LcdControlOperationEnabled)
                            {
                                value |= 0x80;
                            }
                            if (WindowTileMapDisplaySelect)
                            {
                                value |= 0x40;
                            }
                            if (WindowDisplayed)
                            {
                                value |= 0x20;
                            }
                            if (BackgroundAndWindowTileDataSelect)
                            {
                                value |= 0x10;
                            }
                            if (BackgroundTileMapDisplaySelect)
                            {
                                value |= 0x08;
                            }
                            if (LargeSprites)
                            {
                                value |= 0x04;
                            }
                            if (SpritesDisplayed)
                            {
                                value |= 0x02;
                            }
                            if (BackgroundDisplayed)
                            {
                                value |= 0x01;
                            }
                            return value;
                        }
                    case 0xFF41:
                        {// LCDC Status
                            int value = 0;
                            if (LcdcLycLyCoincidenceInterruptEnabled)
                            {
                                value |= 0x40;
                            }
                            if (LcdcOamInterruptEnabled)
                            {
                                value |= 0x20;
                            }
                            if (LcdcVBlankInterruptEnabled)
                            {
                                value |= 0x10;
                            }
                            if (LcdcHBlankInterruptEnabled)
                            {
                                value |= 0x08;
                            }
                            if (Ly == LyCompare)
                            {
                                value |= 0x04;
                            }
                            value |= (int)LcdcMode;
                            return value;
                        }
                    case 0xFF42: // Scroll Y
                        return ScrollY;
                    case 0xFF43: // Scroll X
                        return ScrollX;
                    case 0xFF44: // LY
                        return Ly;
                    case 0xFF45: // LY Compare
                        return LyCompare;
                    case 0xFF47:
                        { // Background palette
                            _invalidateAllBackgroundTilesRequest = true;
                            int value = 0;
                            for (int i = 3; i >= 0; i--)
                            {
                                value <<= 2;
                                switch (BackgroundPalette[i])
                                {
                                    case Black:
                                        value |= 3;
                                        break;
                                    case DarkGray:
                                        value |= 2;
                                        break;
                                    case LightGray:
                                        value |= 1;
                                        break;
                                    case White:
                                        break;
                                }
                            }
                            return value;
                        }
                    case 0xFF48:
                        { // Object palette 0
                            _invalidateAllSpriteTilesRequest = true;
                            int value = 0;
                            for (int i = 3; i >= 0; i--)
                            {
                                value <<= 2;
                                switch (ObjectPalette0[i])
                                {
                                    case Black:
                                        value |= 3;
                                        break;
                                    case DarkGray:
                                        value |= 2;
                                        break;
                                    case LightGray:
                                        value |= 1;
                                        break;
                                    case White:
                                        break;
                                }
                            }
                            return value;
                        }
                    case 0xFF49:
                        { // Object palette 1
                            _invalidateAllSpriteTilesRequest = true;
                            int value = 0;
                            for (int i = 3; i >= 0; i--)
                            {
                                value <<= 2;
                                switch (ObjectPalette1[i])
                                {
                                    case Black:
                                        value |= 3;
                                        break;
                                    case DarkGray:
                                        value |= 2;
                                        break;
                                    case LightGray:
                                        value |= 1;
                                        break;
                                    case White:
                                        break;
                                }
                            }
                            return value;
                        }
                    case 0xFF4A: // Window Y
                        return WindowY;
                    case 0xFF4B: // Window X
                        return WindowX;
                    case 0xFFFF:
                        { // Interrupt Enable
                            int value = 0;
                            if (KeyPressedInterruptEnabled)
                            {
                                value |= 0x10;
                            }
                            if (SerialIoTransferCompleteInterruptEnabled)
                            {
                                value |= 0x08;
                            }
                            if (TimerOverflowInterruptEnabled)
                            {
                                value |= 0x04;
                            }
                            if (LcdcInterruptEnabled)
                            {
                                value |= 0x02;
                            }
                            if (VBlankInterruptEnabled)
                            {
                                value |= 0x01;
                            }
                            return value;
                        }
                }
            }
            return 0;
        }

        public void KeyChanged(Button keyCode, bool pressed)
        {
            switch (keyCode)
            {
                case Button.B:
                    BButtonPressed = pressed;
                    break;
                case Button.A:
                    AButtonPressed = pressed;
                    break;
                case Button.Start:
                    StartButtonPressed = pressed;
                    break;
                case Button.Select:
                    SelectButtonPressed = pressed;
                    break;
                case Button.Up:
                    UpKeyPressed = pressed;
                    break;
                case Button.Down:
                    DownKeyPressed = pressed;
                    break;
                case Button.Left:
                    LeftKeyPressed = pressed;
                    break;
                case Button.Right:
                    RightKeyPressed = pressed;
                    break;
            }

            if (KeyPressedInterruptEnabled)
            {
                KeyPressedInterruptRequested = true;
            }
        }

        public override string ToString()
        {
            return string.Format("PC={8:X} A={0:X} B={1:X} C={2:X} D={3:X} E={4:X} H={5:X} L={6:X} halted={7} SP={9:X} FZ={10} FH={11} FC={12} FN={13} IV={14} IL={15} IK={16} IT={17} INT={18} scrollX={19} scrollY={20} ly={21} lyCompare={22} LHIE={23} LYIE={24} LOIE={25}",
                _a, _b, _c, _d, _e, _h, _l, Halted, _pc, _sp, _fz, _fh, _fc, _fn, VBlankInterruptEnabled, LcdcInterruptEnabled, KeyPressedInterruptEnabled,
                TimerOverflowInterruptEnabled, InterruptsEnabled, ScrollX, ScrollY, Ly, LyCompare,
                LcdcHBlankInterruptEnabled, LcdcLycLyCoincidenceInterruptEnabled, LcdcOamInterruptEnabled);
        }

        public void CheckForBadState()
        {
            if (_a > 0xFF || _a < 0 || _b > 0xFF || _b < 0 || _c > 0xFF || _c < 0 || _d > 0xFF || _d < 0 || _e > 0xFF || _e < 0 || _h > 0xFF || _h < 0 || _sp > 0xFFFF || _sp < 0 || _pc > 0xFFFF || _pc < 0)
            {
                throw new Exception(ToString());
            }
        }

        public void UpdateSpriteTiles()
        {
            for (int i = 0; i < 256; i++)
            {
                if (_spriteTileInvalidated[i] || _invalidateAllSpriteTilesRequest)
                {
                    _spriteTileInvalidated[i] = false;
                    int address = i << 4;
                    for (int y = 0; y < 8; y++)
                    {
                        int lowByte = _videoRam[address++];
                        int highByte = _videoRam[address++] << 1;
                        for (int x = 7; x >= 0; x--)
                        {
                            int paletteIndex = (0x02 & highByte) | (0x01 & lowByte);
                            lowByte >>= 1;
                            highByte >>= 1;
                            if (paletteIndex > 0)
                            {
                                SpriteTile[i, y, x, 0] = ObjectPalette0[paletteIndex];
                                SpriteTile[i, y, x, 1] = ObjectPalette1[paletteIndex];
                            }
                            else
                            {
                                SpriteTile[i, y, x, 0] = 0;
                                SpriteTile[i, y, x, 1] = 0;
                            }
                        }
                    }
                }
            }

            _invalidateAllSpriteTilesRequest = false;
        }

        public void UpdateWindow()
        {
            int tileMapAddress = WindowTileMapDisplaySelect ? 0x1C00 : 0x1800;

            if (BackgroundAndWindowTileDataSelect)
            {
                for (int i = 0; i < 18; i++)
                {
                    for (int j = 0; j < 21; j++)
                    {
                        if (_backgroundTileInvalidated[i, j] || _invalidateAllBackgroundTilesRequest)
                        {
                            int tileDataAddress = _videoRam[tileMapAddress + ((i << 5) | j)] << 4;
                            int y = i << 3;
                            int x = j << 3;
                            for (int k = 0; k < 8; k++)
                            {
                                int lowByte = _videoRam[tileDataAddress++];
                                int highByte = _videoRam[tileDataAddress++] << 1;
                                for (int b = 7; b >= 0; b--)
                                {
                                    WindowBuffer[y + k, x + b] = BackgroundPalette[(0x02 & highByte) | (0x01 & lowByte)];
                                    lowByte >>= 1;
                                    highByte >>= 1;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    for (int j = 0; j < 21; j++)
                    {
                        if (_backgroundTileInvalidated[i, j] || _invalidateAllBackgroundTilesRequest)
                        {
                            int tileDataAddress = _videoRam[tileMapAddress + ((i << 5) | j)];
                            if (tileDataAddress > 127)
                            {
                                tileDataAddress -= 256;
                            }
                            tileDataAddress = 0x1000 + (tileDataAddress << 4);
                            int y = i << 3;
                            int x = j << 3;
                            for (int k = 0; k < 8; k++)
                            {
                                int lowByte = _videoRam[tileDataAddress++];
                                int highByte = _videoRam[tileDataAddress++] << 1;
                                for (int b = 7; b >= 0; b--)
                                {
                                    WindowBuffer[y + k, x + b] = BackgroundPalette[(0x02 & highByte) | (0x01 & lowByte)];
                                    lowByte >>= 1;
                                    highByte >>= 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UpdateBackground()
        {

            int tileMapAddress = BackgroundTileMapDisplaySelect ? 0x1C00 : 0x1800;

            if (BackgroundAndWindowTileDataSelect)
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++, tileMapAddress++)
                    {
                        if (_backgroundTileInvalidated[i, j] || _invalidateAllBackgroundTilesRequest)
                        {
                            _backgroundTileInvalidated[i, j] = false;
                            int tileDataAddress = _videoRam[tileMapAddress] << 4;
                            int y = i << 3;
                            int x = j << 3;
                            for (int k = 0; k < 8; k++)
                            {
                                int lowByte = _videoRam[tileDataAddress++];
                                int highByte = _videoRam[tileDataAddress++] << 1;
                                for (int b = 7; b >= 0; b--)
                                {
                                    BackgroundBuffer[y + k, x + b] = BackgroundPalette[(0x02 & highByte) | (0x01 & lowByte)];
                                    lowByte >>= 1;
                                    highByte >>= 1;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++, tileMapAddress++)
                    {
                        if (_backgroundTileInvalidated[i, j] || _invalidateAllBackgroundTilesRequest)
                        {
                            _backgroundTileInvalidated[i, j] = false;
                            int tileDataAddress = _videoRam[tileMapAddress];
                            if (tileDataAddress > 127)
                            {
                                tileDataAddress -= 256;
                            }
                            tileDataAddress = 0x1000 + (tileDataAddress << 4);
                            int y = i << 3;
                            int x = j << 3;
                            for (int k = 0; k < 8; k++)
                            {
                                int lowByte = _videoRam[tileDataAddress++];
                                int highByte = _videoRam[tileDataAddress++] << 1;
                                for (int b = 7; b >= 0; b--)
                                {
                                    BackgroundBuffer[y + k, x + b] = BackgroundPalette[(0x02 & highByte) | (0x01 & lowByte)];
                                    lowByte >>= 1;
                                    highByte >>= 1;
                                }
                            }
                        }
                    }
                }
            }

            _invalidateAllBackgroundTilesRequest = false;
        }
    }
}
