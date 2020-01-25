using EditorPluginInterfaces;
using Interpreters;
using Logo.TurtleManipulation;
using Repo;
using System.Collections.Generic;
using System.Linq;
using static Interpreters.Logo.LogoInterpeter;
using static Interpreters.Logo.LogoSpecific;

namespace LogoScene.ProgramRunner
{
    public class ProgramRunner
    {
        private readonly ITurtleCommander commander;

        private readonly IToolbar toolbar;

        private readonly IConsole console;

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
            var command = new WpfControlsLib.Controls.Toolbar.Command(() => 
            { var list = RunProgram(model); RunCommandList(list); });
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

        private void RunCommandList(List<LogoCommand> list)
        {
            foreach (var command in list)
            {
                // clumsy: fix it
                if (command is LogoForward)
                {
                    var forward = (LogoForward)command;
                    this.commander.MoveForward(forward.Distance);
                }
                else if (command is LogoBackward)
                {
                    var backward = (LogoBackward)command;
                    this.commander.MoveBackward(backward.Distance);
                }
                else if (command is LogoRight)
                {
                    var right = (LogoRight)command;
                    this.commander.RotateRight(right.Degrees);
                }
                else if (command is LogoLeft)
                {
                    var left = (LogoLeft)command;
                    this.commander.RotateLeft(left.Degrees);
                }
                else { }
            }
        }
    }
}
