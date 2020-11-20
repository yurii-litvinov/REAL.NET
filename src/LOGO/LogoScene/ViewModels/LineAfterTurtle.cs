﻿/* Copyright 2017-2019 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using Logo.TurtleInterfaces;

namespace LogoScene.ViewModels
{
    public class LineAfterTurtle : ViewModelBase
    {
        private double x1;

        private double y1;

        private double x2;

        private double y2;

        public LineAfterTurtle()
            : this(new DoublePoint(0, 0), new DoublePoint(0, 0)) { }

        public LineAfterTurtle(DoublePoint start, DoublePoint end)
        {
            this.X1 = start.X;
            this.Y1 = start.Y;
            this.X2 = end.X;
            this.Y2 = end.Y;
        }

        public LineAfterTurtle(double x1, double y1, double x2, double y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }

        public double X1
        {
            get => x1;
            set
            {
                x1 = value;
                OnPropertyChanged();
            }
        }

        public double Y1
        {
            get => y1;
            set
            {
                y1 = value;
                OnPropertyChanged();
            }
        }

        public double X2
        {
            get => x2;
            set
            {
                x2 = value;
                OnPropertyChanged();
            }
        }

        public double Y2
        {
            get => y2;
            set
            {
                y2 = value;
                OnPropertyChanged();
            }
        }
    }
}