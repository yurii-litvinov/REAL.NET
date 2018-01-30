using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EditorPluginInterfaces;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using PluginManager;
using QuickGraph;
using WpfEditor.Controls.Console;
using WpfEditor.Model;
using WpfEditor.ViewModel;

namespace WpfEditor.View
{
    /// <summary>
    /// Main window of the application, launches on application startup.
    /// </summary>
    internal partial class MainWindow : Window
    {
        private readonly EditorObjectManager editorManager;
        private VertexControl prevVer;
        private VertexControl ctrlVer;
        private EdgeControl ctrlEdg;
        private readonly Model.Model model;
        private readonly Controller.Controller controller;
        private readonly Graph graph;
        private Point pos;
        public AppConsoleViewModel Console { get; } = new AppConsoleViewModel();

        public MainWindow()
        {
            this.DataContext = this;
            this.InitializeComponent();

            this.model = new Model.Model();

            this.palette.SetModel(this.model);

            this.controller = new Controller.Controller(this.model);
            this.graph = new Graph(this.model);
            this.graph.DrawGraph += (sender, args) => this.DrawGraph();
            this.graph.DrawNewEdge += (sender, args) => this.DrawNewEdge(args.Source, args.Target);
            this.graph.DrawNewVertex += (sender, args) => this.DrawNewVertex(args.VertName);
            this.graph.AddNewVertexControl += (sender, args) => this.AddNewVertexControl(args.DataVert);
            this.graph.AddNewEdgeControl += (sender, args) => this.AddNewEdgeControl(args.EdgeViewModel);
            this.editorManager = new EditorObjectManager(this.scene, this.zoomControl);
            var logic =
                new GXLogicCore<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>
                {
                    Graph = this.graph.DataGraph,
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog
                };

            this.scene.LogicCore = logic; // need?

            this.scene.VertexSelected += this.VertexSelectedAction;
            this.scene.EdgeSelected += this.EdgeSelectedAction;
            this.zoomControl.Click += this.ClearSelection;
            this.elementsListBox.MouseDoubleClick += this.ElementInBoxSelectedAction;

            ZoomControl.SetViewFinderVisibility(this.zoomControl, Visibility.Visible);
            this.zoomControl.Loaded += (sender, args) =>
            {
                (this.zoomControl.ViewFinder.Parent as Grid).Children.Remove(this.zoomControl.ViewFinder);
                this.rightPanel.Children.Add(this.zoomControl.ViewFinder);
                Grid.SetRow(this.zoomControl.ViewFinder, 0);
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

            this.InitModelComboBox();

            this.zoomControl.MouseDown += (sender, e) => this.ZoomCtrl_MouseDown(sender, e, this.model.ModelName);
            this.zoomControl.Drop += (sender, e) => this.ZoomControl_Drop(sender, e, this.model.ModelName);

            this.palette.InitPalette(this.model.ModelName);
            this.graph.InitModel(this.model.ModelName);
            
            this.InitAndLaunchPlugins();
        }

        private void InitModelComboBox()
        {
            var repo = this.model.Repo;

            foreach (var currentModel in repo.Models)
            {
                this.modelsComboBox.Items.Add(currentModel.Name);
            }

            this.modelsComboBox.SelectedIndex = 0;
            this.model.ModelName = this.modelsComboBox.SelectedItem.ToString();

            this.modelsComboBox.SelectionChanged += (sender, args) =>
            {
                this.graph.DataGraph.Clear();
                this.elementsListBox.Items.Clear();
                this.model.ModelName = this.modelsComboBox.SelectedItem.ToString();
                this.palette.InitPalette(this.model.ModelName);
                this.graph.InitModel(this.model.ModelName);
            };
        }

        private void InitAndLaunchPlugins()
        {
            var libs = new PluginLauncher<PluginConfig>();
            const string folder = "../../../plugins/SamplePlugin/bin";
            var dirs = new List<string>(System.IO.Directory.GetDirectories(folder));
            var config = new PluginConfig(null, null, this.Console, null);
            foreach (var dir in dirs)
            {
                libs.LaunchPlugins(dir, config);
            }
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.prevVer = null;
            this.ctrlVer = null;
            this.scene.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<NodeViewModel>().Color = Brushes.Green);
        }

        // Need for dropping.
        private void ZoomControl_Drop(object sender, DragEventArgs e, string modelName)
        { 
            this.pos = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.scene);
            this.CreateNewNode((Repo.IElement)e.Data.GetData("REAL.NET palette element"), modelName);
            this.palette.ClearSelection();
        }

        // TODO: Copypaste is heresy.
        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            var sp = (this.elementsListBox.SelectedItem as ListBoxItem)?.Content as StackPanel;
            if (sp == null)
            {
                return;
            }

