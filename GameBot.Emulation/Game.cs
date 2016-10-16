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
using System.Text;

namespace GameBot.Emulation
{
    public class Game
    {
        public string Title;
        public bool GameBoyColorGame;
        public int LicenseCode;
        public bool GameBoy;
        public RomType RomType;
        public int RomSize;
        public int RomBanks;
        public int RamSize;
        public int RamBanks;
        public bool Japanese;
        public int OldLicenseCode;
        public int MaskRomVersion;
        public int Checksum;
        public int ActualChecksum;
        public int HeaderChecksum;
        public int ActualHeaderChecksum;
        public bool NoVerticalBlankInterruptHandler;
        public bool NoLcdcStatusInterruptHandler;
        public bool NoTimerOverflowInterruptHandler;
        public bool NoSerialTransferCompletionInterruptHandler;
        public bool NoHighToLowOfP10ToP13InterruptHandler;
        public ICartridge Cartridge;

        public Game(byte[] fileData)
        {
            Title = ExtractGameTitle(fileData);
            GameBoyColorGame = fileData[0x0143] == 0x80;
            LicenseCode = (((int)fileData[0x0144]) << 4) | fileData[0x0145];
            GameBoy = fileData[0x0146] == 0x00;
            RomType = (RomType)fileData[0x0147];

            switch (fileData[0x0148])
            {
                case 0x00:
                    RomSize = 32 * 1024;
                    RomBanks = 2;
                    break;
                case 0x01:
                    RomSize = 64 * 1024;
                    RomBanks = 4;
                    break;
                case 0x02:
                    RomSize = 128 * 1024;
                    RomBanks = 8;
                    break;
                case 0x03:
                    RomSize = 256 * 1024;
                    RomBanks = 16;
                    break;
                case 0x04:
                    RomSize = 512 * 1024;
                    RomBanks = 32;
                    break;
                case 0x05:
                    RomSize = 1024 * 1024;
                    RomBanks = 64;
                    break;
                case 0x06:
                    RomSize = 2 * 1024 * 1024;
                    RomBanks = 128;
                    break;
                case 0x52:
                    RomSize = 1179648;
                    RomBanks = 72;
                    break;
                case 0x53:
                    RomSize = 1310720;
                    RomBanks = 80;
                    break;
                case 0x54:
                    RomSize = 1572864;
                    RomBanks = 96;
                    break;
            }

            switch (fileData[0x0149])
            {
                case 0x00:
                    RamSize = 0;
                    RamBanks = 0;
                    break;
                case 0x01:
                    RamSize = 2 * 1024;
                    RamBanks = 1;
                    break;
                case 0x02:
                    RamSize = 8 * 1024;
                    RamBanks = 1;
                    break;
                case 0x03:
                    RamSize = 32 * 1024;
                    RamBanks = 4;
                    break;
                case 0x04:
                    RamSize = 128 * 1024;
                    RamBanks = 16;
                    break;
            }

            Japanese = fileData[0x014A] == 0x00;
            OldLicenseCode = fileData[0x014B];
            MaskRomVersion = fileData[0x014C];

            HeaderChecksum = fileData[0x014D];
            for (int i = 0x0134; i <= 0x014C; i++)
            {
                ActualHeaderChecksum = ActualHeaderChecksum - fileData[i] - 1;
            }
            ActualHeaderChecksum &= 0xFF;

            Checksum = (((int)fileData[0x014E]) << 8) | fileData[0x014F];
            for (int i = 0; i < fileData.Length; i++)
            {
                if (i != 0x014E && i != 0x014F)
                {
                    ActualChecksum += fileData[i];
                }
            }
            ActualChecksum &= 0xFFFF;

            NoVerticalBlankInterruptHandler = fileData[0x0040] == 0xD9;
            NoLcdcStatusInterruptHandler = fileData[0x0048] == 0xD9;
            NoTimerOverflowInterruptHandler = fileData[0x0050] == 0xD9;
            NoSerialTransferCompletionInterruptHandler = fileData[0x0058] == 0xD9;
            NoHighToLowOfP10ToP13InterruptHandler = fileData[0x0060] == 0xD9;

            switch (RomType)
            {
                case RomType.Rom:
                    Cartridge = new Rom(fileData);
                    break;
                case RomType.RomMbc1:
                case RomType.RomMbc1Ram:
                case RomType.RomMbc1RamBatt:
                    Cartridge = new Mbc1(fileData, RomType, RomSize, RomBanks);
                    break;
                case RomType.RomMbc2:
                case RomType.RomMbc2Battery:
                    Cartridge = new Mbc2(fileData, RomType, RomSize, RomBanks);
                    break;
                default:
                    throw new Exception($"Cannot emulate cartridge type {RomType}.");
            }
        }

        private string ExtractGameTitle(byte[] fileData)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0x0134; i <= 0x0142; i++)
            {
                if (fileData[i] == 0x00)
                {
                    break;
                }
                sb.Append((char)fileData[i]);
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return "title = " + Title + "\n"
                + "game boy color game = " + GameBoyColorGame + "\n"
                + "license code = " + LicenseCode + "\n"
                + "game boy = " + GameBoy + "\n"
                + "rom type = " + RomType + "\n"
                + "rom size = " + RomSize + "\n"
                + "rom banks = " + RomBanks + "\n"
                + "ram size = " + RamSize + "\n"
                + "ram banks = " + RamBanks + "\n"
                + "japanese = " + Japanese + "\n"
                + "old license code = " + OldLicenseCode + "\n"
                + "mask rom version = " + MaskRomVersion + "\n"
                + "header checksum = " + HeaderChecksum + "\n"
                + "actual header checksum = " + ActualHeaderChecksum + "\n"
                + "checksum = " + Checksum + "\n"
                + "actual checksum = " + ActualChecksum + "\n"
                + "no vertical blank interrupt handler = " + NoVerticalBlankInterruptHandler + "\n"
                + "no lcd status interrupt handler = " + NoLcdcStatusInterruptHandler + "\n"
                + "no timer overflow interrupt handler = " + NoTimerOverflowInterruptHandler + "\n"
                + "no serial transfer completion interrupt handler = " + NoSerialTransferCompletionInterruptHandler + "\n"
                + "no high to lower of P10-P13 interrupt handler = " + NoHighToLowOfP10ToP13InterruptHandler + "\n";
        }
    }    
}