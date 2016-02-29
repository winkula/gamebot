using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Robot.Rendering
{
    public interface IRenderer
    {
        void OpenWindow(string title);

        void CreateImage(int width, int height);

        int? Key(int delay = 0);

        void Render(Image image);

        void End();
    }
}
