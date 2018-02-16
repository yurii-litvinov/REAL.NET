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

using System;
using System.Linq;
using System.Windows.Controls;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using QuickGraph;
using WpfEditor.Model;
using WpfEditor.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphX.Controls.Models;

namespace WpfEditor.Controls.Scene
{
    public partial class Scene : UserControl
    {
        private readonly EditorObjectManager editorManager;
        private VertexControl prevVer;
        private VertexControl ctrlVer;
        private EdgeControl ctrlEdg;
        private Point pos;

        private Model.Model model;
        private Controller.Controller controller;
        private IElementProvider elementProvider;

        public Graph Graph { get; set; }

        public event EventHandler<EventArgs> ElementUsed;
        public event EventHandler<NodeSelectedEventArgs> NodeSelected;
        public event EventHandler<EdgeSelectedEventArgs> EdgeSelected;

        public event EventHandler<Graph.ElementAddedEventArgs> ElementAdded;

        public Scene()
        {
            this.InitializeComponent();

            this.editorManager = new EditorObjectManager(this.scene, this.zoomControl);

            ZoomControl.SetViewFinderVisibility(this.zoomControl, Visibility.Hidden);

            this.scene.VertexSelected += this.VertexSelectedAction;
            this.scene.EdgeSelected += this.EdgeSelectedAction;
            this.zoomControl.Click += this.ClearSelection;
            this.zoomControl.MouseDown += this.OnSceneMouseDown;
            this.zoomControl.Drop += this.ZoomControl_Drop;
        }

        private void InitGraphXLogicCore()
        {
            var logic =
                new GXLogicCore<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>
                {
                    Graph = this.Graph.DataGraph,
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog
                };

            this.scene.LogicCore = logic;

            logic.DefaultLayoutAlgorithmParams =
                logic.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.LinLog);
            logic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams =
                logic.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            logic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            logic.AsyncAlgorithmCompute = false;
        }

        internal void Init(Model.Model model, Controller.Controller controller, IElementProvider elementProvider)
        {
            this.controller = controller;
            this.model = model;
            this.elementProvider = elementProvider;

            this.Graph = new Graph(model);
            this.Graph.DrawGraph += (sender, args) => this.DrawGraph();
            this.Graph.ElementAdded += (sender, args) => this.ElementAdded?.Invoke(this, args);
            this.Graph.AddNewVertexControl += (sender, args) => this.AddNewVertexControl(args.DataVert);
            this.Graph.AddNewEdgeControl += (sender, args) => this.AddNewEdgeControl(args.EdgeViewModel);

            this.InitGraphXLogicCore();
        }

        public void Clear() => this.Graph.DataGraph.Clear();

        public void Reload() => this.Graph.InitModel(this.model.ModelName);

