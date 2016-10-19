namespace GameBot.Game.Tetris.Agents.States
{
    public interface ITetrisAgentState
    {
        // this method could be omitted, if we dont have a gui
        void Extract();
        
        void Play();
    }
}
