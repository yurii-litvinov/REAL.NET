using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using GraphX.Controls;
using GraphX.Controls.Models;
using QuickGraph;
using System.Windows.Media;

namespace EditorPrototype
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private VertexControl prevVer;
        private VertexControl ctrlVer;
        private EdgeControl ctrlEdg;
        private GraphExample dataGraph;

        private Repo.Repo repo = Repo.RepoFactory.CreateRepo();

        public MainWindow()
        {
            InitializeComponent();
            dataGraph = new GraphExample();
            var logic = new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>();
            g_Area.LogicCore = logic;
            logic.Graph = dataGraph;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog;
            g_Area.VertexSelected += VertexSelectedAction;
            g_Area.EdgeSelected += EdgeSelectedAction;
            g_zoomctrl.Click += ClearSelection;
            elementsListBox.MouseDoubleClick += ElementInBoxSelectedAction;

            ZoomControl.SetViewFinderVisibility(g_zoomctrl, Visibility.Visible);
            g_zoomctrl.Loaded += (sender, args) =>
            {
                (g_zoomctrl.ViewFinder.Parent as Grid).Children.Remove(g_zoomctrl.ViewFinder);
                rightPanel.Children.Add(g_zoomctrl.ViewFinder);
                Grid.SetRow(g_zoomctrl.ViewFinder, 0);
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

            Closed += CloseChildrenWindows;

            InitPalette();

            InitModel();
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            prevVer = null;
            ctrlVer = null;
            g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);
        }

        private void InitModel()
        {
            foreach (var node in repo.ModelNodes())
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

                CreateNode(node.name, nodeType(node.nodeType));
            }

            foreach (var edge in repo.ModelEdges())
            {
                var source = dataGraph.Vertices.First(v => v.Name == edge.source);
                var target = dataGraph.Vertices.First(v => v.Name == edge.target);

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

                var newEdge = new DataEdge(source, target) { EdgeType = edgeType(edge.edgeType) };
                dataGraph.AddEdge(newEdge);
            }

            DrawGraph();
        }

        private void InitPalette()
        {
            foreach (var type in repo.MetamodelNodes())
            {
                var button = new Button { Content = type };
                //RoutedEventHandler createNode = (sender, args) => CreateNode(type);
                //RoutedEventHandler createEdge = (sender, args) => CreateEdge(type);
                //button.Click += Repo.Repo.IsEdge(type) ? createEdge : createNode;
                
                // TODO: Bind it to XAML, do not do GUI work in C#.
                paletteGrid.RowDefinitions.Add(new RowDefinition());  
                paletteGrid.Children.Add(button);
                Grid.SetRow(button, paletteGrid.RowDefinitions.Count - 1);
            }
        }

        private void CreateEdge(string type)
        {
            var prevVerVertex = prevVer?.Vertex as DataVertex;
            var ctrlVerVertex = ctrlVer?.Vertex as DataVertex;
            if (prevVerVertex == null || ctrlVerVertex == null)
            {
                return;
            }

            var newEdge = new DataEdge(prevVerVertex, ctrlVerVertex) { Text = type };
            dataGraph.AddEdge(newEdge);
            DrawNewEdge(prevVerVertex.Key, ctrlVerVertex.Key);
        }

        private void CreateNode(string name, DataVertex.VertexTypeEnum type)
        {
            var vertex = new DataVertex(name)
            {
                Key = $"{name}",
                VertexType = type
            };

            dataGraph.AddVertex(vertex);
            DrawNewVertex(vertex.Key);
        }

        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            StackPanel sp = (elementsListBox.SelectedItem as ListBoxItem).Content as StackPanel;
            if(sp.Children.Count > 3)
            {
                var source = (sp.Children[2] as TextBlock).Text;
                var target = (sp.Children[4] as TextBlock).Text;
                for (int i = 0; i < dataGraph.Edges.Count(); i++)
                {
                    if (dataGraph.Edges.ToList()[i].Source.Name == source &&
                        dataGraph.Edges.ToList()[i].Target.Name == target)
                    {
                        var edge = dataGraph.Edges.ToList()[i];
                        elementsTextBlock.Text = "Type: Edge\n\rText: " + edge.Text 
                            + "\n\rSource: " + edge.Source.Name + "\n\rTarget: " + edge.Target.Name;
                        foreach (KeyValuePair<DataEdge, EdgeControl> ed in g_Area.EdgesList)
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
                for (int i = 0; i < dataGraph.Vertices.Count(); i++)
                {
                    if (dataGraph.Vertices.ToList()[i].Name == name)
                    {
                        var vertex = dataGraph.Vertices.ToList()[i];
                        elementsTextBlock.Text = "Type: Vertex\n\rName: " + vertex.Name + "\n\rKey: " 
                            + vertex.Key;
                        foreach (KeyValuePair<DataVertex, VertexControl> ed in g_Area.VertexList)
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
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png"));
            TextBlock spaces = new TextBlock { Text = "  " };
            TextBlock tx = new TextBlock { Text = vertexName };
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            elementsListBox.Items.Add(lbi);

            DrawGraph();
        }

        private void DrawNewEdge(string source, string target)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Edge.png"));
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
            elementsListBox.Items.Add(lbi);

            DrawGraph();
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            prevVer = ctrlVer;
            ctrlVer = args.VertexControl;
            elementsTextBlock.Text = "Type: Vertex\n\rName: " + ctrlVer.GetDataVertex<DataVertex>().Name 
                + "\n\rKey: " + ctrlVer.GetDataVertex<DataVertex>().Key;

            g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);

            ctrlVer.GetDataVertex<DataVertex>().Color = Brushes.LightBlue;
            if (prevVer != null)
            {
                prevVer.GetDataVertex<DataVertex>().Color = Brushes.Yellow;
            }

            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.VertexControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.VertexControl };
                mi.Click += MenuItemClickVert;
                args.VertexControl.ContextMenu.Items.Add(mi);
                args.VertexControl.ContextMenu.IsOpen = true;
            }
        }

        private void EdgeSelectedAction(object sender, EdgeSelectedEventArgs args)
        {
            ctrlEdg = args.EdgeControl;

            g_zoomctrl.MouseMove += OnEdgeMouseMove;

            // Those crazy russians intercept MouseUp event, so we are forced to use PreviewMouseUp here.
            ctrlEdg.PreviewMouseUp += OnEdgeMouseUp;

            elementsTextBlock.Text = "Type: Edge\n\rText: " + ctrlEdg.GetDataEdge<DataEdge>().Text 
                + "\n\rSource: " + ctrlEdg.GetDataEdge<DataEdge>().Source.Name + "\n\rTarget: " 
                + ctrlEdg.GetDataEdge<DataEdge>().Target.Name;
            
            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.EdgeControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.EdgeControl };
                mi.Click += MenuItemClickEdge;
                args.EdgeControl.ContextMenu.Items.Add(mi);
                args.EdgeControl.ContextMenu.IsOpen = true;
            }
        }

        private void OnEdgeMouseMove(object sender, MouseEventArgs e)
        {
            var dataEdge = ctrlEdg.GetDataEdge<DataEdge>();
            if (dataEdge.RoutingPoints == null)
            {
                dataEdge.RoutingPoints = new GraphX.Measure.Point[3];
            }

            dataEdge.RoutingPoints[0] = new GraphX.Measure.Point(100, 100);
            var mousePosition = Mouse.GetPosition(g_Area);
            dataEdge.RoutingPoints[1] = new GraphX.Measure.Point(mousePosition.X, mousePosition.Y);
            dataEdge.RoutingPoints[2] = new GraphX.Measure.Point(100, 100);

            g_Area.UpdateAllEdges();
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            g_zoomctrl.MouseMove -= OnEdgeMouseMove;
            ctrlEdg.PreviewMouseUp -= OnEdgeMouseUp;
        }

        private void MenuItemClickVert(object sender, EventArgs e)
        {
            dataGraph.RemoveVertex(ctrlVer.GetDataVertex<DataVertex>());
            DrawGraph();
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            dataGraph.RemoveEdge(ctrlEdg.GetDataEdge<DataEdge>());
            DrawGraph();
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
            g_Area.GenerateGraph(dataGraph);
            g_zoomctrl.ZoomToFill();
        }
    }
}