        // TODO: Selecting shall be done on actual IElement reference.
        // TODO: It seems to be non-working anyway.
        public void SelectEdge(string source, string target)
        {
            for (var i = 0; i < this.Graph.DataGraph.Edges.Count(); i++)
            {
                if (this.Graph.DataGraph.Edges.ToList()[i].Source.Name == source &&
                    this.Graph.DataGraph.Edges.ToList()[i].Target.Name == target)
                {
                    var edge = this.Graph.DataGraph.Edges.ToList()[i];
                    foreach (var ed in this.scene.EdgesList)
                    {
                        if (ed.Key == edge)
                        {
                            HighlightBehaviour.SetIsHighlightEnabled(ed.Value, true);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        public void SelectNode(string name)
        {
            for (var i = 0; i < this.Graph.DataGraph.Vertices.Count(); i++)
            {
                if (this.Graph.DataGraph.Vertices.ToList()[i].Name == name)
                {
                    var vertex = this.Graph.DataGraph.Vertices.ToList()[i];
                    this.NodeSelected?.Invoke(this, new NodeSelectedEventArgs {Node = vertex});
                    foreach (var ed in this.scene.VertexList)
                    {
                        if (ed.Key == vertex)
                        {
                            HighlightBehaviour.SetIsHighlightEnabled(ed.Value, true);
                        }
                    }

                    break;
                }
            }
        }   

        public void ChangeEdgeLabel(string newLabel)
        {
            if (this.ctrlEdg != null)
            {
                var data = ctrlEdg.GetDataEdge<EdgeViewModel>();
                data.Text = newLabel;
                scene.GenerateAllEdges();
            }
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.prevVer = null;
            this.ctrlVer = null;
            this.scene.GetAllVertexControls().ToList().ForEach(
                x => x.GetDataVertex<NodeViewModel>().Color = Brushes.Green);
        }

        // Need for dropping.
        private void ZoomControl_Drop(object sender, DragEventArgs e)
        {
            this.pos = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.scene);
            this.CreateNewNode((Repo.IElement)e.Data.GetData("REAL.NET palette element"));
            this.ElementUsed?.Invoke(this, EventArgs.Empty);
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            this.ctrlVer = args.VertexControl;
            if (this.elementProvider.Element?.InstanceMetatype == Repo.Metatype.Edge)
            {
                if (this.prevVer == null)
                {
                    this.editorManager.CreateVirtualEdge(this.ctrlVer, this.ctrlVer.GetPosition());
                    this.prevVer = this.ctrlVer;
                }
                else
                {
                    this.controller.NewEdge(this.elementProvider.Element, this.prevVer, this.ctrlVer);
                    this.ElementUsed?.Invoke(this, EventArgs.Empty);
                }

                return;
            }

            this.NodeSelected?.Invoke(this,
                new NodeSelectedEventArgs {Node = this.ctrlVer.GetDataVertex<NodeViewModel>()});

            this.scene.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<NodeViewModel>().
                Color = Brushes.Green);

            this.ctrlVer.GetDataVertex<NodeViewModel>().Color = Brushes.LightBlue;
            if (this.prevVer != null)
            {
                this.prevVer.GetDataVertex<NodeViewModel>().Color = Brushes.Yellow;
            }

            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.VertexControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.VertexControl };
                mi.Click += this.MenuItemClickVert;
                args.VertexControl.ContextMenu.Items.Add(mi);
                args.VertexControl.ContextMenu.IsOpen = true;
            }
        }

        private void EdgeSelectedAction(object sender, GraphX.Controls.Models.EdgeSelectedEventArgs args)
        {
            this.ctrlEdg = args.EdgeControl;

            this.ctrlEdg.PreviewMouseUp += this.OnEdgeMouseUp;
            this.zoomControl.MouseMove += this.OnEdgeMouseMove;

            this.EdgeSelected?.Invoke(this,
                new EdgeSelectedEventArgs { Edge = this.ctrlEdg.GetDataEdge<EdgeViewModel>() });
            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.EdgeControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.EdgeControl };
                mi.Click += this.MenuItemClickEdge;
                args.EdgeControl.ContextMenu.Items.Add(mi);
                args.EdgeControl.ContextMenu.IsOpen = true;
            }
        }

        private void AddNewVertexControl(NodeViewModel vertex)
        {
            var vc = new VertexControl(vertex);
            vc.SetPosition(this.pos);
            this.scene.AddVertex(vertex, vc);
        }

        private void AddNewEdgeControl(EdgeViewModel edgeViewModel)
        {
            var ec = new EdgeControl(this.prevVer, this.ctrlVer, edgeViewModel);
            this.scene.InsertEdge(edgeViewModel, ec);
            this.prevVer = null;
            this.editorManager.DestroyVirtualEdge();
        }

        private void MenuItemClickVert(object sender, EventArgs e)
        {
            this.Graph.DataGraph.RemoveVertex(this.ctrlVer.GetDataVertex<NodeViewModel>());
            this.DrawGraph();
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            this.Graph.DataGraph.RemoveEdge(this.ctrlEdg.GetDataEdge<EdgeViewModel>());
            this.DrawGraph();
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.zoomControl.MouseMove -= this.OnEdgeMouseMove;
            this.ctrlEdg.PreviewMouseUp -= this.OnEdgeMouseUp;
        }

        private void DrawGraph()
        {
            this.scene.GenerateGraph(this.Graph.DataGraph);
            this.zoomControl.ZoomToFill();
        }

        private void OnEdgeMouseMove(object sender, MouseEventArgs e)
        {
            var dataEdge = this.ctrlEdg.GetDataEdge<EdgeViewModel>();
            if (dataEdge == null)
            {
                return;
            }

            if (dataEdge.RoutingPoints == null)
            {
                dataEdge.RoutingPoints = new GraphX.Measure.Point[3];
            }

            dataEdge.RoutingPoints[0] = new GraphX.Measure.Point(100, 100);
            var mousePosition = Mouse.GetPosition(this.scene);
            dataEdge.RoutingPoints[1] = new GraphX.Measure.Point(mousePosition.X, mousePosition.Y);
            dataEdge.RoutingPoints[2] = new GraphX.Measure.Point(100, 100);
            this.scene.UpdateAllEdges();
        }

        private void OnSceneMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.scene);
                if (this.elementProvider.Element?.InstanceMetatype == Repo.Metatype.Node)
                {
                    this.pos = position;
                    this.CreateNewNode(this.elementProvider.Element);
                    this.ElementUsed?.Invoke(this, EventArgs.Empty);
                }

                if (this.elementProvider.Element?.InstanceMetatype == Repo.Metatype.Edge)
                {
                    if (this.prevVer != null)
                    {
                        this.prevVer = null;
                        this.editorManager.DestroyVirtualEdge();
                        this.ElementUsed?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void CreateNewNode(Repo.IElement element)
        {
            this.controller.NewNode(element, this.model.ModelName);
        }

        public class NodeSelectedEventArgs : EventArgs
        {
            public NodeViewModel Node { get; set; }
        }

        public class EdgeSelectedEventArgs : EventArgs
        {
            public EdgeViewModel Edge { get; set; }
        }
    }
}
