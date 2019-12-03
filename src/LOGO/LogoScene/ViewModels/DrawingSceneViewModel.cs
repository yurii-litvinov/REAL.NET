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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogoScene.ViewModels
{
    public class DrawingSceneViewModel : ViewModelBase
    {
        public TurtleControlViewModel TurtleViewModel { get; set; }

        public Point StartPoint
        {
            get => startPoint;
            set
            {
                startPoint = value;
                OnPropertyChanged();
            }
        }

        public Point FinalPoint
        {
            get => finalPoint;
            set
            {
                finalPoint = value;
                OnPropertyChanged();
            }
        }

        public double SpeedRatio
        {
            get => speedRatio;
            set
            {
                speedRatio = value;
                OnPropertyChanged();
            }
        }

        public DrawingSceneViewModel()
        {
            this.TurtleViewModel = new TurtleControlViewModel();
            this.TurtleViewModel.MoveTurtle(this.StartPoint, this.FinalPoint);
            this.TurtleViewModel.TurtleMovingEnded += OnTurtleMovementEnded;
        }

        private void OnTurtleMovementEnded(object sender, EventArgs e)
        {
            this.TurtleViewModel.MoveTurtle(this.StartPoint, this.FinalPoint);
        }

        private double speedRatio = 0.3;

        private Point startPoint = new Point(100, 100);

        private Point finalPoint = new Point(100, 0);
    }
}
