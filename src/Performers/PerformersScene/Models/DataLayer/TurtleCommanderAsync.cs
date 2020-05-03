using log4net;
using Logo.TurtleInterfaces;
using System;
using System.Collections.Concurrent;
using PerformersScene.Models.Log;
using ITurtle = PerformersScene.TurtleInterfaces.ITurtle;

namespace PerformersScene.Models.DataLayer
{
    internal class TurtleCommanderAsync : ITurtleCommanderAsync
    {
        private readonly TurtleCommander turtleCommander;

        private volatile ConcurrentQueue<Action> actionQueue;

        private readonly ILog log = Logger.Log;

        private readonly object objectToLock = new object();
        
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
            add => turtleCommander.PenActionPerformed += value;

            remove => turtleCommander.PenActionPerformed -= value;
        }

        public event EventHandler<LineEventArgs> MovementStarted
        {
            add => turtleCommander.MovementStarted += value;

            remove => turtleCommander.MovementStarted -= value;
        }

        public event EventHandler<EventArgs> ActionPerformed
        {
            add => turtleCommander.ActionPerformed += value;

            remove => turtleCommander.ActionPerformed -= value;
        }

        public event EventHandler<EventArgs> MovementPerformed
        {
            add => turtleCommander.MovementPerformed += value;

            remove => turtleCommander.MovementPerformed -= value;
        }

        public event EventHandler<EventArgs> RotationPerformed
        {
            add => turtleCommander.RotationPerformed += value;

            remove => turtleCommander.RotationPerformed -= value;
        }

        public event EventHandler<EventArgs> SpeedUpdatedPerformed
        {
            add => turtleCommander.SpeedUpdatedPerformed += value;

            remove => turtleCommander.SpeedUpdatedPerformed -= value;
        }

        public event EventHandler<RotationEventArgs> RotationStarted
        {
            add => turtleCommander.RotationStarted += value;

            remove => turtleCommander.RotationStarted -= value;
        }

        public event EventHandler<PenActionEventArgs> PenActionStarted
        {
            add => turtleCommander.PenActionStarted += value;

            remove => turtleCommander.PenActionStarted -= value;
        }

        public event EventHandler<SpeedUpdateEventArgs> SpeedUpdateStarted
        {
            add => turtleCommander.SpeedUpdateStarted += value;

            remove => turtleCommander.SpeedUpdateStarted -= value;
        }

        public void MoveBackward(double distance)
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.MoveBackward(distance);
                AddActionToQueue(Action);
            }
        }

        public void MoveForward(double distance)
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.MoveForward(distance);
                AddActionToQueue(Action);
            }
        }

        public void PenDown()
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.PenDown();
                AddActionToQueue(Action);
            }
        }

        public void PenUp()
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.PenUp();
                AddActionToQueue(Action);
            }
        }

        public void RotateLeft(double degrees)
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.RotateLeft(degrees);
                AddActionToQueue(Action);
            }
        }

        public void RotateRight(double degrees)
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.RotateRight(degrees);
                AddActionToQueue(Action);
            }
        }

        public void SetSpeed(double speed)
        {
            lock (objectToLock)
            {
                void Action() => turtleCommander.SetSpeed(speed);
                AddActionToQueue(Action);
            }
        }

        public void NotifyMovementPerformed()
        {
            this.turtleCommander.NotifyMovementPerformed();
        }

        public void NotifyRotationPerformed() => this.turtleCommander.NotifyRotationPerformed();

        public void NotifySpeedUpdatedPerformed() => this.turtleCommander.NotifySpeedUpdatePerformed();

        public void NotifyPenActionPerformed() => this.turtleCommander.NotifyPenActionPerformed();

        public void Stop()
        {
            lock (objectToLock)
            {
                actionQueue = new ConcurrentQueue<Action>();
            }
        }

        public void ResetTurtle()
        {
            turtleCommander.ResetTurtle();
            
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            lock (objectToLock)
            {
                if (!actionQueue.IsEmpty)
                {
                    var attempt = actionQueue.TryDequeue(out var action);
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
            lock (objectToLock)
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
