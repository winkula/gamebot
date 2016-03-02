﻿using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.ImageProcessing;
using GameBot.Emulation;
using GameBot.Robot.Actors;
using GameBot.Robot.Rendering;
using SimpleInjector;
using System.Reflection;
using System.Linq;
using System;

namespace GameBot.Robot
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            var container = BuildContainer();

            var engine = container.GetInstance<IEngine>();
            engine.Run();
        }

        static Container BuildContainer()
        {
            var container = new Container();

            container.Register<Emulator>();

            container.Register<IEngine, Engine>();

            container.Register<IQuantizer, Quantizer>();
            container.Register<IExecuter, Executer>();

            container.Register<IRenderer, EmguRenderer>();

            // TODO: remove build-dependency to the "GameBot.Game.Tetris" and load
            // the assembly with "LoadFrom"
            //var assembly = Assembly.LoadFrom(@"C:\Users\Winkler\Documents\visual studio 2015\Projects\GameBot\GameBot.Game.Tetris\bin\x86\Debug\GameBot.Game.Tetris.dll"); //Assembly.Load(assemblyName);

            var assemblyName = "GameBot.Game.Tetris";
            var assembly = Assembly.Load(assemblyName);

            container.Register(typeof(IGameState), GetSingleImplementation<IGameState>(assembly));
            container.Register(typeof(IAgent), GetSingleImplementation<IAgent>(assembly));
            container.Register(typeof(IExtractor<>), new[] { assembly });
            container.Register(typeof(IDecider<>), new[] { assembly });

            container.Verify();

            return container;
        }

        static Type GetSingleImplementation<T>(Assembly assembly)
        {
            return assembly
                .GetExportedTypes()
                .Where(x => x.GetInterfaces().Any(y => y == typeof(T)))
                .Single();
        }
    }
}
