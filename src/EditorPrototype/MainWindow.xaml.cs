using System.CodeDom.Compiler;
using System.Security.Permissions;
using System.Threading;
using EditorPrototype.AirSim;

namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using GraphX.Controls;
    using GraphX.Controls.Models;
    using GraphX.PCL.Common.Enums;
    using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
    using GraphX.PCL.Logic.Models;
    using PluginLibrary;
    using PluginLibrary.MainInterfaces;
    using QuickGraph;
    using System.Windows.Shapes;

    /// <summary>
    /// Ëîãèêà âçàèìîäåéñòâèÿ äëÿ MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private readonly EditorObjectManager editorManager;
        private VertexControl prevVer;
        private VertexControl ctrlVer;
        private EdgeControl ctrlEdg;
        private readonly Model model;
        private IConsole console;
        private readonly Controller controller;
        private readonly Graph graph;
        private Repo.IElement currentElement;
        private Point pos;

        public MainWindow()
        {
            this.InitializeComponent();
            this.model = new Model();
            this.controller = new Controller(this.model);
            this.graph = new Graph(this.model);
            this.graph.DrawGraph += (sender, args) => this.DrawGraph();
            this.graph.DrawNewEdge += (sender, args) => this.DrawNewEdge(args.Source, args.Target);
            this.graph.DrawNewVertex += (sender, args) => this.DrawNewVertex(args.VertName);
            this.graph.AddNewVertexControl += (sender, args) => this.AddNewVertexControl(args.DataVert);
            this.graph.AddNewEdgeControl += (sender, args) => this.AddNewEdgeControl(args.Edge);
            this.editorManager = new EditorObjectManager(this.g_Area, this.g_zoomctrl);
            var logic =
                new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
                {
                    Graph = this.graph.DataGraph,
                    DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog
                };

            this.g_Area.LogicCore = logic; // need?

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

            const string modelName = "AirSimModel";

            this.g_zoomctrl.MouseDown += (sender, e) => this.ZoomCtrl_MouseDown(sender, e, modelName);

            this.g_zoomctrl.MouseDown += (sender, e) => this.ZoomCtrl_MouseDown(sender, e, modelName);
            this.g_zoomctrl.Drop += (sender, e) => this.ZoomControl_Drop(sender, e, modelName);

            this.InitPalette(modelName);
            this.graph.InitModel(modelName);
            this.InitConsole();
            this.InitAndLaunchPlugins();
        }

        private void InitConsole()
        {
            var factory = Models.ControlFactory.ControlFactoryCreator.CreateControlFactory();
            this.console = factory.CreateConsole();
            this.console.NewMessage += this.OnConsoleMessagesHaveBeenUpdated;
            this.console.NewError += this.OnConsoleErrorsHaveBeenUpdated;
        }

        private void InitAndLaunchPlugins()
        {
            var libs = new PluginLauncher();
            var folder = "../../../plugins/SamplesPlugin/bin";
            var dirs = new List<string>(System.IO.Directory.GetDirectories(folder));
            var config = new PluginConfig(null, null, this.console, null);
            foreach (var dir in dirs)
            {
                libs.LaunchPlugins(dir, config);
            }
        }

        private void OnConsoleMessagesHaveBeenUpdated(object sender, EventArgs args)
        {
            string allMessages = "";

            foreach (var message in this.console.Messages)
            {
                allMessages += message + "\n";
            }

            this.Messages.Text = allMessages;
        }

        private void OnConsoleErrorsHaveBeenUpdated(object sender, EventArgs args)
        {
            string allErrors = "";

            foreach(var error in this.console.Errors)
            {
                allErrors += error + "\n";
            }

            this.Errors.Text = allErrors;
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.prevVer = null;
            this.ctrlVer = null;
            this.g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);
        }

        private void InitPalette(string metamodelName)
        {
            var model = this.model.ModelRepo.Model(metamodelName).Metamodel;
            if (model == null)
            {
                return;
            }

            foreach (var type in model.Elements)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                sp.HorizontalAlignment = HorizontalAlignment.Left;

                Label l = new Label() { Content = type.Name };
                Image img = new Image()
                {
                    Source = type.Shape != string.Empty
                    ? new BitmapImage(new Uri("pack://application:,,,/" + type.Shape))
                    : new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png")),
                };

                img.LayoutTransform = new ScaleTransform(0.3, 0.3);
                img.HorizontalAlignment = HorizontalAlignment.Left;
                l.VerticalAlignment = VerticalAlignment.Center;
                l.HorizontalAlignment = HorizontalAlignment.Left;

                sp.Children.Add(img);
                sp.Children.Add(l);

                var button = new ToggleButton { Content = sp };
                button.HorizontalContentAlignment = HorizontalAlignment.Left;

                RoutedEventHandler createNode = (sender, args) => this.PaletteButton_Checked(type);
                RoutedEventHandler createEdge = (sender, args) => { };
                button.Click += (sender, args) => this.currentElement = type;
                
                if (type.InstanceMetatype == Repo.Metatype.Edge)
                {
                    this.g_Area.VertexSelected += (sender, args) => button.IsChecked = false;
                }
                else
                {
                    this.g_zoomctrl.MouseDown += (sender, args) => button.IsChecked = false;
                    button.PreviewMouseMove += (sender, args) => this.currentElement = type;
                    button.PreviewMouseMove += this.PaletteButton_MouseMove;
                    button.GiveFeedback += this.DragSource_GiveFeedback;
                }

                // TODO: Bind it to XAML, do not do GUI work in C#.
                this.paletteGrid.RowDefinitions.Add(new RowDefinition());
                this.paletteGrid.Children.Add(button);
                Grid.SetRow(button, this.paletteGrid.RowDefinitions.Count - 1);
            }
        }

        // Code for drag-n-drop.
        // Helps dragging button image.
        private Window dragdropWindow = new Window();

        // Need for dragging.
        private void PaletteButton_MouseMove(object sender, MouseEventArgs args)
        {
            var button = sender as ToggleButton;

            if (button == null || !button.IsPressed)
            {
                return;
            }

            // Setting shadow for button.
            button.Effect = new DropShadowEffect
            {
                Color = new Color { A = 50, R = 0, G = 0, B = 0 },
                Direction = 300,
                ShadowDepth = 0,
                Opacity = 0.75
            };

            var dragData = new DataObject("MyFormat", this.currentElement);

            this.CreateDragDropWindow(button);
            DragDrop.DoDragDrop(button, dragData, DragDropEffects.Copy);

            if (this.dragdropWindow == null)
            {
                return;
            }

            this.dragdropWindow.Close();
            this.dragdropWindow = null;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        // Need for creating an image on cursor.
        private void CreateDragDropWindow(Visual dragElement)
        {
            this.dragdropWindow = new Window();
            this.dragdropWindow.WindowStyle = WindowStyle.None;
            this.dragdropWindow.AllowsTransparency = true;
            this.dragdropWindow.AllowDrop = false;
            this.dragdropWindow.Background = null;
            this.dragdropWindow.IsHitTestVisible = false;
            this.dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            this.dragdropWindow.Topmost = true;
            this.dragdropWindow.ShowInTaskbar = false;

            var rectangle = new Rectangle
            {
                Width = ((FrameworkElement)dragElement).ActualWidth,
                Height = ((FrameworkElement)dragElement).ActualHeight,
                Fill = new VisualBrush(dragElement)
            };

            this.dragdropWindow.Content = rectangle;

            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this.dragdropWindow.Left = w32Mouse.X;
            this.dragdropWindow.Top = w32Mouse.Y;
            this.dragdropWindow.Show();
        }

        // Need for dropping.
        private void ZoomControl_Drop(object sender, DragEventArgs e, string modelName)
        { 
            this.pos = this.g_zoomctrl.TranslatePoint(e.GetPosition(this.g_zoomctrl), this.g_Area);
            this.CreateNewNode((Repo.IElement)e.Data.GetData("MyFormat"), modelName);
            this.currentElement = null;
        }

        // Need for pre-defined cursor while dragging.
        private void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Update the position of the visual feedback item.
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this.dragdropWindow.Left = w32Mouse.X;
            this.dragdropWindow.Top = w32Mouse.Y;
        }

        private void PaletteButton_Checked(Repo.IElement element)
        {
            this.currentElement = element;
        }

        // TODO: Copypaste is heresy.
        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            StackPanel sp = (this.elementsListBox.SelectedItem as ListBoxItem)?.Content as StackPanel;
            if (sp == null)
            {
                return;
            }

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
                for (int i = 0; i < this.graph.DataGraph.Vertices.Count(); i++)
                {
                    if (this.graph.DataGraph.Vertices.ToList()[i].Name == name)
                    {
                        var vertex = this.graph.DataGraph.Vertices.ToList()[i];
                        this.attributesView.DataContext = vertex;
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

        private void DrawNewVertex(DataVertex vertex)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            Image img = new Image()
            {
                Source = vertex.Picture != "pack://application:,,,/"
                        ? new BitmapImage(new Uri(vertex.Picture))
                        : new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png")),
            };

            img.LayoutTransform = new ScaleTransform(0.3, 0.3);
            TextBlock spaces = new TextBlock { Text = "  " };
            TextBlock tx = new TextBlock
            {
                Text = vertex.Name,
                VerticalAlignment = VerticalAlignment.Center,
            };
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
        }

        private void DrawNewEdge(Repo.INode source, Repo.INode target)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
            Image img = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Edge.png")),
            };
            TextBlock spaces = new TextBlock { Text = "  " };
            TextBlock tx0 = new TextBlock { Text = source.Name };
            TextBlock tx1 = new TextBlock { Text = " - " };
            TextBlock tx2 = new TextBlock { Text = target.Name };
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx0);
            sp.Children.Add(tx1);
            sp.Children.Add(tx2);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            this.ctrlVer = args.VertexControl;
            if (this.currentElement != null && this.currentElement.InstanceMetatype == Repo.Metatype.Edge)
            {
                if (this.prevVer == null)
                {
                    this.editorManager.CreateVirtualEdge(this.ctrlVer, this.ctrlVer.GetPosition());
                    this.prevVer = this.ctrlVer;
                }
                else
                {
                    this.controller.NewEdge(this.currentElement, this.prevVer, this.ctrlVer);
                    this.currentElement = null;
                }
            }

            this.attributesView.DataContext = this.ctrlVer.GetDataVertex<DataVertex>();
            this.g_Area.GetAllVertexControls().ToList().ForEach(x => x.GetDataVertex<DataVertex>().
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
            this.g_zoomctrl.MouseMove += this.OnEdgeMouseMove;
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

        private void ZoomCtrl_MouseDown(object sender, MouseButtonEventArgs e, string modelName)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = this.g_zoomctrl.TranslatePoint(e.GetPosition(this.g_zoomctrl), this.g_Area);
                if (this.currentElement != null && this.currentElement.InstanceMetatype == Repo.Metatype.Node)
                {
                    this.pos = position;
                    this.CreateNewNode(this.currentElement, modelName);
                    this.currentElement = null;
                }

                if (this.currentElement != null && this.currentElement.InstanceMetatype == Repo.Metatype.Edge)
                {
                    if (this.prevVer != null)
                    {
                        this.prevVer = null;
                        this.editorManager.DestroyVirtualEdge();
                        this.currentElement = null;
                    }
                }
            }
        }

        private void CreateNewNode(Repo.IElement element, string modelName)
        {
            this.controller.NewNode(element, modelName);
        }

        private void AddNewVertexControl(DataVertex vertex)
        {
            this.DrawNewVertex(vertex.Name);
            var vc = new VertexControl(vertex);
            vc.SetPosition(this.pos);
            this.g_Area.AddVertex(vertex, vc);
        }

        private void AddNewEdgeControl(DataEdge edge)
        {
            this.DrawNewEdge(edge.Source.Name, edge.Target.Name);
            var ec = new EdgeControl(this.prevVer, this.ctrlVer, edge);
            this.g_Area.InsertEdge(edge, ec);
            this.prevVer = null;
            this.editorManager.DestroyVirtualEdge();
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
            this.g_zoomctrl.MouseMove -= this.OnEdgeMouseMove;
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
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png")),
            };
            TextBlock spaces = new TextBlock { Text = "  " };
            TextBlock tx = new TextBlock { Text = vertexName };

            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
        }

        private void DrawNewEdge(string source, string target)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };

            Image img = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Edge.png")),
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
        }

        private void DrawGraph()
        {
            this.g_Area.GenerateGraph(this.graph.DataGraph);
            this.g_zoomctrl.ZoomToFill();
        }

        private void ConstraintsButtonClick(object sender, RoutedEventArgs e)
        {
            //var constraints = new ConstraintsWindow(this.repo, this.repo.Model(this.modelName));
            //constraints.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread newThread = new Thread(CodeExecution.Execute);
            newThread.Start(this.graph);
            Messages.Text = "Running your code";
        }
    }
}
