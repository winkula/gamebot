namespace GameBot.Game.Tetris.Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            new GeneticAlgorithmProgram().Run();
            return;

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
