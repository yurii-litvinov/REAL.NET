using EditorPluginInterfaces;
using Logo.TurtleManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.ProgramRunner
{
    public class ProgramRunner
    {
        private ITurtleCommander commander;

        private IToolbar toolbar;

        private IConsole console;

        public ProgramRunner(ITurtleCommander commander, IToolbar toolbar, IConsole console)
        {
            this.commander = commander;
            this.toolbar = toolbar;
            this.console = console;
            AddButtons();
        }

        private void AddButtons()
        {
            var command = new WpfControlsLib.Controls.Toolbar.Command(() => { example(); });
            var pictureLocation = "pack://application:,,,/" + "View/Pictures/Toolbar/play.png";
            var button = new WpfControlsLib.Controls.Toolbar.Button(command, "Run program", pictureLocation);
            toolbar.AddButton(button);
        }

        public void RunProgram(Repo.IModel model)
        {
            var itrp = 1;
            example();
        }

        private void example()
        {
            commander.RotateRight(30);
            for (int i = 0; i < 3; i++)
            {
                this.commander.MoveForward(100);
                this.commander.RotateRight(120);
            }
            commander.SetSpeed(4);
            for (int i = 0; i < 3; i++)
            {
                this.commander.RotateLeft(120);
                this.commander.MoveBackward(100);
            }
            commander.RotateLeft(30);
        }
    }
}
