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

using EditorPluginInterfaces;
using Logo.TurtleManipulation;
using LogoScene.Models;
using LogoScene.Models.Log;
using System;
using System.Collections.ObjectModel;

namespace LogoScene.ViewModels
{
    public class DrawingSceneViewModel : ViewModelBase
    {
        public TurtleControlViewModel TurtleViewModel { get; private set; }

        public ObservableCollection<LineAfterTurtle> LinesOnScene { get; private set; } = new ObservableCollection<LineAfterTurtle>();

        public bool IsLineAnimated
        {
            get => isLineAnimated;
            set
            {
                isLineAnimated = value;
                OnPropertyChanged();
            }
        }

        public bool IsLineVisible
        {
            get => isLineVisible;
            set
            {
                isLineVisible = value;
                OnPropertyChanged();
            }
        }

        public DoublePoint StartPoint
        {
            get => startPoint;
            set
            {
                startPoint = value;
                OnPropertyChanged();
            }
        }

        public DoublePoint FinalPoint
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

        public ITurtleCommander TurtleCommander => commander;

        public DrawingSceneViewModel()
        {
            Logger.InitLogger();
            this.model = new DrawingScene();
            this.commander = model.GetTurtleCommander();
            this.turtleModel = commander.Turtle;
            this.TurtleViewModel = new TurtleControlViewModel();
            this.TurtleViewModel.InitModel(turtleModel);
            this.TurtleViewModel.TurtleMovingEnded += OnTurtleMovementEnded;
            this.TurtleViewModel.TurtleRotationEnded += OnTurtleRotationEnded;
            this.TurtleViewModel.TurtleSpeedUpdateEnded += OnTurtleSpeedUpdateEnded;
            this.model.MovementOnDrawingSceneStarted += OnMovementStarted;
            this.model.LineAdded += OnLineAdded;
            this.commander.RotationStarted += OnRotationStarted;
            this.commander.SpeedUpdateStarted += OnSpeedUpdateStarted;
            this.commander.PenActionStarted += OnPenActionStarted;
            this.IsLineVisible = this.commander.Turtle.IsPenDown;
        }

        public void MoveTurtle(DoublePoint startPoint, DoublePoint finalPoint)
        {
            this.StartPoint = startPoint;
            this.FinalPoint = finalPoint;
            this.TurtleViewModel.MoveTurtle(this.StartPoint, this.FinalPoint);
        }

        public void MoveTurtle(DoublePoint destination) => MoveTurtle(this.StartPoint, destination);

        public void MoveTurtle() => MoveTurtle(this.StartPoint, this.FinalPoint);

        private void OnTurtleMovementEnded(object sender, EventArgs e)
        {
            IsLineAnimated = false;
            this.model.NotifyMovementPerformed();
        }

        private void OnTurtleSpeedUpdateEnded(object sender, EventArgs e) => this.model.NotifySpeedUpdatedPerformed();

        private void OnTurtleRotationEnded(object sender, EventArgs e) => this.model.NotifyRotationPerformed();

        private void OnMovementStarted(object sender, LineEventArgs e)
        {
            this.StartPoint = e.StartPoint;
            this.FinalPoint = e.EndPoint;
            IsLineAnimated = true;
            Logger.Log.Info($"{e.StartPoint} {e.EndPoint} {IsLineAnimated}");
            MoveTurtle();
        }

        private void OnRotationStarted(object sender, RotationEventArgs e) => this.TurtleViewModel.UpdateAngle();

        private void OnSpeedUpdateStarted(object sender, SpeedUpdateEventArgs e)
        {
            this.SpeedRatio = this.commander.Turtle.Speed;
            this.TurtleViewModel.UpdateSpeed();
        }

        private void OnPenActionStarted(object sender, PenActionEventArgs e)
        {
            this.IsLineVisible = commander.Turtle.IsPenDown;
            this.OnPenActionPerformed(this, EventArgs.Empty);
        }

        private void OnPenActionPerformed(object sender, EventArgs e) => this.model.NotifyPenActionPerformed();

        private void OnLineAdded(object sender, LineEventArgs e)
        {
            this.LinesOnScene.Add(new LineAfterTurtle(e.StartPoint, e.EndPoint));
        }

        private double speedRatio = 1;

        private DoublePoint startPoint = new DoublePoint(100, 100);

        private DoublePoint finalPoint = new DoublePoint(100, 0);

        private bool isLineAnimated;

        private bool isLineVisible;

        private readonly DrawingScene model;

        private readonly ITurtleCommander commander;

        private readonly ITurtle turtleModel;
    }
}
