namespace GameBot.Game.Tetris.Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "ga")
            {
                new GeneticAlgorithmProgram().Run();
            }
            else
            {
                new SimulatorProgram().Run();
            }
        }
    }
}
