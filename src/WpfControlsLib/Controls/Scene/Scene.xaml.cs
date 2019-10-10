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
    using System.Collections.Generic;
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
            this.graphArea.VertexMouseUp += (sender, args) => this.CheckThatForReconnection(args);
            this.InitGraphXLogicCore();
        }

        private void CheckThatForReconnection(VertexSelectedEventArgs args)
        {
            var vertexToCheck = args.VertexControl;
            if (IsVertexVirtual(vertexToCheck))
            {
                var vertices = this.graphArea.VertexList.Select(x => x.Value).Where(x => !IsVertexVirtual(x));
                var verticesToConnect = vertices.Where(x => IsCenterOfFirstVertexOnAnother(vertexToCheck, x));
                if (verticesToConnect.Count() != 0)
                {
                    var vertexToConnect = verticesToConnect.First();
                    ReconnectEdgeFromVirtualNodeToReal(vertexToCheck, vertexToConnect);
                }
            }
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
                    this.NodeSelected?.Invoke(this, new NodeSelectedEventArgs { Node = vertex });
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

        // removed for now
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
                if (IsVertexVirtual(currentVertex))
                {
                    return;
                }
                if (this.previosVertex == null)
                {
                    this.editorManager.CreateVirtualEdge(this.currentVertex, this.currentVertex.GetPosition());
                    this.previosVertex = this.currentVertex;
                }
                else if (this.previosVertex.GetDataVertex<NodeViewModel>().IsVirtual)
                {
                    var virtualEdgeData = new EdgeViewModel(previosVertex.GetDataVertex<NodeViewModel>(), currentVertex.GetDataVertex<NodeViewModel>());
                    var virtualEdgeControl = new EdgeControl(previosVertex, currentVertex, virtualEdgeData);
                    this.graphArea.AddEdge(virtualEdgeData, virtualEdgeControl);
                    this.editorManager.DestroyVirtualEdge();
                    this.previosVertex = null;
                    this.ElementManipulationDone?.Invoke(this, EventArgs.Empty);
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
                if (IsVertexVirtual(args.VertexControl))
                {
                    mi.Click += (senderObj, eventArgs) => this.MenuItemClickedOnVirtualVertex(args.VertexControl, EventArgs.Empty);
                }
                else
                {
                    mi.Click += this.MenuItemClickedOnVertex;
                }
                args.VertexControl.ContextMenu.Items.Add(mi);
                args.VertexControl.ContextMenu.IsOpen = true;
            }
        }

        private bool IsVertexVirtual(VertexControl vertexControl) => vertexControl.GetDataVertex<NodeViewModel>().IsVirtual;

        private bool IsVertexHasEdges(VertexControl vertexControl)
        {
            var edges = this.graphArea.EdgesList.Select(x => x.Value)
                .Where(x => x.Target == vertexControl || x.Source == vertexControl);
            return edges.Count() > 0;
        }

        private bool IsEdgeSourceVirtual(EdgeControl edgeControl) => IsVertexVirtual(edgeControl.Source);

        private bool IsEdgeTargetVirtual(EdgeControl edgeControl) => IsVertexVirtual(edgeControl.Target);

        private bool IsCenterOfFirstVertexOnAnother(VertexControl vertex1, VertexControl vertex2)
        {
            Point center = this.graphArea.TranslatePoint(vertex1.GetCenterPosition(), vertex1);
            Point coordinates = vertex1.TranslatePoint(center, vertex2);
            HitTestResult hit = VisualTreeHelper.HitTest(vertex2, coordinates);
            return hit != null;
        }

        private void ReconnectEdgeFromVirtualNodeToReal(VertexControl virtualVertex, VertexControl vertexToConnect)
        {
            var dataNode = virtualVertex.GetDataVertex<NodeViewModel>();
            var edgeControls = this.graphArea.EdgesList.Select(x => x.Value);
            var edgesWhereSourceStack = new Stack<EdgeControl>(edgeControls.Where(x => x.Source == virtualVertex));
            var edgesWhereTargetStack = new Stack<EdgeControl>(edgeControls.Where(x => x.Target == virtualVertex));
            while (edgesWhereSourceStack.Count != 0)
            {
                var edge = edgesWhereSourceStack.Pop();
                if (IsEdgeTargetVirtual(edge))
                {
                    var source = edge.Source;
                    edge.Source = vertexToConnect;
                    this.graphArea.RemoveVertex(source.GetDataVertex<NodeViewModel>());
                }
                else
                {
                    // add to model with rooting points? not done yet
                    var source = edge.Source;
                    edge.Source = vertexToConnect;
                    this.graphArea.RemoveVertex(source.GetDataVertex<NodeViewModel>());
                }
            }
            while (edgesWhereTargetStack.Count != 0)
            {
                var edge = edgesWhereTargetStack.Pop();
                if (IsEdgeSourceVirtual(edge))
                {
                    var target = edge.Target;
                    edge.Target = vertexToConnect;
                    this.graphArea.RemoveVertex(target.GetDataVertex<NodeViewModel>());
                }
                else
                {
                    // add to model ? nor done yet
                    var target = edge.Target;
                    edge.Source = vertexToConnect;
                    this.graphArea.RemoveVertex(target.GetDataVertex<NodeViewModel>());
                    ; }
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
                var edgeControl = args.EdgeControl;
                edgeControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = edgeControl };
                if (edgeControl.Source.GetDataVertex<NodeViewModel>().IsVirtual || edgeControl.Target.GetDataVertex<NodeViewModel>().IsVirtual)
                {
                    // TODO: real delete with virtual edge
                    mi.Click += (senderObj, eventArgs) => MenuItemDeleteClickedOnVirtualEdge(edgeControl, EventArgs.Empty);
                }
                else
                {
                    mi.Click += this.MenuItemClickEdge;
                    var miSource = new MenuItem { Header = "Unpin Source", Tag = edgeControl };
                    miSource.Click += (senderObj, eventArgs) => UnpinEdgeFromVertex(edgeControl, true);
                    var miTarget = new MenuItem { Header = "Unpin Target", Tag = edgeControl };
                    miTarget.Click += (senderObj, eventArgs) => UnpinEdgeFromVertex(edgeControl, false);
                    edgeControl.ContextMenu.Items.Add(miSource);
                    edgeControl.ContextMenu.Items.Add(miTarget);
                }

                edgeControl.ContextMenu.Items.Add(mi);
                edgeControl.ContextMenu.IsOpen = true;

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
                    edgeControl.ContextMenu.Items.Add(mi2);
                }
            }
        }

        
        private void UnpinEdgeFromVertex(EdgeControl edgeControl, bool isSource)
        {
            // to do : real remove form model
            var vertexData = new NodeViewModel()
            {
                IsVirtual = true
            };
            var virtualVertex = new VertexControl(vertexData);
            this.graphArea.AddVertex(vertexData, virtualVertex);
            if (isSource)
            {
                var source = edgeControl.Source;                
                virtualVertex.SetPosition(source.GetCenterPosition());
                edgeControl.Source = virtualVertex;
            }
            else
            {
                var target = edgeControl.Target;
                virtualVertex.SetPosition(target.GetCenterPosition());
                edgeControl.Target = virtualVertex;
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

        public VertexControl AddVirtualVertexControl(NodeViewModel innerNodeData)
        {
            innerNodeData.IsVirtual = true;
            var virtualControl = new VertexControl(innerNodeData);
            this.position = this.zoomControl.TranslatePoint(Mouse.GetPosition(this.zoomControl), this.graphArea);
            virtualControl.SetPosition(this.position);
            this.graphArea.AddVertex(innerNodeData, virtualControl);
            return virtualControl;
        }

        private void AddNewEdgeControl(EdgeViewModel edgeViewModel)
        {
            var ec = new EdgeControl(this.previosVertex, this.currentVertex, edgeViewModel);
            this.graphArea.InsertEdge(edgeViewModel, ec);
            this.previosVertex = null;
            this.editorManager.DestroyVirtualEdge();
        }

        private void MenuItemDeleteClickedOnVirtualEdge(object sender, EventArgs args)
        {
            var edgeControl = sender as EdgeControl;
            var source = edgeControl.Source;
            var target = edgeControl.Target;
            this.graphArea.RemoveEdge(edgeControl.GetDataEdge<EdgeViewModel>());
            if (IsVertexVirtual(source))
            {
                this.graphArea.RemoveVertex(source.GetDataVertex<NodeViewModel>());
            }
            if (IsVertexVirtual(target))
            {
                this.graphArea.RemoveVertex(target.GetDataVertex<NodeViewModel>());
            }
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


        private void MenuItemClickedOnVirtualVertex(object sender, EventArgs args)
        {
            var vertex = sender as VertexControl;
            // to do working with model if necessary
            var edges = this.graphArea.EdgesList.Select(x => x.Value)
                .Where(x => x.Target == vertex || x.Source == vertex);
            var deleteEdgesCommand = new CompositeCommand();
            foreach (var edge in edges)
            {
                var anotherVertex = edge.Target == vertex ? edge.Source : edge.Target;
                void removingEdge() => this.graphArea.RemoveEdge(edge.GetDataEdge<EdgeViewModel>());
                void removingAnotherVertex() 
                {
                    if (IsVertexVirtual(anotherVertex))
                    {
                        this.graphArea.RemoveVertex(anotherVertex.GetDataVertex<NodeViewModel>());
                    }
                }
                var command1 = new Toolbar.Command(removingEdge);
                var command2 = new Toolbar.Command(removingAnotherVertex);
                deleteEdgesCommand.Add(command1);
                deleteEdgesCommand.Add(command2);
            }
            (deleteEdgesCommand as EditorPluginInterfaces.ICommand).Execute();
            this.graphArea.RemoveVertex(vertex.GetDataVertex<NodeViewModel>());
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
                    var vertexData = new NodeViewModel();
                    var virtualVertex = this.AddVirtualVertexControl(vertexData);
                    if (this.previosVertex != null)
                    {
                        var virtualEdgeData = new EdgeViewModel(previosVertex.GetDataVertex<NodeViewModel>(), virtualVertex.GetDataVertex<NodeViewModel>());
                        var virtualEdgeControl = new EdgeControl(previosVertex, virtualVertex, virtualEdgeData);
                        this.graphArea.AddEdge(virtualEdgeData, virtualEdgeControl);
                        this.editorManager.DestroyVirtualEdge();
                        this.previosVertex = null;
                        this.ElementManipulationDone?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        this.editorManager.CreateVirtualEdge(virtualVertex, virtualVertex.GetPosition());
                        this.previosVertex = virtualVertex;
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
