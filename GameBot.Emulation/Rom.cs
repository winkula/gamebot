using System;

namespace GameBot.Emulation
{
    public class Rom : ICartridge
    {
        private byte[] _fileData;

        public Rom(byte[] fileData)
        {
            _fileData = fileData;
        }

        public int ReadByte(int address)
        {
            return _fileData[0x7FFF & address];
        }

        public void WriteByte(int address, int value)
        {
        }
    }

    public class Mbc1 : ICartridge
    {
        private RomType _romType;
        private bool _ramBankingMode;
        private int _selectedRomBank = 1;
        private int _selectedRamBank;
        private byte[,] _ram = new byte[4, 8 * 1024];
        private byte[,] _rom;

        public Mbc1(byte[] fileData, RomType romType, int romSize, int romBanks)
        {
            _romType = romType;
            int bankSize = romSize / romBanks;
            _rom = new byte[romBanks, bankSize];
            for (int i = 0, k = 0; i < romBanks; i++)
            {
                for (int j = 0; j < bankSize; j++, k++)
                {
                    _rom[i, j] = fileData[k];
                }
            }
        }

        public int ReadByte(int address)
        {
            if (address <= 0x3FFF)
            {
                return _rom[0, address];
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                return _rom[_selectedRomBank, address - 0x4000];
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                return _ram[_selectedRamBank, address - 0xA000];
            }
            throw new Exception(string.Format("Invalid cartridge read: {0:X}", address));
        }

        public void WriteByte(int address, int value)
        {
            if (address >= 0xA000 && address <= 0xBFFF)
            {
                _ram[_selectedRamBank, address - 0xA000] = (byte)(0xFF & value);
            }
            else if (address >= 0x6000 && address <= 0x7FFF)
            {
                _ramBankingMode = (value & 0x01) == 0x01;
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                int selectedRomBankLow = 0x1F & value;
                if (selectedRomBankLow == 0x00)
                {
                    selectedRomBankLow++;
                }
                _selectedRomBank = (_selectedRomBank & 0x60) | selectedRomBankLow;
            }
            else if (address >= 0x4000 && address <= 0x5FFF)
            {
                if (_ramBankingMode)
                {
                    _selectedRamBank = 0x03 & value;
                }
                else
                {
                    _selectedRomBank = (_selectedRomBank & 0x1F) | ((0x03 & value) << 5);
                }
            }
        }
    }

    public class Mbc2 : ICartridge
    {
        private RomType _romType;
        private int _selectedRomBank = 1;
        private byte[] _ram = new byte[512];
        private byte[,] _rom;

        public Mbc2(byte[] fileData, RomType romType, int romSize, int romBanks)
        {
            _romType = romType;
            int bankSize = romSize / romBanks;
            _rom = new byte[romBanks, bankSize];
            for (int i = 0, k = 0; i < romBanks; i++)
            {
                for (int j = 0; j < bankSize; j++, k++)
                {
                    _rom[i, j] = fileData[k];
                }
            }
        }

        public int ReadByte(int address)
        {
            if (address <= 0x3FFF)
            {
                return _rom[0, address];
            }
            else if (address >= 0x4000 && address <= 0x7FFF)
            {
                return _rom[_selectedRomBank, address - 0x4000];
            }
            else if (address >= 0xA000 && address <= 0xA1FF)
            {
                return _ram[address - 0xA000];
            }
            throw new Exception(string.Format("Invalid cartridge address: {0}", address));
        }

        public void WriteByte(int address, int value)
        {
            if (address >= 0xA000 && address <= 0xA1FF)
            {
                _ram[address - 0xA000] = (byte)(0x0F & value);
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                _selectedRomBank = 0x0F & value;
            }
        }
    }
}
