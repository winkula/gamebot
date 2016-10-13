﻿using GameBot.Core;
using GameBot.Game.Tetris.Agents;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Searching;
using GameBot.Game.Tetris.Searching.Heuristics;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace GameBot.Game.Tetris
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IAgent, TetrisAgent>();

            //container.RegisterSingleton<ISearch, SimpleSearch>();
            container.RegisterSingleton<ISearch, RecursiveSearch>();
            container.RegisterSingleton<IHeuristic, YiyuanLeeHeuristic>();

            container.RegisterSingleton<TetrisExtractor>();
            container.RegisterSingleton<TetrisAi>();
        }
    }
}
