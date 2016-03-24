using GameBot.Core.Data;

namespace GameBot.Core
{
    public interface IEmulator
    {
    }

    public interface IEmulator<T> : IEmulator where T : class, IGameState
    {
        T GameState { get; }

        void Execute(ICommand command);
    }
}
