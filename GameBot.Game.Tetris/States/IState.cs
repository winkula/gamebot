namespace GameBot.Game.Tetris.States
{
    public interface IState
    {
        // this method could be omitted, if we dont have a gui
        void Extract();
        
        void Play();
    }
}
