namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
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
        private readonly EditorObjectManager editorManager;
        private VertexControl prevVer;
        private VertexControl ctrlVer;
        private EdgeControl ctrlEdg;
        private string currentId;
        private Point pos;
        private Model model;
        private Controller controller;
        private Graph graph;

        public MainWindow()
        {
            InitializeComponent();
            this.model = new Model();
            controller = new Controller(this.model);
            this.graph = new Graph(this.model);
            this.graph.DrawGraph += (sender, args) => this.DrawGraph();
            this.graph.DrawNewEdge += (sender, args) => this.DrawNewEdge(args.Source, args.Target);
            this.graph.DrawNewVertex += (sender, args) => this.DrawNewVertex(args.VertName);
            this.graph.AddNewVertexControl += (sender, args) => this.AddNewVertexControl(args.DataVert);
            this.graph.AddNewEdgeControl += (sender, args) => this.AddNewEdgeControl(args.Edge);
            this.editorManager = new EditorObjectManager(g_Area, g_zoomctrl);
            this.currentId = String.Empty;
            var logic = new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>();
            logic.Graph = this.graph.DataGraph;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog;
            g_Area.LogicCore = logic;
            g_Area.VertexSelected += this.VertexSelectedAction;
            g_Area.EdgeSelected += this.EdgeSelectedAction;
            g_zoomctrl.Click += this.ClearSelection;
            elementsListBox.MouseDoubleClick += this.ElementInBoxSelectedAction;
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
            Closed += this.CloseChildrenWindows;
            g_zoomctrl.MouseDown += this.ZoomCtrl_MouseDown;
            this.InitPalette(this.model.ModelName);
            this.graph.InitModel(this.model.ModelName);
        }
        
        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.prevVer = null;
            this.ctrlVer = null;
            g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);
        }

        private void InitPalette(string modelName)
        {
            foreach (var type in this.model.ModelRepo.MetamodelNodes(modelName))
            {
                var button = new ToggleButton { Content = type.name };
                button.Click += (sender, args) => this.currentId = type.id;
                if (this.model.ModelRepo.IsEdgeClass(type.id))
                {
                    g_Area.VertexSelected += (sender, args) => button.IsChecked = false;
                }
                else
                {
                    g_zoomctrl.MouseDown += (sender, args) => button.IsChecked = false;
                }

                // TODO: Bind it to XAML, do not do GUI work in C#.

                paletteGrid.RowDefinitions.Add(new RowDefinition());
                paletteGrid.Children.Add(button);
                Grid.SetRow(button, paletteGrid.RowDefinitions.Count - 1);
            }
        }

        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            StackPanel sp = (elementsListBox.SelectedItem as ListBoxItem).Content as StackPanel;
            if (sp.Children.Count > 3)
            {
                var source = (sp.Children[2] as TextBlock).Text;
                var target = (sp.Children[4] as TextBlock).Text;
                for (int i = 0; i < this.graph.DataGraph.Edges.Count(); i++)
                {
                    if (this.graph.DataGraph.Edges.ToList()[i].Source.Name == source &&
                        this.graph.DataGraph.Edges.ToList()[i].Target.Name == target)
                    {
                        var edge = this.graph.DataGraph.Edges.ToList()[i];
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
                for (int i = 0; i < this.graph.DataGraph.Vertices.Count(); i++)
                {
                    if (this.graph.DataGraph.Vertices.ToList()[i].Name == name)
                    {
                        var vertex = this.graph.DataGraph.Vertices.ToList()[i];
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

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            this.ctrlVer = args.VertexControl;
            if (this.currentId != String.Empty && this.model.ModelRepo.IsEdgeClass(this.currentId))
            {
                if (this.prevVer == null)
                {
                    this.editorManager.CreateVirtualEdge(this.ctrlVer, this.ctrlVer.GetPosition());
                    this.prevVer = this.ctrlVer;
                }
                else
                {
                    this.controller.NewEdge(this.currentId, this.prevVer, this.ctrlVer);
                }
            }

            attributesView.DataContext = this.ctrlVer.GetDataVertex<DataVertex>();
            g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().
            Color = Brushes.Green);
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

            // Those crazy russians intercept MouseUp event, so we are forced to use PreviewMouseUp here.

            this.ctrlEdg.PreviewMouseUp += this.OnEdgeMouseUp;
            g_zoomctrl.MouseMove += this.OnEdgeMouseMove;
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
            var mousePosition = Mouse.GetPosition(g_Area);
            dataEdge.RoutingPoints[1] = new GraphX.Measure.Point(mousePosition.X, mousePosition.Y);
            dataEdge.RoutingPoints[2] = new GraphX.Measure.Point(100, 100);
            g_Area.UpdateAllEdges();
        }

        private void ZoomCtrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = g_zoomctrl.TranslatePoint(e.GetPosition(g_zoomctrl), g_Area);
                if (this.currentId != string.Empty && !this.model.ModelRepo.IsEdgeClass(this.currentId))
                {
                    this.pos = position;
                    this.CreateNewNode(this.currentId);
                    this.currentId = string.Empty;
                }

                if (this.currentId != string.Empty && this.model.ModelRepo.IsEdgeClass(this.currentId))
                {
                    if (this.prevVer != null)
                    {
                        this.prevVer = null;
                        this.editorManager.DestroyVirtualEdge();
                        this.currentId = string.Empty;
                    }
                }
            }
        }

        private void CreateNewNode(string typeId)
        {
            this.controller.NewNode(typeId);
        }

        private void AddNewVertexControl(DataVertex vertex)
        {
            this.DrawNewVertex(vertex.Name);
            var vc = new VertexControl(vertex);
            vc.SetPosition(this.pos);
            g_Area.AddVertex(vertex, vc);
        }

        private void AddNewEdgeControl(DataEdge edge)
        {
            this.DrawNewEdge(edge.Source.Name, edge.Target.Name);
            var ec = new EdgeControl(this.prevVer, this.ctrlVer, edge);
            g_Area.InsertEdge(edge, ec);
            this.prevVer = null;
            this.editorManager.DestroyVirtualEdge();
            this.currentId = string.Empty;
        }

        private void MenuItemClickVert(object sender, EventArgs e)
        {
            this.graph.DataGraph.RemoveVertex(this.ctrlVer.GetDataVertex<DataVertex>());
            this.DrawGraph();
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            this.graph.DataGraph.RemoveEdge(this.ctrlEdg.GetDataEdge<DataEdge>());
            this.DrawGraph();
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            g_zoomctrl.MouseMove -= this.OnEdgeMouseMove;
            this.ctrlEdg.PreviewMouseUp -= this.OnEdgeMouseUp;
        }

        private void CloseChildrenWindows(object sender, EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                w.Close();
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
            elementsListBox.Items.Add(lbi);
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
            elementsListBox.Items.Add(lbi);
        }

        private void DrawGraph()
        {
            g_Area.GenerateGraph(this.graph.DataGraph);
            g_zoomctrl.ZoomToFill();
        }
    }
}