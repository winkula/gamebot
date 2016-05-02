using Emgu.CV;
using System.Drawing;

namespace GameBot.Robot.Renderers
{
    public interface IRenderer
    {
        void OpenWindow(string title);

        void CreateImage(int width, int height);

        int? Key(int delay = 0);

        void Render(Image image, string title);

        void Render(IImage image, string title);

        void End();
    }
}
