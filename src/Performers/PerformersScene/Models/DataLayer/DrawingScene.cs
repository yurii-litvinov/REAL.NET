using Logo.TurtleInterfaces;
using System;
using System.Collections.Generic;
using PerformersScene.Models.DataLayer;
using PerformersScene.TurtleInterfaces;

namespace PerformersScene.Models
{
    internal class DrawingScene
    {
        public ITurtleCommanderAsync GetTurtleCommander() => turtleCommander;

        public IEnumerable<OrientedLine> Lines => lines;

        public event EventHandler<LineEventArgs> MovementOnDrawingSceneStarted;

        public event EventHandler<LineEventArgs> LineAdded;

        public DrawingScene()
        {
            this.turtleCommander = new TurtleCommanderAsync();
            this.turtleCommander.MovementStarted += RaiseMovementOnDrawingSceneStarted;
        }

        public void NotifyMovementPerformed()
        {
            if (turtleCommander.Turtle.IsPenDown)
            {
                AddLine(startOfLine, endOfLine);
                LineAdded?.Invoke(this, new LineEventArgs(startOfLine, endOfLine));
            }
            turtleCommander.NotifyMovementPerformed();
        }

        public void NotifyRotationPerformed() => turtleCommander.NotifyRotationPerformed();

        public void NotifySpeedUpdatedPerformed() => turtleCommander.NotifySpeedUpdatedPerformed();

        public void NotifyPenActionPerformed() => turtleCommander.NotifyPenActionPerformed();

        private readonly TurtleCommanderAsync turtleCommander;

        private List<OrientedLine> lines = new List<OrientedLine>();

        private DoublePoint startOfLine;

        private DoublePoint endOfLine;

        private void RaiseMovementOnDrawingSceneStarted(object sender, LineEventArgs e)
        {
            startOfLine = e.StartPoint;
            endOfLine = e.EndPoint;
            MovementOnDrawingSceneStarted?.Invoke(this, e);
        }

        private void AddLine(DoublePoint start, DoublePoint end)
        {
            var line = new OrientedLine(start, end);
            lines.Add(line);
        }
    }
}
