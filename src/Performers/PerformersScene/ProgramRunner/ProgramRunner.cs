using EditorPluginInterfaces;
using Interpreters.Logo.LogoInterpeter;
using Interpreters.RobotPerformer;
using Logo.TurtleInterfaces;
using Repo;
using System.Collections.Generic;
using System.Linq;
using Interpreters.Logo.LogoSpecific;
using Interpreters;
using System;
using System.Security.Cryptography;
using PerformersScene.RobotInterfaces;

namespace PerformersScene.ProgramRunner
{
    public class ProgramRunner
    {
        private IRobotCommanderAsync robotCommander;

        private readonly IRobotMaze maze;

        private readonly ITurtleCommanderAsync logoCommander;

        private readonly IToolbar toolbar;

        private readonly IConsole console;

        private readonly IRepo repo;

        private IModel model;

        private volatile bool isStopped = false;

        private IProgramRunner<ILogoContext> logoRunner;

        private IProgramRunner<IRobotContext> robotRunner;
        
        private bool isToStartFromInitialState;

        public ProgramRunner(IToolbar toolbar, IConsole console, IRepo repo, ITurtleCommanderAsync logoCommander,
            IRobotCommanderAsync robotCommander, IRobotMaze maze)
        {
            this.robotCommander = robotCommander;
            this.maze = maze;
            this.logoCommander = logoCommander;
            this.toolbar = toolbar;
            this.console = console;
            this.repo = repo;
            AddButtons();
        }

        public void SetModel(string modelName) => this.model = repo.Model(modelName);

        public void StopProgram()
        {
            this.logoRunner?.Stop();
            this.logoCommander.Stop();
            this.robotRunner?.Stop();
            this.robotCommander.Stop();
        }

        private void AddButtons()
        {
            var command = new WpfControlsLib.Controls.Toolbar.Command(LaunchProgram);
            var pictureLocation = "pack://application:,,,/" + "View/Pictures/Toolbar/play.png";
            var buttonRun = new WpfControlsLib.Controls.Toolbar.Button(command, "Run program", pictureLocation);
            toolbar.AddButton(buttonRun);
            var commandStop = new WpfControlsLib.Controls.Toolbar.Command(StopProgram);
            var pictureLocationStop = "pack://application:,,,/" + "View/Pictures/Toolbar/stop.png";
            var buttonStop =
                new WpfControlsLib.Controls.Toolbar.Button(commandStop, "Stop program", pictureLocationStop);
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
                RunProgram(model);
            }
        }

        private void RunProgram(IModel model)
        {
            var metamodelName = model.Metamodel.Name;
            if (metamodelName == "LogoMetamodel")
            {
                var list = RunLogoProgram(this.model);
                RunLogoCommandList(list);
            }

            if (metamodelName == "RobotPerformerMetamodel")
            {
                if (isToStartFromInitialState)
                {
                    isToStartFromInitialState = false;
                    robotCommander.ResetRobot();
                }
                RunRobotProgram(this.model);
            }
            else
            {
                console.ReportError("Unknown language " + metamodelName);
            }
        }

        private void RaiseProgramReset()
        {
            isToStartFromInitialState = true;
        }


        private List<LogoCommand> RunLogoProgram(Repo.IModel model)
        {
            logoRunner = new LogoRunner(model);
            try
            {
                logoRunner.Run();
            }
            catch (ParserException e)
            {
                console.ReportError(e.Message);
            }
            catch (OperatorException e)
            {
                console.ReportError(e.Message);
                console.SendMessage(e.Message);
            }
            catch (InterpreterException e)
            {
                console.ReportError(e.Message);
            }

            var context = logoRunner.SpecificContext;
            var commandList = context.LogoCommands.ToList();
            commandList.Reverse();
            return commandList;
        }

        private void RunLogoCommandList(List<LogoCommand> list)
        {
            foreach (var command in list)
            {
                switch (command)
                {
                    // clumsy: fix it
                    case LogoForward forward:
                        this.logoCommander.MoveForward(forward.Distance);
                        break;
                    case LogoBackward backward:
                        this.logoCommander.MoveBackward(backward.Distance);
                        break;
                    case LogoRight right:
                        this.logoCommander.RotateRight(right.Degrees);
                        break;
                    case LogoLeft left:
                        this.logoCommander.RotateLeft(left.Degrees);
                        break;
                    case LogoPenUp _:
                        this.logoCommander.PenUp();
                        break;
                    case LogoPenDown _:
                        this.logoCommander.PenDown();
                        break;
                }
            }
        }

        private void RunRobotProgram(IModel modelToRun)
        {
            bool[,] ToBool(Side[,] lines)
            {
                var booleans = new bool[lines.GetLength(0), lines.GetLength(1)];
                for (int i = 0; i < lines.GetLength(0); i++)
                {
                    for (int j = 0; j < lines.GetLength(1); j++)
                    {
                        booleans[i, j] = lines[i, j].IsWall;
                    }
                }

                return booleans;
            }

            robotRunner = new RobotRunner(modelToRun, ToBool(maze.HorizontalLines), ToBool(maze.VerticalLines));
            try
            {
                while (!robotRunner.IsEnded)
                {
                    robotRunner.Step();
                    var context = robotRunner.SpecificContext;
                    var command = context.WrappedCommands.First();
                    RunRobotCommand(command);
                }
            }
            catch (ParserException e)
            {
                console.ReportError(e.Message);
                RaiseProgramReset();
            }
            catch (OperatorException e)
            {
                console.ReportError(e.Message);
                console.SendMessage(e.Message);
                RaiseProgramReset();
            }
            catch (InterpreterException e)
            {
                console.ReportError(e.Message);
                RaiseProgramReset();
            }
        }

        private void RunRobotCommand(RobotCommand command)
        {
            switch (command)
            {
                case RobotBackward robotBackward:
                    this.robotCommander.MoveBackward();
                    break;
                case RobotForward robotForward:
                    this.robotCommander.MoveForward();
                    break;
                case RobotLeft robotLeft:
                    this.robotCommander.RotateLeft();
                    break;
                case RobotRight robotRight:
                    this.robotCommander.RotateRight();
                    break;
                case RobotNoCommand _:
                    break;
            }
        }
    }
}