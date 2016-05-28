using Emgu.CV;

namespace GameBot.Core
{
    public interface IUi
    {
        void Show(IImage original, IImage processed);
    }
}
