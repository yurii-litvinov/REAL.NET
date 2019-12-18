using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogoScene.Models.Log;
using log4net;

namespace LogoScene.Models.DataLayer
{
    internal class TurtleCommanderAsync : ITurtleCommander
    {
        private readonly TurtleCommander turtleCommander;

        private readonly ConcurrentQueue<Action> actionQueue;

        private readonly ILog log = Logger.Log;

        private TurtleCommanderAsync(TurtleCommander turtleCommander)
        {
            this.turtleCommander = turtleCommander;
            this.actionQueue = new ConcurrentQueue<Action>();
            this.ActionPerformed += OnActionPerformed;
        }

        public TurtleCommanderAsync()
            : this(new TurtleCommander()) { }

        public ITurtle Turtle => turtleCommander.Turtle;

        public bool InProgress => turtleCommander.InProgress;

        public event EventHandler<EventArgs> PenActionPerformed
        {
            add
            {
                turtleCommander.PenActionPerformed += value;
            }

            remove
            {
                turtleCommander.PenActionPerformed -= value;
            }
        }

        public event EventHandler<MovementEventArgs> MovementStarted
        {
            add
            {
                turtleCommander.MovementStarted += value;
            }

            remove
            {
                turtleCommander.MovementStarted -= value;
            }
        }

        public event EventHandler<EventArgs> ActionPerformed
        {
            add
            {
                turtleCommander.ActionPerformed += value;
            }

            remove
            {
                turtleCommander.ActionPerformed -= value;
            }
        }

        public event EventHandler<EventArgs> MovementPerformed
        {
            add
            {
                turtleCommander.MovementPerformed += value;
            }

            remove
            {
                turtleCommander.MovementPerformed -= value;
            }
        }

        public event EventHandler<EventArgs> RotationPerformed
        {
            add
            {
                turtleCommander.RotationPerformed += value;
            }

            remove
            {
                turtleCommander.RotationPerformed -= value;
            }
        }

        public event EventHandler<EventArgs> SpeedUpdatedPerformed
        {
            add
            {
                turtleCommander.SpeedUpdatedPerformed += value;
            }

            remove
            {
                turtleCommander.SpeedUpdatedPerformed -= value;
            }
        }

        public event EventHandler<RotationEventArgs> RotationStarted
        {
            add
            {
                ((ITurtleCommander)turtleCommander).RotationStarted += value;
            }

            remove
            {
                turtleCommander.RotationStarted -= value;
            }
        }

        public event EventHandler<PenActionEventArgs> PenActionStarted
        {
            add
            {
                turtleCommander.PenActionStarted += value;
            }

            remove
            {
                turtleCommander.PenActionStarted -= value;
            }
        }

        public event EventHandler<SpeedUpdateEventArgs> SpeedUpdateStarted
        {
            add
            {
                turtleCommander.SpeedUpdateStarted += value;
            }

            remove
            {
                turtleCommander.SpeedUpdateStarted -= value;
            }
        }

        public void MoveBackward(double distance)
        {
            lock (this)
            {
                Action action = () => { turtleCommander.MoveBackward(distance); };
                AddActionToQueue(action);
                log.Info($"backward {distance}");
            }
        }

        public void MoveForward(double distance)
        {
            lock (this)
            {
                Action action = () => turtleCommander.MoveForward(distance);
                AddActionToQueue(action);
                log.Info($"forward {distance}");
            }
        }

        public void PenDown()
        {
            lock (this)
            {
                Action action = () => turtleCommander.PenDown();
                AddActionToQueue(action);
                log.Info($"pen down");
            }
        }

        public void PenUp()
        {
            lock (this)
            {
                Action action = () => turtleCommander.PenUp();
                AddActionToQueue(action);
                log.Info($"pen up");
            }
        }

        public void RotateLeft(double degrees)
        {
            lock (this)
            {
                Action action = () => turtleCommander.RotateLeft(degrees);
                AddActionToQueue(action);
                log.Info($"rotate left {degrees}");
            }
        }

        public void RotateRight(double degrees)
        {
            lock (this)
            {
                Action action = () => turtleCommander.RotateRight(degrees);
                AddActionToQueue(action);
                log.Info($"rotate right {degrees}");
            }
        }

        public void SetSpeed(double speed)
        {
            lock (this)
            {
                Action action = () => turtleCommander.SetSpeed(speed);
                AddActionToQueue(action);
                log.Info($"set speed {speed}");
            }
        }

        public void NotifyMovementPerformed() => this.turtleCommander.NotifyMovementPerformed();

        public void NotifyRotationPerformed() => this.turtleCommander.NotifyRotationPerformed();

        public void NotifySpeedUpdatedPerformed() => this.turtleCommander.NotifySpeedUpdatePerformed();

        public void NotifyPenActionPerformed() => this.turtleCommander.NotifyPenActionPerformed();

        private void OnActionPerformed(object sender, EventArgs e)
        {
            lock (this)
            {
                if (!actionQueue.IsEmpty)
                {
                    Action action;
                    var attempt = actionQueue.TryDequeue(out action);
                    if (!attempt)
                    {
                        throw new SystemException("can't get action from queue");
                    }
                    action?.Invoke();
                }
            }
        }

        private void AddActionToQueue(Action action)
        {
            if (actionQueue.IsEmpty && !turtleCommander.InProgress)
            {
                action?.Invoke();
            }
            else
            {
                actionQueue.Enqueue(action);
            }
        }
    }
}
