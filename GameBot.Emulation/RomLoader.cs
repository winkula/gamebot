using System.IO;

namespace GameBot.Emulation
{
    public class RomLoader
    {
        public Game Load(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            byte[] fileData = new byte[fileInfo.Length];
            FileStream fileStream = fileInfo.OpenRead();
            fileStream.Read(fileData, 0, fileData.Length);
            fileStream.Close();

            return new Game(fileData);
        }
    }
}
