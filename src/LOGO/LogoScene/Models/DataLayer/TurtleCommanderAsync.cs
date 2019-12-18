using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.Models.DataLayer
{
    internal class TurtleCommanderAsync : ITurtleCommander
    {
        private readonly TurtleCommander turtleCommander;

        private readonly ConcurrentQueue<Action> actionQueue;        

        private TurtleCommanderAsync(TurtleCommander turtleCommander)
        {
            this.turtleCommander = turtleCommander;
            this.actionQueue = new ConcurrentQueue<Action>();
            this.ActionPerformed += OnActionPerformed;
        }

        public TurtleCommanderAsync()        
            : this(new TurtleCommander()){}

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

        public void MoveBackward(double distance)
        {
            lock (this)
            {
                Action action = () => { turtleCommander.MoveBackward(distance); };
                AddActionToQueue(action);
            }
        }

        public void MoveForward(double distance)
        {
            lock (this)
            {
                Action action = () => turtleCommander.MoveForward(distance);
                AddActionToQueue(action);
            }
        }

        public void PenDown()
        {
            lock (this)
            {
                Action action = () => turtleCommander.PenDown();
                AddActionToQueue(action);
            }
        }

        public void PenUp()
        {
            lock (this)
            {
                Action action = () => turtleCommander.PenUp();
                AddActionToQueue(action);
            }
        }

        public void RotateLeft(double degrees)
        {
            lock (this)
            {
                Action action = () => turtleCommander.RotateLeft(degrees);
                AddActionToQueue(action);
            }
        }

        public void RotateRight(double degrees)
        {
            lock (this)
            {
                Action action = () => turtleCommander.RotateRight(degrees);
                AddActionToQueue(action);
            }
        }

        public void SetSpeed(double speed)
        {
            lock (this)
            {
                Action action = () => turtleCommander.SetSpeed(speed);
                AddActionToQueue(action);
            }
        }

        public void NotifyMovementPerformed() => this.turtleCommander.NotifyMovementPerformed();

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
