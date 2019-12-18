using System;
using System.Windows;

namespace LogoScene.Models
{
    public class MovementEventArgs : EventArgs
    {
        public Point OldPosition { get; }

        public Point NewPosition { get; }

        public MovementEventArgs(Point oldPosition, Point newPosition)
        {
            this.OldPosition = oldPosition;
            this.NewPosition = newPosition;
        }
    }
}