﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoScene.Models.DataLayer
{
    internal class Turtle : ITurtle
    {
        public double Speed { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Angle { get; private set; }

        public bool IsPenDown { get; private set; }

        public Turtle(double x, double y, double angle, bool isPenDown, double speed)
        {
            this.X = x;
            this.Y = y;
            this.Angle = angle;
            this.IsPenDown = isPenDown;
            this.Speed = speed;
        }

        public Turtle(double x, double y, double angle, bool isPenDown)
            : this(x, y, angle, isPenDown, 1) { }

        public void SetX(double newX) => this.X = newX; 

        public void SetY(double newY) => this.Y = newY; 
        
        public void SetSpeed(double newSpeed) => this.Speed = newSpeed;

        public void SetIsPenDown(bool isPenDown) => this.IsPenDown = isPenDown;

        public void SetAngle(double newAngle)
        {
            while (newAngle >= 360)
            {
                newAngle -= 360;
            }
            while (newAngle < 0)
            {
                newAngle += 360;
            }
            this.Angle = newAngle;
        }
    }
}
