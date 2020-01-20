using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogoScene.Models.Log;
using log4net;
using Logo.TurtleManipulation;

namespace LogoScene.Models.DataLayer
{
    internal class TurtleCommanderAsync : ITurtleCommander
    {
        private readonly TurtleCommander turtleCommander;

        private readonly ConcurrentQueue<Action> actionQueue;

        private readonly ILog log = Logger.Log;

        private object objectToLock = new object();

        private TurtleCommanderAsync(TurtleCommander turtleCommander)
        {
            this.turtleCommander = turtleCommander;
            this.actionQueue = new ConcurrentQueue<Action>();
            this.ActionPerformed += OnActionPerformed;
        }

        public TurtleCommanderAsync()
            : this(new TurtleCommander()) { }

        public ITurtle Turtle => turtleCommander.Turtle;

        public bool IsInProgress => turtleCommander.IsInProgress;

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

        public event EventHandler<LineEventArgs> MovementStarted
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
            lock (objectToLock)
            {
                Action action = () => { turtleCommander.MoveBackward(distance); };
                AddActionToQueue(action);
            }
        }

        public void MoveForward(double distance)
        {
            lock (objectToLock)
            {
                Action action = () => turtleCommander.MoveForward(distance);
                AddActionToQueue(action);
            }
        }

        public void PenDown()
        {
            lock (objectToLock)
            {
                Action action = () => turtleCommander.PenDown();
                AddActionToQueue(action);
            }
        }

        public void PenUp()
        {
            lock (objectToLock)
            {
                Action action = () => turtleCommander.PenUp();
                AddActionToQueue(action);
            }
        }

        public void RotateLeft(double degrees)
        {
            lock (objectToLock)
            {
                Action action = () => turtleCommander.RotateLeft(degrees);
                AddActionToQueue(action);
            }
        }

        public void RotateRight(double degrees)
        {
            lock (objectToLock)
            {
                Action action = () => turtleCommander.RotateRight(degrees);
                AddActionToQueue(action);
            }
        }

        public void SetSpeed(double speed)
        {
            lock (objectToLock)
            {
                Action action = () => turtleCommander.SetSpeed(speed);
                AddActionToQueue(action);
            }
        }

        public void NotifyMovementPerformed()
        {
            this.turtleCommander.NotifyMovementPerformed();
        }

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
            lock (this)
            {
                if (actionQueue.IsEmpty && !turtleCommander.IsInProgress)
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
}
