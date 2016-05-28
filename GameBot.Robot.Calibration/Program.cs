using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Ui;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace GameBot.Robot.Ui
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var container = new Container())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                container.RegisterPackages(GetAssemblies(
                    "GameBot.Emulation",
                    "GameBot.Robot",
                    "GameBot.Robot.Ui",
                    "GameBot.Game.Tetris"));
                container.Verify();

                Application.Run((Form)container.GetInstance<IUi>());
            }
        }

        static IEnumerable<Assembly> GetAssemblies(params string[] assemblyNames)
        {
            return assemblyNames.Select(x => Assembly.Load(x)).ToList();
        }
    }
}
