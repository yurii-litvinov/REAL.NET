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

using System;
using System.Windows;
using System.Windows.Input;

namespace LogoScene.ViewModels
{
    public class TurtleControlViewModel : ViewModelBase
    {
        public event EventHandler<EventArgs> TurtleMovingEnded;

        public ICommand AnimationCompletedCommand { get; set; }

        public double TurtleWidth => Models.Constants.TurtleWidth;

        public double TurtleHeight => Models.Constants.TurtleHeight;

        public Point Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged();
            }
        }

        public Point End
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

        public void MoveTurtle(Point start, Point end)
        {
            this.start = start;
            this.end = end;
            IsMovingStarted = true;
        }

        public TurtleControlViewModel()
        {
            this.AnimationCompletedCommand = new RelayCommand(
                (parameter) => { isMovingStarted = false; RaiseAnimationCompletedEvent(); });
        }

        private double speedRatio = 1;

        private Point end = new Point(100, 100);

        private Point start = new Point(100, 0);

        private bool isMovingStarted = false;

        private void RaiseAnimationCompletedEvent() => TurtleMovingEnded?.Invoke(this, e: EventArgs.Empty);
    }
}
