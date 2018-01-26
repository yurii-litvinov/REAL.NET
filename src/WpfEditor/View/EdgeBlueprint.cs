/* Copyright 2017-2018 REAL.NET group
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

namespace WpfEditor.View
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using GraphX.Controls;
    using GraphX.Controls.Models;

    /// <summary>
    /// Control for virtual edge that is used when one clicks on a first node and draws an edge to a second node.
    /// Actual edge is created only when drawing is finished (by clicking on a target node).
    /// </summary>
    internal class EdgeBlueprint : IDisposable
    {
        public EdgeBlueprint(VertexControl source, Point targetPos, Brush brush)
        {
            this.EdgePath = new Path { Stroke = brush, Data = new LineGeometry() };
            this.Source = source;
            this.TargetPos = targetPos;
            this.Source.PositionChanged += this.Source_PositionChanged;
        }

        public VertexControl Source { get; set; }

        public Point TargetPos { get; set; }

        public Path EdgePath { get; set; }

        public void Dispose()
        {
            if (this.Source != null)
            {
                this.Source.PositionChanged -= this.Source_PositionChanged;
                this.Source = null;
            }
        }

        public void UpdateTargetPosition(Point point)
        {
            this.TargetPos = point;
            if (this.Source != null)
            {
                this.UpdateGeometry(this.Source.GetCenterPosition(), point);
            }
        }

        private void Source_PositionChanged(object sender, VertexPositionEventArgs args)
        {
            this.UpdateGeometry(this.Source.GetCenterPosition(), this.TargetPos);
        }

        private void UpdateGeometry(Point start, Point end)
        {
            var geometry = new LineGeometry(start, end);
            this.EdgePath.Data = geometry;
            geometry.Freeze();
        }
    }
}