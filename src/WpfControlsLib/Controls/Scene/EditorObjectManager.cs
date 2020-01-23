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

using GraphX.Controls;
using System;
using System.Windows;
using System.Windows.Media;

namespace WpfControlsLib.Controls.Scene
{
    /// <summary>
    /// Creates, holds and controls various auxiliary objects on a scene, like virtual edge used to draw future edge
    /// position when user draws a new edge.
    /// </summary>
    public class EditorObjectManager : IDisposable
    {
        private GraphArea graphArea;
        private ZoomControl zoomControl;
        private EdgeBlueprint edgeBlueprint;
        private ResourceDictionary resourceDictionary;

        public EditorObjectManager(GraphArea graphArea, ZoomControl zc)
        {
            this.graphArea = graphArea;
            this.zoomControl = zc;
            this.resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/View/Templates/EditorTemplate.xaml", UriKind.RelativeOrAbsolute),
            };
        }

        public void CreateVirtualEdge(VertexControl source, Point targetPos)
        {
            this.zoomControl.MouseMove += this.ZoomControlMouseMove;
            this.edgeBlueprint = new EdgeBlueprint(source, targetPos, (LinearGradientBrush)this.resourceDictionary["EdgeBrush"]);
            this.graphArea.InsertCustomChildControl(0, this.edgeBlueprint.EdgePath);
        }

        public void Dispose()
        {
            this.ClearEdgeBlueprint();
            this.graphArea = null;
            if (this.zoomControl != null)
            {
                this.zoomControl.MouseMove -= this.ZoomControlMouseMove;
            }

            this.zoomControl = null;
            this.resourceDictionary = null;
        }

        public void DestroyVirtualEdge()
        {
            this.ClearEdgeBlueprint();
        }

        private void ZoomControlMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.edgeBlueprint == null)
            {
                return;
            }

            var pos = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.graphArea);
            pos.Offset(2, 2);
            this.edgeBlueprint.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBlueprint()
        {
            if (this.edgeBlueprint == null)
            {
                return;
            }

            this.graphArea.RemoveCustomChildControl(this.edgeBlueprint.EdgePath);
            this.edgeBlueprint.Dispose();
            this.edgeBlueprint = null;
        }
    }
}