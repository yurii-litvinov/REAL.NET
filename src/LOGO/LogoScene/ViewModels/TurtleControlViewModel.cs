/* Copyright 2017-2019 REAL.NET group
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

using Logo.TurtleManipulation;
using LogoScene.ViewModels.Animation;
using System;
using System.Windows.Input;

namespace LogoScene.ViewModels
{
    public class TurtleControlViewModel : ViewModelBase, IAnimationCompletedHandler
    {
        public event EventHandler<EventArgs> TurtleMovingEnded;

        public event EventHandler<EventArgs> TurtleRotationEnded;

        public event EventHandler<EventArgs> TurtleSpeedUpdateEnded;

        public ICommand AnimationCompletedCommand { get; set; }

        public double TurtleWidth => Models.Constants.TurtleWidth;

        public double TurtleHeight => Models.Constants.TurtleHeight;

        public double CenterX => TurtleWidth / 2;

        public double CenterY => TurtleHeight / 2;

        public double Angle
        {
            get => angle;
            set
            {
                angle = value;
                OnPropertyChanged();
            }
        }

        public DoublePoint Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged();
            }
        }

        public DoublePoint End
        {
            get => end;

            set
            {
                end = value;
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

        public bool IsMovingStarted
        {
            get => isMovingStarted;
            set
            {
                isMovingStarted = value;
                OnPropertyChanged();
            }
        }

        public void MoveTurtle(DoublePoint start, DoublePoint end)
        {
            this.Start = start;
            this.End = end;
            IsMovingStarted = true;
        }

        public void UpdateAngle()
        {
            this.Angle = turtleModel.Angle;
            TurtleRotationEnded?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateSpeed()
        {
            this.SpeedRatio = turtleModel.Speed;
            TurtleSpeedUpdateEnded?.Invoke(this, EventArgs.Empty);
        }

        public void InitModel(ITurtle turtle)
        {
            this.turtleModel = turtle;
        }

        public TurtleControlViewModel()
        {
            this.AnimationCompletedCommand = new RelayCommand(
                (parameter) => { isMovingStarted = false; RaiseAnimationCompletedEvent(); });
            this.Start = new DoublePoint(100, 100);
            this.End = new DoublePoint(100, 100);
        }

        private double speedRatio = 1;

        private DoublePoint end = new DoublePoint(0, 0);

        private DoublePoint start = new DoublePoint(0, 0);

        private bool isMovingStarted = false;
        private ITurtle turtleModel;

        private double angle = 90;

        private void RaiseAnimationCompletedEvent() => TurtleMovingEnded?.Invoke(this, e: EventArgs.Empty);
    }
}
