using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Emulation
{
    public class Rom : ICartridge
    {
        private byte[] fileData;

        public Rom(byte[] fileData)
        {
            this.fileData = fileData;
        }

        public int ReadByte(int address)
        {
            return fileData[0x7FFF & address];
        }

        public void WriteByte(int address, int value)
        {
        }
    }

    public class Mbc1 : ICartridge
    {
        private RomType romType;
        private bool ramBankingMode;
        private int selectedRomBank = 1;
        private int selectedRamBank;
        private byte[,] ram = new byte[4, 8 * 1024];
        private byte[,] rom;

        public Mbc1(byte[] fileData, RomType romType, int romSize, int romBanks)
        {
            this.romType = romType;
            int bankSize = romSize / romBanks;
            rom = new byte[romBanks, bankSize];
            for (int i = 0, k = 0; i < romBanks; i++)
            {
                for (int j = 0; j < bankSize; j++, k++)
                {
                    rom[i, j] = fileData[k];
                }
            }
        }

        public int ReadByte(int address)
        {
            if (address <= 0x3FFF)
            {
                return rom[0, address];
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                return rom[selectedRomBank, address - 0x4000];
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                return ram[selectedRamBank, address - 0xA000];
            }
            throw new Exception(string.Format("Invalid cartridge read: {0:X}", address));
        }

        public void WriteByte(int address, int value)
        {
            if (address >= 0xA000 && address <= 0xBFFF)
            {
                ram[selectedRamBank, address - 0xA000] = (byte)(0xFF & value);
            }
            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                ramBankingMode = (value & 0x01) == 0x01;
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                int selectedRomBankLow = 0x1F & value;
                if (selectedRomBankLow == 0x00)
                {
                    selectedRomBankLow++;
                }
                selectedRomBank = (selectedRomBank & 0x60) | selectedRomBankLow;
            }
            else if (address >= 0x4000 && address <= 0x5FFF)
            {
                if (ramBankingMode)
                {
                    selectedRamBank = 0x03 & value;
                }
                else
                {
                    selectedRomBank = (selectedRomBank & 0x1F) | ((0x03 & value) << 5);
                }
            }
        }
    }

    public class Mbc2 : ICartridge
    {
        private RomType romType;
        private int selectedRomBank = 1;
        private byte[] ram = new byte[512];
        private byte[,] rom;

        public Mbc2(byte[] fileData, RomType romType, int romSize, int romBanks)
        {
            this.romType = romType;
            int bankSize = romSize / romBanks;
            rom = new byte[romBanks, bankSize];
            for (int i = 0, k = 0; i < romBanks; i++)
            {
                for (int j = 0; j < bankSize; j++, k++)
                {
                    rom[i, j] = fileData[k];
                }
            }
        }

        public int ReadByte(int address)
        {
            if (address <= 0x3FFF)
            {
                return rom[0, address];
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                return rom[selectedRomBank, address - 0x4000];
            }
            else if (address >= 0xA000 && address <= 0xA1FF)
            {
                return ram[address - 0xA000];
            }
            throw new Exception(string.Format("Invalid cartridge address: {0}", address));
        }

        public void WriteByte(int address, int value)
        {
            if (address >= 0xA000 && address <= 0xA1FF)
            {
                ram[address - 0xA000] = (byte)(0x0F & value);
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                selectedRomBank = 0x0F & value;
            }
        }
    }
}
