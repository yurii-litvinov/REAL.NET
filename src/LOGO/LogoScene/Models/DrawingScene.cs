using LogoScene.Models.DataLayer;
using System;

namespace LogoScene.Models
{
    public class DrawingScene 
    {              
        public ITurtleCommander GetTurtleCommander() => turtleCommander;

        public event EventHandler<MovementEventArgs> MovementOnDrawingSceneStarted;

        public DrawingScene()
        {
            var turtle = new Turtle(100, 100, 90, false);
            this.turtleCommander = new TurtleCommanderAsync();
            this.turtleCommander.MovementStarted += RaiseMovementOnDrawingSceneStarted;
        }

        public void NotifyMovementPermormed() => turtleCommander.NotifyMovementPerformed();

        public void NotifyRotationPerformed() => turtleCommander.NotifyRotationPerformed();

        public void NotifySpeedUpdatedPerformed() => turtleCommander.NotifySpeedUpdatedPerformed();

        private readonly TurtleCommanderAsync turtleCommander;

        private void RaiseMovementOnDrawingSceneStarted(object sender, MovementEventArgs e) => MovementOnDrawingSceneStarted?.Invoke(this, e);

    }
}
