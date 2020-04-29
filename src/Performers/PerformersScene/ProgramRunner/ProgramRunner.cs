using EditorPluginInterfaces;
using Interpreters.Logo.LogoInterpeter;
using Logo.TurtleInterfaces;
using Repo;
using System.Collections.Generic;
using System.Linq;
using Interpreters.Logo.LogoSpecific;
using Interpreters;
using System;

namespace LogoScene.ProgramRunner
{
    public class ProgramRunner
    {
        private readonly ITurtleCommanderAsync commander;

        private readonly IToolbar toolbar;

        private readonly IConsole console;

        private readonly IRepo repo;

        private IModel model;

        private volatile bool isStopped = false;
        
        private LogoRunner runner;
        
        public ProgramRunner(ITurtleCommanderAsync commander, IToolbar toolbar, IConsole console, IRepo repo)
        {
            this.commander = commander;
            this.toolbar = toolbar;
            this.console = console;
            this.repo = repo;
            AddButtons();
        }

        public void SetModel(string modelName) => this.model = repo.Model(modelName);
        
        public void StopProgram()
        {
            this.runner.Stop();
            this.commander.Stop();
        }

        private void AddButtons()
        {
            var command = new WpfControlsLib.Controls.Toolbar.Command(LaunchProgram);
            var pictureLocation = "pack://application:,,,/" + "View/Pictures/Toolbar/play.png";
            var buttonRun = new WpfControlsLib.Controls.Toolbar.Button(command, "Run program", pictureLocation);
            toolbar.AddButton(buttonRun);
            var commandStop = new WpfControlsLib.Controls.Toolbar.Command(StopProgram);
            var pictureLocationStop = "pack://application:,,,/" + "View/Pictures/Toolbar/stop.png";
            var buttonStop = new WpfControlsLib.Controls.Toolbar.Button(commandStop, "Stop program", pictureLocationStop);
            toolbar.AddButton(buttonStop);
        }

        private void LaunchProgram()
        {
            if (model == null)
            {
                console.ReportError("No model selected");
            }
            else
            {
                var list = RunProgram(this.model);
                RunCommandList(list);
            }
        }

        private List<LogoCommand> RunProgram(Repo.IModel model)
        {
            runner = new LogoRunner(model);
            try
            {
                runner.Run();
            }
            catch (ParserException e)
            {
                console.ReportError(e.Message);
            }
            ILogoContext context = runner.SpecificContext;
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
