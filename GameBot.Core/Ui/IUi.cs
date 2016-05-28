using Emgu.CV;

namespace GameBot.Core.Ui
{
    public interface IUi
    {
        void Show(IImage original, IImage processed);
    }
}