            if (sp.Children.Count > 3)
            {
                var source = (sp.Children[2] as TextBlock).Text;
                var target = (sp.Children[4] as TextBlock).Text;
                for (var i = 0; i < this.graph.DataGraph.Edges.Count(); i++)
                {
                    if (this.graph.DataGraph.Edges.ToList()[i].Source.Name == source &&
                        this.graph.DataGraph.Edges.ToList()[i].Target.Name == target)
                    {
                        var edge = this.graph.DataGraph.Edges.ToList()[i];
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
            else
            {
                var name = (sp.Children[2] as TextBlock).Text;
                for (var i = 0; i < this.graph.DataGraph.Vertices.Count(); i++)
                {
                    if (this.graph.DataGraph.Vertices.ToList()[i].Name == name)
                    {
                        var vertex = this.graph.DataGraph.Vertices.ToList()[i];
                        this.attributesView.DataContext = vertex;
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
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            this.ctrlVer = args.VertexControl;
            if (this.palette.SelectedElement?.InstanceMetatype == Repo.Metatype.Edge)
            {
                if (this.prevVer == null)
                {
                    this.editorManager.CreateVirtualEdge(this.ctrlVer, this.ctrlVer.GetPosition());
                    this.prevVer = this.ctrlVer;
                }
                else
                {
                    this.controller.NewEdge(this.palette.SelectedElement, this.prevVer, this.ctrlVer);
                    this.palette.ClearSelection();
                }

                return;
            }

            this.attributesView.DataContext = this.ctrlVer.GetDataVertex<NodeViewModel>();
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

        private void EdgeSelectedAction(object sender, EdgeSelectedEventArgs args)
        {
            this.ctrlEdg = args.EdgeControl;

            // Those crazy russians intercept MouseUp event, so we are forced to use PreviewMouseUp here.
            this.ctrlEdg.PreviewMouseUp += this.OnEdgeMouseUp;
            this.zoomControl.MouseMove += this.OnEdgeMouseMove;
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
            var dataEdge = this.ctrlEdg.GetDataEdge<EdgeViewModel>();
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

        private void ZoomCtrl_MouseDown(object sender, MouseButtonEventArgs e, string modelName)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = this.zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.scene);
                if (this.palette.SelectedElement?.InstanceMetatype == Repo.Metatype.Node)
                {
                    this.pos = position;
                    this.CreateNewNode(this.palette.SelectedElement, modelName);
                    this.palette.ClearSelection();
                }

                if (this.palette.SelectedElement?.InstanceMetatype == Repo.Metatype.Edge)
                {
                    if (this.prevVer != null)
                    {
                        this.prevVer = null;
                        this.editorManager.DestroyVirtualEdge();
                        this.palette.ClearSelection();
                    }
                }
            }
        }

        private void CreateNewNode(Repo.IElement element, string modelName)
        {
            this.controller.NewNode(element, modelName);
        }

        private void AddNewVertexControl(NodeViewModel vertex)
        {
            this.DrawNewVertex(vertex.Name);
            var vc = new VertexControl(vertex);
            vc.SetPosition(this.pos);
            this.scene.AddVertex(vertex, vc);
        }

        private void AddNewEdgeControl(EdgeViewModel edgeViewModel)
        {
            this.DrawNewEdge(edgeViewModel.Source.Name, edgeViewModel.Target.Name);
            var ec = new EdgeControl(this.prevVer, this.ctrlVer, edgeViewModel);
            this.scene.InsertEdge(edgeViewModel, ec);
            this.prevVer = null;
            this.editorManager.DestroyVirtualEdge();
        }

        private void MenuItemClickVert(object sender, EventArgs e)
        {
            this.graph.DataGraph.RemoveVertex(this.ctrlVer.GetDataVertex<NodeViewModel>());
            this.DrawGraph();
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            this.graph.DataGraph.RemoveEdge(this.ctrlEdg.GetDataEdge<EdgeViewModel>());
            this.DrawGraph();
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.zoomControl.MouseMove -= this.OnEdgeMouseMove;
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
            var lbi = new ListBoxItem();
            var sp = new StackPanel { Orientation = Orientation.Horizontal };

            var img = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/View/Pictures/vertex.png")),
            };
            var spaces = new TextBlock { Text = "  " };
            var tx = new TextBlock { Text = vertexName };

            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
        }

        private void DrawNewEdge(string source, string target)
        {
            var lbi = new ListBoxItem();
            var sp = new StackPanel { Orientation = Orientation.Horizontal };

            var img = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/View/Pictures/edge.png")),
            };
            var spaces = new TextBlock { Text = "  " };
            var tx0 = new TextBlock { Text = source };
            var tx1 = new TextBlock { Text = " - " };
            var tx2 = new TextBlock { Text = target };

            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx0);
            sp.Children.Add(tx1);
            sp.Children.Add(tx2);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
        }

        private void DrawGraph()
        {
            this.scene.GenerateGraph(this.graph.DataGraph);
            this.zoomControl.ZoomToFill();
        }

        private void ConstraintsButtonClick(object sender, RoutedEventArgs e)
        {
            //var constraints = new ConstraintsWindow(this.repo, this.repo.Model(this.modelName));
            //constraints.ShowDialog();
        }
    }
}
