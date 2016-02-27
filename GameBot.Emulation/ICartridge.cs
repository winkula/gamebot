namespace GameBot.Emulation
{
    public interface ICartridge
    {
        int ReadByte(int address);

        void WriteByte(int address, int value);
    }
}
