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

namespace WpfControlsLib.Controls.Scene
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using GraphX.Controls;
    using GraphX.Controls.Models;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
    using GraphX.PCL.Logic.Models;
    using QuickGraph;
    using WpfControlsLib.Controller;
    using WpfControlsLib.Controls.Scene.Commands;
    using WpfControlsLib.Controls.Scene.EventArguments;
    using WpfControlsLib.Model;
    using WpfControlsLib.ViewModel;

    /// <summary>
    /// Visualizes  model graph and makes it possible for a user to interact with it.
    /// </summary>
    public partial class Scene : UserControl
    {
        private readonly EditorObjectManager editorManager;
        private VertexControl previosVertex;
        private VertexControl currentVertex;
        private EdgeControl edgeControl;
        private Point position;

        private Model model;
        private Controller controller;
        private IElementProvider elementProvider;

        public Scene()
        {
            this.InitializeComponent();

            this.editorManager = new EditorObjectManager(this.graphArea, this.zoomControl);

            ZoomControl.SetViewFinderVisibility(this.zoomControl, Visibility.Hidden);

            this.graphArea.VertexSelected += this.VertexSelectedAction;
            this.graphArea.EdgeSelected += this.EdgeSelectedAction;
            this.zoomControl.Click += this.ClearSelection;
            this.zoomControl.MouseDown += this.OnSceneMouseDown;
            this.zoomControl.Drop += this.ZoomControlDrop;
        }

        public event EventHandler<EventArgs> ElementManipulationDone;

        public event EventHandler<NodeSelectedEventArgs> NodeSelected;

        public event EventHandler<EventArguments.EdgeSelectedEventArgs> EdgeSelected;

        public event EventHandler<ElementAddedEventArgs> ElementAdded;

        public event EventHandler<ElementRemovedEventArgs> ElementRemoved;

        public Graph Graph { get; set; }

        private void InitGraphXLogicCore()
        {
            var logic =
                new GXLogicCore<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>
                {
                    Graph = this.Graph.DataGraph,
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog,
                };

            this.graphArea.LogicCore = logic;

            logic.DefaultLayoutAlgorithmParams =
                logic.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.LinLog);
            logic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams =
                logic.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            logic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            logic.AsyncAlgorithmCompute = false;
            logic.EdgeCurvingEnabled = false;
        }

        public void Init(Model model, Controller controller, IElementProvider elementProvider)
        {
            this.controller = controller;
            this.model = model;
            this.elementProvider = elementProvider;
            this.Graph = new Graph(model);
            this.Graph.DrawGraph += (sender, args) => this.DrawGraph();
            this.Graph.ElementAdded += (sender, args) => this.ElementAdded?.Invoke(this, args);
            this.Graph.ElementRemoved += (sender, args) => this.ElementRemoved?.Invoke(this, args);
            this.Graph.AddNewVertexControl += (sender, args) => this.AddNewVertexControl(args.DataVertex);
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
                    foreach (var ed in this.graphArea.EdgesList)
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
                    foreach (var ed in this.graphArea.VertexList)
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
            if (this.edgeControl != null)
            {
                var data = this.edgeControl.GetDataEdge<EdgeViewModel>();
                data.Text = newLabel;
                this.graphArea.GenerateAllEdges();
            }
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.previosVertex = null;
            this.currentVertex = null;
            this.graphArea.GetAllVertexControls().ToList().ForEach(
                x => x.GetDataVertex<NodeViewModel>().Color = Brushes.Green);
        }

        /// <summary>
        /// Handles drag and drop event.
        /// Notice: drag and drop event does not work with edges yet.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Drag event arguments.</param>
        private void ZoomControlDrop(object sender, DragEventArgs e)
        {
            var element = e.Data.GetData("REAL.NET palette element") as Repo.IElement;

            if (element.Metatype == Repo.Metatype.Node)
            {
                this.position = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.graphArea);
                this.CreateNewNode(element);
                this.ElementManipulationDone?.Invoke(this, EventArgs.Empty);
            }
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            this.currentVertex = args.VertexControl;
            if (this.elementProvider.Element?.InstanceMetatype == Repo.Metatype.Edge)
            {
                if (this.previosVertex == null)
                {
                    this.editorManager.CreateVirtualEdge(this.currentVertex, this.currentVertex.GetPosition());
                    this.previosVertex = this.currentVertex;
                }
                else
                {
                    var command = new Commands.CreateEdgeCommand(
                        this.model,
                        this.elementProvider.Element,
                        (this.previosVertex?.Vertex as NodeViewModel)?.Node,
                        (this.currentVertex?.Vertex as NodeViewModel)?.Node);
                    this.controller.Execute(command);
                    this.ElementManipulationDone?.Invoke(this, EventArgs.Empty);
                }

                return;
            }

            this.NodeSelected?.Invoke(
                this,
                new NodeSelectedEventArgs { Node = this.currentVertex.GetDataVertex<NodeViewModel>() });

            this.graphArea.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<NodeViewModel>().
                Color = Brushes.Green);

            this.currentVertex.GetDataVertex<NodeViewModel>().Color = Brushes.LightBlue;
            if (this.previosVertex != null)
            {
                this.previosVertex.GetDataVertex<NodeViewModel>().Color = Brushes.Yellow;
            }

            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.VertexControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.VertexControl };
                mi.Click += this.MenuItemClickedOnVertex;
                args.VertexControl.ContextMenu.Items.Add(mi);
                args.VertexControl.ContextMenu.IsOpen = true;
            }
        }

        private void EdgeSelectedAction(object sender, GraphX.Controls.Models.EdgeSelectedEventArgs args)
        {
            this.edgeControl = args.EdgeControl;

            this.edgeControl.PreviewMouseUp += this.OnEdgeMouseUp;
            this.zoomControl.MouseMove += this.OnEdgeMouseMove;

            this.EdgeSelected?.Invoke(this,
                new EventArguments.EdgeSelectedEventArgs { Edge = this.edgeControl.GetDataEdge<EdgeViewModel>() });

            var dataEdge = this.edgeControl.GetDataEdge<EdgeViewModel>();
            var mousePosition = args.MouseArgs.GetPosition(this.graphArea).ToGraphX();

            // Adding new routing point.
            if (args.MouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                this.HandleRoutingPoints(dataEdge, mousePosition);
            }

            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                this.zoomControl.MouseMove -= this.OnEdgeMouseMove;
                args.EdgeControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.EdgeControl };
                mi.Click += this.MenuItemClickEdge;
                args.EdgeControl.ContextMenu.Items.Add(mi);
                args.EdgeControl.ContextMenu.IsOpen = true;
                if (dataEdge.RoutingPoints == null)
                {
                    return;
                }

                var isRoutingPoint = dataEdge.RoutingPoints.Where(point => Geometry.GetDistance(point, mousePosition).CompareTo(3) <= 0).ToArray().Length != 0;

                // Adding MenuItem to routing point in order to delete it.
                if (isRoutingPoint)
                {
                    var routingPoint = Array.Find(dataEdge.RoutingPoints, point => Geometry.GetDistance(point, mousePosition).CompareTo(3) <= 0);
                    var mi2 = new MenuItem { Header = "Delete routing point", Tag = routingPoint };
                    mi2.Click += this.MenuItemClickRoutingPoint;
                    args.EdgeControl.ContextMenu.Items.Add(mi2);
                }
            }
        }

        /// <summary>
        /// Handling of adding new routing point.
        /// </summary>
        /// <param name="edge">Edge.</param>
        /// <param name="mousePosition">Position of mouse.</param>
        private void HandleRoutingPoints(EdgeViewModel edge, GraphX.Measure.Point mousePosition)
        {
            if (edge.RoutingPoints == null)
            {
                var sourcePos = this.edgeControl.Source.GetPosition().ToGraphX();
                var targetPos = this.edgeControl.Target.GetPosition().ToGraphX();

                edge.RoutingPoints = new GraphX.Measure.Point[] { sourcePos, mousePosition, targetPos };
                edge.IndexOfInflectionPoint = 1;
                return;
            }

            var isRoutingPoint = edge.RoutingPoints.Where(point => Geometry.GetDistance(point, mousePosition).CompareTo(3) <= 0).ToArray().Length != 0;

            if (isRoutingPoint)
            {
                edge.IndexOfInflectionPoint = Array.FindIndex(edge.RoutingPoints, point => Geometry.GetDistance(point, mousePosition).CompareTo(3) <= 0);
            }
            else
            {
                var numberOfRoutingPoints = edge.RoutingPoints.Length;

                for (var i = 0; i < numberOfRoutingPoints - 1; ++i)
                {
                    if (Geometry.BelongsToLine(edge.RoutingPoints[i], edge.RoutingPoints[i + 1], mousePosition))
                    {
                        edge.IndexOfInflectionPoint = i + 1;
                        var oldRoutingPoints = edge.RoutingPoints;
                        var newRoutingPoints = new GraphX.Measure.Point[numberOfRoutingPoints + 1];
                        Array.Copy(oldRoutingPoints, 0, newRoutingPoints, 0, i + 1);
                        Array.Copy(oldRoutingPoints, i + 1, newRoutingPoints, i + 2, numberOfRoutingPoints - i - 1);
                        newRoutingPoints[i + 1] = mousePosition;
                        edge.RoutingPoints = newRoutingPoints;
                        return;
                    }
                }
            }
        }

        private void AddNewVertexControl(NodeViewModel vertex)
        {
            var vc = new VertexControl(vertex);
            vc.SetPosition(this.position);
            this.graphArea.AddVertex(vertex, vc);
        }

        private void AddNewEdgeControl(EdgeViewModel edgeViewModel)
        {
            var ec = new EdgeControl(this.previosVertex, this.currentVertex, edgeViewModel);
            this.graphArea.InsertEdge(edgeViewModel, ec);
            this.previosVertex = null;
            this.editorManager.DestroyVirtualEdge();
        }

        private void MenuItemClickRoutingPoint(object sender, EventArgs e)
        {
            var edge = this.edgeControl.GetDataEdge<EdgeViewModel>();
            var newRoutingPoints = edge.RoutingPoints.Where(element => element != (GraphX.Measure.Point)((MenuItem)sender).Tag).ToArray();
            edge.RoutingPoints = newRoutingPoints;
            this.graphArea.UpdateAllEdges();
        }

        private Point[] ConvertIntoWinPoint(GraphX.Measure.Point[] points) => points?.Select(y => new Point(y.X, y.Y)).ToArray();

        private void MenuItemClickedOnVertex(object sender, EventArgs e)
        {
            var vertex = this.currentVertex.GetDataVertex<NodeViewModel>();
            var edges = this.Graph.DataGraph.Edges.ToArray();

            var edgesToRestore = this.graphArea.EdgesList.ToList()
                .Where(x => x.Key.Source == vertex || x.Key.Target == vertex)
                .Select(x => Tuple.Create(x.Key.Edge, this.ConvertIntoWinPoint(x.Key.RoutingPoints)));

            var command = new CompositeCommand();

            foreach (var edge in edges)
            {
                if (edge.Source == vertex || edge.Target == vertex)
                {
                    command.Add(new RemoveEdgeCommand(this.model, edge.Edge));
                }
            }

            command.Add(new RemoveNodeCommand(this.model, vertex.Node));
            this.controller.Execute(command);
            this.DrawGraph();
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            var edge = this.edgeControl.GetDataEdge<EdgeViewModel>();
            var command = new RemoveEdgeCommand(this.model, edge.Edge);
            this.controller.Execute(command);
            this.DrawGraph();
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.zoomControl.MouseMove -= this.OnEdgeMouseMove;
            this.edgeControl.PreviewMouseUp -= this.OnEdgeMouseUp;
        }

        private void DrawGraph()
        {
            this.graphArea.GenerateGraph(this.Graph.DataGraph);
            this.zoomControl.ZoomToFill();
        }

        private void OnEdgeMouseMove(object sender, MouseEventArgs e)
        {
            var dataEdge = this.edgeControl.GetDataEdge<EdgeViewModel>();

            if (dataEdge == null)
            {
                return;
            }

            var mousePosition = Mouse.GetPosition(this.graphArea);
            var index = dataEdge.IndexOfInflectionPoint;
            dataEdge.RoutingPoints[index] = new GraphX.Measure.Point(mousePosition.X, mousePosition.Y);
            this.graphArea.UpdateAllEdges();
        }

        private void OnSceneMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.graphArea);
                if (this.elementProvider.Element?.InstanceMetatype == Repo.Metatype.Node)
                {
                    this.position = position;
                    this.CreateNewNode(this.elementProvider.Element);
                    this.ElementManipulationDone?.Invoke(this, EventArgs.Empty);
                }

                if (this.elementProvider.Element?.InstanceMetatype == Repo.Metatype.Edge)
                {
                    if (this.previosVertex != null)
                    {
                        this.previosVertex = null;
                        this.editorManager.DestroyVirtualEdge();
                        this.ElementManipulationDone?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void CreateNewNode(Repo.IElement element)
        {
            var command = new CreateNodeCommand(this.model, element);
            this.controller.Execute(command);
        }
    }
}
