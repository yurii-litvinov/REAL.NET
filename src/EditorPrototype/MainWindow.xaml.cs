﻿namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using GraphX.Controls;
    using GraphX.Controls.Models;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
    using GraphX.PCL.Logic.Models;
    using QuickGraph;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private VertexControl prevVer;
        private VertexControl ctrlVer;
        private EdgeControl ctrlEdg;
        private GraphExample dataGraph;

        private Repo.IRepo repo = Repo.RepoFactory.CreateRepo();

        public MainWindow()
        {
            this.InitializeComponent();
            this.dataGraph = new GraphExample();
            var logic = new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>();
            this.g_Area.LogicCore = logic;
            logic.Graph = this.dataGraph;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog;
            this.g_Area.VertexSelected += this.VertexSelectedAction;
            this.g_Area.EdgeSelected += this.EdgeSelectedAction;
            this.g_zoomctrl.Click += this.ClearSelection;
            this.elementsListBox.MouseDoubleClick += this.ElementInBoxSelectedAction;

            ZoomControl.SetViewFinderVisibility(this.g_zoomctrl, Visibility.Visible);
            this.g_zoomctrl.Loaded += (sender, args) =>
            {
                (this.g_zoomctrl.ViewFinder.Parent as Grid).Children.Remove(this.g_zoomctrl.ViewFinder);
                this.rightPanel.Children.Add(this.g_zoomctrl.ViewFinder);
                Grid.SetRow(this.g_zoomctrl.ViewFinder, 0);
            };

            logic.DefaultLayoutAlgorithmParams =
                logic.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.LinLog);
            logic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams =
                logic.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            logic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            logic.AsyncAlgorithmCompute = false;

            this.Closed += this.CloseChildrenWindows;

            this.InitPalette();

            this.InitModel();
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.prevVer = null;
            this.ctrlVer = null;
            this.g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);
        }

        private void InitModel()
        {
            foreach (var node in this.repo.ModelNodes())
            {
                Func<Repo.NodeType, DataVertex.VertexTypeEnum> nodeType = n =>
                {
                    switch (n)
                    {
                        case Repo.NodeType.Attribute:
                            return DataVertex.VertexTypeEnum.Attribute;
                        case Repo.NodeType.Node:
                            return DataVertex.VertexTypeEnum.Node;
                    }

                    return DataVertex.VertexTypeEnum.Node;
                };

                this.CreateNode(node.name, nodeType(node.nodeType), node.attributes);
            }

            foreach (var edge in this.repo.ModelEdges())
            {
                var isViolation = Constraints.CheckEdge(edge, this.repo);
                var source = this.dataGraph.Vertices.First(v => v.Name == edge.source);
                var target = this.dataGraph.Vertices.First(v => v.Name == edge.target);

                Func<Repo.EdgeType, DataEdge.EdgeTypeEnum> edgeType = e =>
                {
                    switch (e)
                    {
                        case Repo.EdgeType.Generalization:
                            return DataEdge.EdgeTypeEnum.Generalization;
                        case Repo.EdgeType.Association:
                            return DataEdge.EdgeTypeEnum.Association;
                        case Repo.EdgeType.Attribute:
                            return DataEdge.EdgeTypeEnum.Attribute;
                        case Repo.EdgeType.Type:
                            return DataEdge.EdgeTypeEnum.Type;
                    }

                    return DataEdge.EdgeTypeEnum.Generalization;
                };

                var newEdge = new DataEdge(source, target, isViolation) { EdgeType = edgeType(edge.edgeType) };
                this.dataGraph.AddEdge(newEdge);
            }

            this.DrawGraph();
        }

        private void InitPalette()
        {
            foreach (var type in this.repo.MetamodelNodes())
            {
                var button = new Button { Content = type.name };
                RoutedEventHandler createNode = (sender, args) => this.CreateNewNode(type.id);
                RoutedEventHandler createEdge = (sender, args) => { };
                button.Click += this.repo.IsEdgeClass(type.id) ? createEdge : createNode;

                // TODO: Bind it to XAML, do not do GUI work in C#.
                this.paletteGrid.RowDefinitions.Add(new RowDefinition());
                this.paletteGrid.Children.Add(button);
                Grid.SetRow(button, this.paletteGrid.RowDefinitions.Count - 1);
            }
        }

        private void CreateEdge(string type)
        {
            var prevVerVertex = this.prevVer?.Vertex as DataVertex;
            var ctrlVerVertex = this.ctrlVer?.Vertex as DataVertex;
            if (prevVerVertex == null || ctrlVerVertex == null)
            {
                return;
            }

            var newEdge = new DataEdge(prevVerVertex, ctrlVerVertex, true) { Text = type };
            this.dataGraph.AddEdge(newEdge);
            this.DrawNewEdge(prevVerVertex.Key, ctrlVerVertex.Key);
        }

        private void CreateNode(string name, DataVertex.VertexTypeEnum type, IList<Repo.AttributeInfo> attributes)
        {
            var vertex = new DataVertex(name)
            {
                Key = $"{name}",
                VertexType = type
            };

            var attributeInfos = attributes.Select(x => new DataVertex.Attribute()
            {
                Name = x.name,
                Type = this.repo.Node(x.attributeType).name,
                Value = x.value
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));

            this.dataGraph.AddVertex(vertex);
            this.DrawNewVertex(vertex.Key);
        }

        private void CreateNewNode(string typeId)
        {
            var newNode = this.repo.AddNode(typeId);
            Func<Repo.NodeType, DataVertex.VertexTypeEnum> nodeType = n =>
            {
                switch (n)
                {
                    case Repo.NodeType.Attribute:
                        return DataVertex.VertexTypeEnum.Attribute;
                    case Repo.NodeType.Node:
                        return DataVertex.VertexTypeEnum.Node;
                }

                return DataVertex.VertexTypeEnum.Node;
            };

            this.CreateNode(newNode.name, nodeType(newNode.nodeType), newNode.attributes);
        }

        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            StackPanel sp = (this.elementsListBox.SelectedItem as ListBoxItem).Content as StackPanel;
            if (sp.Children.Count > 3)
            {
                var source = (sp.Children[2] as TextBlock).Text;
                var target = (sp.Children[4] as TextBlock).Text;
                for (int i = 0; i < this.dataGraph.Edges.Count(); i++)
                {
                    if (this.dataGraph.Edges.ToList()[i].Source.Name == source &&
                        this.dataGraph.Edges.ToList()[i].Target.Name == target)
                    {
                        var edge = this.dataGraph.Edges.ToList()[i];
                        foreach (KeyValuePair<DataEdge, EdgeControl> ed in this.g_Area.EdgesList)
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
            else
            {
                var name = (sp.Children[2] as TextBlock).Text;
                for (int i = 0; i < this.dataGraph.Vertices.Count(); i++)
                {
                    if (this.dataGraph.Vertices.ToList()[i].Name == name)
                    {
                        var vertex = this.dataGraph.Vertices.ToList()[i];
                        foreach (KeyValuePair<DataVertex, VertexControl> ed in this.g_Area.VertexList)
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
        }

        private void DrawNewVertex(string vertexName)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            Image img = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png"))
            };
            TextBlock spaces = new TextBlock { Text = "  " };
            TextBlock tx = new TextBlock { Text = vertexName };
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);

            this.DrawGraph();
        }

        private void DrawNewEdge(string source, string target)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            Image img = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Edge.png"))
            };
            TextBlock spaces = new TextBlock { Text = "  " };
            TextBlock tx0 = new TextBlock { Text = source };
            TextBlock tx1 = new TextBlock { Text = " - " };
            TextBlock tx2 = new TextBlock { Text = target };
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx0);
            sp.Children.Add(tx1);
            sp.Children.Add(tx2);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);

            this.DrawGraph();
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            this.prevVer = this.ctrlVer;
            this.ctrlVer = args.VertexControl;
            this.attributesView.DataContext = this.ctrlVer.GetDataVertex<DataVertex>();

            this.g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);

            this.ctrlVer.GetDataVertex<DataVertex>().Color = Brushes.LightBlue;
            if (this.prevVer != null)
            {
                this.prevVer.GetDataVertex<DataVertex>().Color = Brushes.Yellow;
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

        private void EdgeSelectedAction(object sender, EdgeSelectedEventArgs args)
        {
            this.ctrlEdg = args.EdgeControl;

            this.g_zoomctrl.MouseMove += this.OnEdgeMouseMove;

            // Those crazy russians intercept MouseUp event, so we are forced to use PreviewMouseUp here.
            this.ctrlEdg.PreviewMouseUp += this.OnEdgeMouseUp;

            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.EdgeControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.EdgeControl };
                mi.Click += this.MenuItemClickEdge;
                args.EdgeControl.ContextMenu.Items.Add(mi);
                args.EdgeControl.ContextMenu.IsOpen = true;
            }
        }

        private void OnEdgeMouseMove(object sender, MouseEventArgs e)
        {
            var dataEdge = this.ctrlEdg.GetDataEdge<DataEdge>();
            if (dataEdge.RoutingPoints == null)
            {
                dataEdge.RoutingPoints = new GraphX.Measure.Point[3];
            }

            dataEdge.RoutingPoints[0] = new GraphX.Measure.Point(100, 100);
            var mousePosition = Mouse.GetPosition(this.g_Area);
            dataEdge.RoutingPoints[1] = new GraphX.Measure.Point(mousePosition.X, mousePosition.Y);
            dataEdge.RoutingPoints[2] = new GraphX.Measure.Point(100, 100);

            this.g_Area.UpdateAllEdges();
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.g_zoomctrl.MouseMove -= this.OnEdgeMouseMove;
            this.ctrlEdg.PreviewMouseUp -= this.OnEdgeMouseUp;
        }

        private void MenuItemClickVert(object sender, EventArgs e)
        {
            this.dataGraph.RemoveVertex(this.ctrlVer.GetDataVertex<DataVertex>());
            this.DrawGraph();
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            this.dataGraph.RemoveEdge(this.ctrlEdg.GetDataEdge<DataEdge>());
            this.DrawGraph();
        }

        private void CloseChildrenWindows(object sender, EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                w.Close();
            }
        }

        private void DrawGraph()
        {
            this.g_Area.GenerateGraph(this.dataGraph);
            this.g_zoomctrl.ZoomToFill();
        }
    }
}
