/* Copyright 2017
 * Yurii Litvinov
 * Ivan Yarkov
 * Egor Zainullin
 * Denis Sushentsev
 * Arseniy Zavalishin
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System;
using System.Windows;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using QuickGraph;
using WPF_Editor.ViewModels.Helpers;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class SceneViewModel : ISceneViewModel
    {
        private static ISceneViewModel _scene;
        private readonly ISceneMediatorViewModel _sceneMediator;
        private VertexControl _firstSelectedVertexControl;
        private GraphArea _graphArea;
        private GXLogicCore _logicCore;
        private ZoomControl _zoomControl;

        private SceneViewModel(ISceneMediatorViewModel sceneMediator)
        {
            this._sceneMediator = sceneMediator;
        }

        public void InitializeScene(ZoomControl zoomControl)
        {
            this._zoomControl = zoomControl;
            this._graphArea = zoomControl.Content as GraphArea;
            if (this._graphArea == null)
                throw new ArgumentException("Zoom control doesn't contain an instance of class GraphArea.");
            var graph = new BidirectionalGraph<ModelNode, ModelEdge>();

            this._logicCore = new GXLogicCore
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK,
                Graph = graph
            };

            this._graphArea.LogicCore = this._logicCore;
            this._graphArea.SetVerticesDrag(true);
            this._graphArea.GenerateGraph();
            zoomControl.CenterContent();
        }


        public void HandleSingleLeftClick(Point position)
        {
            var metaElement = this._sceneMediator.GetSelectedMetamodelType();

            if (metaElement is MetamodelNode)
            {
                var node = this._sceneMediator.GetModelNode(metaElement as MetamodelNode);
                var nodeControl = new VertexControl(node);
                nodeControl.Click += this.TryCreateEdge;
                nodeControl.SetPosition(position);
                this._graphArea.AddVertex(node, nodeControl);
                this._graphArea.RelayoutGraph(true);
            }
        }

        public static ISceneViewModel CreateScene(ISceneMediatorViewModel sceneMediator = null)
        {
            if (_scene == null)
                _scene = new SceneViewModel(sceneMediator);
            return _scene;
        }

        private void TryCreateEdge(object sender, RoutedEventArgs e)
        {
            if (this._sceneMediator.GetSelectedMetamodelType() is MetamodelNode)
            {
                this._firstSelectedVertexControl = null;
                return;
            }
            if (this._firstSelectedVertexControl == null)
            {
                this._firstSelectedVertexControl = sender as VertexControl;
                return;
            }
            var metaEdge = this._sceneMediator.GetSelectedMetamodelType() as MetamodelEdge;
            var secondSelectedVertexControl = sender as VertexControl;

            var source = this._firstSelectedVertexControl.DataContext as ModelNode;
            var target = secondSelectedVertexControl.DataContext as ModelNode;

            var edge = this._sceneMediator.GetModelEdge(metaEdge, source, target);
            var edgeControl = new EdgeControl(this._firstSelectedVertexControl, sender as VertexControl, edge);
            this._graphArea.AddEdge(edge, edgeControl);
            this._graphArea.RelayoutGraph(true);
            this._firstSelectedVertexControl = null;
        }
    }
}