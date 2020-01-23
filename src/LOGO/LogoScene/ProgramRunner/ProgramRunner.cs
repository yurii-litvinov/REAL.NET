using EditorPluginInterfaces;
using Logo.TurtleManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using Interpreters;
using static Languages.Logo.LogoSpecific;
using static Languages.Logo.LogoInterpeter;


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
            var repo = RepoFactory.Create();
            // TODO: remove hardcode
            Repo.IModel model = repo.Model("LogoModel");
            var list = RunProgram(model);
            var command = new WpfControlsLib.Controls.Toolbar.Command(() => { runCommandList(list); });
            var pictureLocation = "pack://application:,,,/" + "View/Pictures/Toolbar/play.png";
            var button = new WpfControlsLib.Controls.Toolbar.Button(command, "Run program", pictureLocation);
            toolbar.AddButton(button);
        }

        private List<LogoCommand> RunProgram(Repo.IModel model)
        {
            IProgramRunner<ILogoContext> runner = new LogoRunner(model);
            runner.Run();
            ILogoContext context = runner.SpicificContext;
            var commandList = context.LogoCommands.ToList();
            commandList.Reverse();
            return commandList;
        }

        private void runCommandList(List<LogoCommand> list)
        {
            foreach (var command in list)
            {
                // clumsy: fix it
                if (command is LogoForward)
                {
                    var forward = (LogoForward)command;
                    this.commander.MoveForward(forward.Distance);
                }
                else if (command is LogoRight)
                {
                    var right = (LogoRight)command;
                    this.commander.RotateRight(right.Degrees);
                }
                else { }
            }
            
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
