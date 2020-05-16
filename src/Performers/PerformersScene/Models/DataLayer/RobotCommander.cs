using System;
using System.Collections.Concurrent;
using Microsoft.Expression.Interactivity.Media;
using PerformersScene.RobotInterfaces;

namespace PerformersScene.Models.DataLayer
{
    public class RobotCommander : IRobotCommanderAsync
    {
        private volatile ConcurrentQueue<Action> actionQueue;

        private volatile bool isInProgress = false;

        private readonly object objectToLock = new object();

        private readonly IRobot initialState;
        
        private volatile IRobot robotPerformer;
        
        public event EventHandler<EventArgs> RobotReset;

        public RobotCommander(IRobot robotPerformer)
        {
            RobotPerformer = robotPerformer;
            initialState = Robot.CreateRobot(robotPerformer.Direction, robotPerformer.Position);
            actionQueue = new ConcurrentQueue<Action>();
        }

        public IRobot RobotPerformer
        {
            get => robotPerformer;
            private set => robotPerformer = value;
        }

        private IRobot MoveAnimation(int k)
        {
            var oldPosition = RobotPerformer.Position;
            var newPosition = new IntPoint();
            switch (RobotPerformer.Direction)
            {
                case Direction.Up:
                    newPosition = (new IntPoint(0, 1));
                    break;
                case Direction.Down:
                    newPosition = new IntPoint(0, -1);
                    break;
                case Direction.Right:
                    newPosition = new IntPoint(1, 0);
                    break;
                case Direction.Left:
                    newPosition = new IntPoint(-1, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            newPosition = k * newPosition + oldPosition;
            return Robot.CreateRobot(RobotPerformer.Direction, newPosition);
        }

        private IRobot LeftHelper()
        {
            var position = RobotPerformer.Position;
            switch (RobotPerformer.Direction)
            {
                case Direction.Up:
                    return Robot.CreateRobot(Direction.Left, position);
                case Direction.Down:
                    return Robot.CreateRobot(Direction.Right, position);
                case Direction.Right:
                    return Robot.CreateRobot(Direction.Up, position);
                case Direction.Left:
                    return Robot.CreateRobot(Direction.Down, position);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IRobot RightHelper()
        {
            var position = RobotPerformer.Position;
            switch (RobotPerformer.Direction)
            {
                case Direction.Up:
                    return Robot.CreateRobot(Direction.Right, position);
                case Direction.Down:
                    return Robot.CreateRobot(Direction.Left, position);
                case Direction.Right:
                    return Robot.CreateRobot(Direction.Down, position);
                case Direction.Left:
                    return Robot.CreateRobot(Direction.Up, position);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void MoveForward()
        {
            lock (objectToLock)
            {
                void Action()
                {
                    var robot = MoveAnimation(1);
                    RobotPerformer = robot;
                    MovementStarted?.Invoke(this, new RobotEvent(robot));
                }

                AddToQueue(Action);
            }
        }

        public void MoveBackward()
        {
            lock (objectToLock)
            {
                void Action()
                {
                    var robot = MoveAnimation(-1);
                    RobotPerformer = robot;
                    MovementStarted?.Invoke(this, new RobotEvent(robot));
                }

                AddToQueue(Action);
            }
        }

        public void RotateLeft()
        {
            lock (objectToLock)
            {
                void Action()
                {
                    var robot = LeftHelper();
                    RobotPerformer = robot;
                    RotationStarted?.Invoke(this, new RobotEvent(robot));
                }
                
                AddToQueue(Action);
            }
        }

        public void RotateRight()
        {
            lock (objectToLock)
            {
                void Action()
                {
                    var robot = RightHelper();
                    RobotPerformer = robot;
                    RotationStarted?.Invoke(this, new RobotEvent(robot));
                }
                
                AddToQueue(Action);
            }
        }

        public void Stop()
        {
            lock (objectToLock)
            {
                actionQueue = new ConcurrentQueue<Action>();
                isInProgress = false;
            }
        }
        
        public void ResetRobot()
        {
            lock (objectToLock)
            {
                isInProgress = false;
                RobotPerformer = initialState;
                RobotReset?.Invoke(this, EventArgs.Empty);
            }
        }

        public void NotifyActionDone()
        {
            isInProgress = false;
            lock (objectToLock)
            {
                if (!actionQueue.IsEmpty)
                {
                    isInProgress = true;
                    var attempt = actionQueue.TryDequeue(out var action);
                    if (!attempt)
                    {
                        throw new SystemException("can't get action from queue");
                    }
                    action?.Invoke();
                }
                ActionDone?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<RobotEvent> MovementStarted;

        public event EventHandler<RobotEvent> RotationStarted;

        public event EventHandler<EventArgs> ActionDone;

        private void AddToQueue(Action action)
        {
            lock (objectToLock)
            {
                if (actionQueue.IsEmpty && !isInProgress)
                {
                    isInProgress = true;
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