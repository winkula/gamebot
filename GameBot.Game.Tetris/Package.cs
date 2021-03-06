﻿using GameBot.Core;
using GameBot.Game.Tetris.Extraction;
using GameBot.Game.Tetris.Extraction.Extractors;
using GameBot.Game.Tetris.Extraction.Matchers;
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

            container.RegisterSingleton<IExtractor, MorphologyExtractor>();
            container.RegisterSingleton<IBoardExtractor, BoardExtractor>();
            container.RegisterSingleton<IScreenExtractor, ScreenExtractor>();

            //container.RegisterSingleton<ISearch, OnePieceSearch>();
            container.RegisterSingleton<ISearch, TwoPieceSearch>();
            //container.RegisterSingleton<ISearch, PredictiveSearch>();

            container.RegisterSingleton<IHeuristic, YiyuanLeeHeuristic>();
            //container.RegisterSingleton<IHeuristic, MaxBergmarkHeuristic>();
            //container.RegisterSingleton<IHeuristic, ExperimentalHeuristic>();

            container.RegisterSingleton<IMatcher, MorphologyMatcher>();
            container.RegisterSingleton<TetrisExtractor>();
            container.RegisterSingleton<PieceExtractor>();
            container.RegisterSingleton<TemplateMatcher>();
        }
    }
}
