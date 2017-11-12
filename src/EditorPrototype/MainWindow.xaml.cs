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
        private GraphExample dataGraph;
        private Repo.IElement currentElement = null;

        private Repo.IRepo repo = Repo.RepoFactory.CreateRepo();

        public MainWindow()
        {
            this.InitializeComponent();
            this.editorManager = new EditorObjectManager(this.g_Area, this.g_zoomctrl);
            this.dataGraph = new GraphExample();
            var logic = new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>();
            this.g_Area.LogicCore = logic;
            logic.Graph = this.dataGraph;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog;

            this.g_Area.VertexSelected += this.VertexSelectedAction;
            this.g_Area.EdgeSelected += this.EdgeSelectedAction;
            this.g_zoomctrl.Click += this.ClearSelection;
            this.elementsListBox.MouseDoubleClick += this.ElementInBoxSelectedAction;
            this.attributesView.CellEditEnding += this.AttributeValueChanged;

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

            var modelName = "GreenhouseTestModel";

            this.g_zoomctrl.MouseDown += (object sender, MouseButtonEventArgs e)
                => this.ZoomCtrl_MouseDown(sender, e, modelName);

            this.InitPalette(modelName);
            this.InitModel(modelName);
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            this.prevVer = null;
            this.ctrlVer = null;
            this.attributesView.Visibility = Visibility.Collapsed;
            this.g_Area.GetAllVertexControls().ToList().
                ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);
        }

        private void InitModel(string modelName)
        {
            Repo.IModel model = this.repo.Model(modelName);
            if (model == null)
            {
                return;
            }

            foreach (var node in model.Nodes)
            {
                this.CreateNode(node);
            }

            foreach (var edge in model.Edges)
            {
                /* var isViolation = Constraints.CheckEdge(edge, this.repo, modelName); */

                var sourceNode = edge.From as Repo.INode;
                var targetNode = edge.To as Repo.INode;
                if (sourceNode == null || targetNode == null)
                {
                    // Editor does not support edges linked to edges. Yet.
                    continue;
                }

                if (this.dataGraph.Vertices.Count(v => v.Node == sourceNode) == 0
                    || this.dataGraph.Vertices.Count(v => v.Node == targetNode) == 0)
                {
                    // Link to an attribute node. TODO: It's ugly.
                    continue;
                }

                var source = this.dataGraph.Vertices.First(v => v.Node == sourceNode);
                var sourceControl = this.g_Area.VertexList.First(vc => vc.Key == source).Value;
                var target = this.dataGraph.Vertices.First(v => v.Node == targetNode);
                var targetControl = this.g_Area.VertexList.First(vc => vc.Key == target).Value;
                var newEdge = new DataEdge(source, target, false)
                    { EdgeType = DataEdge.EdgeTypeEnum.Association };

                this.dataGraph.AddEdge(newEdge);
                this.DrawNewEdge(source.Node, target.Node);
                var ec = new EdgeControl(sourceControl, targetControl, newEdge);
                this.g_Area.InsertEdge(newEdge, ec);
            }

            this.g_Area.RelayoutGraph(true);
            this.g_zoomctrl.ZoomToFill();

            foreach (var vertex in this.g_Area.VertexList)
            {
                foreach (var edge in this.dataGraph.Edges)
                {
                    if (edge.Target == vertex.Key)
                    {
                        StaticVertexConnectionPointForGH targetVCP = null;
                        foreach (var x in vertex.Value.VertexConnectionPointsList)
                        {
                            var aVCPforGH = x as StaticVertexConnectionPointForGH;
                            if (aVCPforGH != null && aVCPforGH.IsOccupied == false && aVCPforGH.IsSource == false)
                            {
                                aVCPforGH.IsOccupied = true;
                                targetVCP = aVCPforGH;
                                break;
                            }
                        }

                        if (targetVCP != null)
                        {
                            edge.TargetConnectionPointId = targetVCP.Id;
                        }
                        else
                        {
                            // if the model is initially incorret
                            if (vertex.Key.Node.Name == "aInterval")
                            {
                                return;
                            }

                            var newId = vertex.Value.VertexConnectionPointsList.Last().Id + 1;
                            var vcp = new StaticVertexConnectionPointForGH()
                                { Id = newId, IsOccupied = true, IsSource = false };
                            var ctrl = new Border
                                { Margin = new Thickness(0, 2, 2, 0), Padding = new Thickness(0), Child = vcp };
                            ((VertexControlForGH)vertex.Value).VCPTargetRoot.Children.Add(ctrl);
                            vertex.Value.VertexConnectionPointsList.Add(vcp);
                            edge.TargetConnectionPointId = newId;
                        }
                    }

                    if (edge.Source == vertex.Key)
                    {
                        StaticVertexConnectionPointForGH sourceVCP = null;
                        foreach (var x in vertex.Value.VertexConnectionPointsList)
                        {
                            var aVCPforGH = x as StaticVertexConnectionPointForGH;
                            if (aVCPforGH != null && aVCPforGH.IsOccupied == false && aVCPforGH.IsSource == true)
                            {
                                aVCPforGH.IsOccupied = true;
                                sourceVCP = aVCPforGH;
                                break;
                            }
                        }

                        if (sourceVCP != null)
                        {
                            edge.SourceConnectionPointId = sourceVCP.Id;
                        }
                        else
                        {
                            var newId = vertex.Value.VertexConnectionPointsList.Last().Id + 1;
                            sourceVCP = new StaticVertexConnectionPointForGH()
                                { Id = newId, IsOccupied = true, IsSource = true };
                            var ctrl = new Border
                                { Margin = new Thickness(0, 2, 2, 0), Padding = new Thickness(0), Child = sourceVCP };
                            ((VertexControlForGH)vertex.Value).VCPSourceRoot.Children.Add(ctrl);
                            vertex.Value.VertexConnectionPointsList.Add(sourceVCP);
                            edge.SourceConnectionPointId = newId;
                        }
                    }

                    this.g_Area.EdgesList[edge].UpdateEdge();
                }
            }
        }

        private void InitPalette(string metamodelName)
        {
            var model = this.repo.Model(metamodelName).Metamodel;
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
                    : new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png"))
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
                }

                // TODO: Bind it to XAML, do not do GUI work in C#.
                this.paletteGrid.RowDefinitions.Add(new RowDefinition());
                this.paletteGrid.Children.Add(button);
                Grid.SetRow(button, this.paletteGrid.RowDefinitions.Count - 1);
            }
        }

        private void PaletteButton_Checked(Repo.IElement element)
        {
            this.currentElement = element;
        }

        private void CreateEdge(Repo.IEdge edge)
        {
            var prevVerVertex = this.prevVer?.Vertex as DataVertex;
            var ctrlVerVertex = this.ctrlVer?.Vertex as DataVertex;
            if (prevVerVertex == null || ctrlVerVertex == null)
            {
                return;
            }

            var newEdge = new DataEdge(prevVerVertex, ctrlVerVertex, true)
                { EdgeType = DataEdge.EdgeTypeEnum.Association };

            StaticVertexConnectionPointForGH targetVCP = null;
            foreach (var x in this.ctrlVer.VertexConnectionPointsList)
            {
                var aVCPforGH = x as StaticVertexConnectionPointForGH;
                if (aVCPforGH != null && aVCPforGH.IsOccupied == false && aVCPforGH.IsSource == false)
                {
                    aVCPforGH.IsOccupied = true;
                    targetVCP = aVCPforGH;
                    break;
                }
            }

            if (targetVCP != null)
            {
                newEdge.TargetConnectionPointId = targetVCP.Id;
            }
            else
            {
                if (ctrlVerVertex.Node.Name == "aInterval")
                {
                    return;
                }

                var newId = this.ctrlVer.VertexConnectionPointsList.Last().Id + 1;
                targetVCP = new StaticVertexConnectionPointForGH()
                    { Id = newId, IsOccupied = true, IsSource = false };
                var ctrl = new Border
                    { Margin = new Thickness(0, 2, 2, 0), Padding = new Thickness(0), Child = targetVCP };
                ((VertexControlForGH)this.ctrlVer).VCPTargetRoot.Children.Add(ctrl);
                this.ctrlVer.VertexConnectionPointsList.Add(targetVCP);
                newEdge.TargetConnectionPointId = newId;
            }

            StaticVertexConnectionPointForGH sourceVCP = null;
            foreach (var x in this.prevVer.VertexConnectionPointsList)
            {
                var aVCPforGH = x as StaticVertexConnectionPointForGH;
                if (aVCPforGH != null && aVCPforGH.IsOccupied == false && aVCPforGH.IsSource == true)
                {
                    aVCPforGH.IsOccupied = true;
                    sourceVCP = aVCPforGH;
                    break;
                }
            }

            if (sourceVCP != null)
            {
                newEdge.SourceConnectionPointId = sourceVCP.Id;
            }
            else
            {
                var newId = this.prevVer.VertexConnectionPointsList.Last().Id + 1;
                sourceVCP = new StaticVertexConnectionPointForGH()
                    { Id = newId, IsOccupied = true, IsSource = true };
                var ctrl = new Border
                    { Margin = new Thickness(0, 2, 2, 0), Padding = new Thickness(0), Child = sourceVCP };
                ((VertexControlForGH)this.prevVer).VCPSourceRoot.Children.Add(ctrl);
                this.prevVer.VertexConnectionPointsList.Add(sourceVCP);
                newEdge.SourceConnectionPointId = newId;
            }

            this.dataGraph.AddEdge(newEdge);
            this.DrawNewEdge(prevVerVertex.Node, ctrlVerVertex.Node);
            var ec = new EdgeControl(this.prevVer, this.ctrlVer, newEdge);
            this.g_Area.InsertEdge(newEdge, ec);
        }

        // TODO: Copypaste is heresy.
        private void CreateNode(Repo.INode node)
        {
            var vertex = new DataVertex(node.Name)
            {
                Node = node,
                VertexType = DataVertex.VertexTypeEnum.Node,
                Picture = node.Class.Shape
            };

            var attributeInfos = node.Attributes.Select(x => new DataVertex.Attribute()
            {
                Name = x.Name,
                Type = x.Kind.ToString(),
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));

            this.dataGraph.AddVertex(vertex);
            this.DrawNewVertex(vertex);
            var vc = new VertexControlForGH(vertex);
            this.g_Area.AddVertex(vertex, vc);
        }

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
                        : new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png"))
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
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Edge.png"))
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

        private void AttributeValueChanged(object sender, DataGridCellEditEndingEventArgs args)
        {
            var changedTextBox = args.EditingElement as TextBox;
            if (changedTextBox == null)
            {
                return;
            }

            var newValue = changedTextBox.Text;
            var changedAttribute = args.Row.Item as DataVertex.Attribute;
            if (changedAttribute == null)
            {
                return;
            }

            var changedAttributeName = changedAttribute.Name;
            DataVertex ctrlDataVertex = this.ctrlVer.GetDataVertex<DataVertex>();
            ctrlDataVertex.Node.Attributes.First(x => x.Name == changedAttributeName).StringValue = newValue;
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
                    this.CreateEdge(this.currentElement as Repo.IEdge);
                    this.prevVer = null;
                    this.editorManager.DestroyVirtualEdge();
                    this.currentElement = null;
                }
            }

            this.attributesView.Visibility = Visibility.Visible;
            this.attributesView.DataContext = this.ctrlVer.GetDataVertex<DataVertex>();

            this.g_Area.GetAllVertexControls().ToList().
                ForEach(x => x.GetDataVertex<DataVertex>().Color = Brushes.Green);

            this.ctrlVer.GetDataVertex<DataVertex>().Color = Brushes.LightBlue;
            if (this.prevVer != null)
            {
                this.prevVer.GetDataVertex<DataVertex>().Color = Brushes.Yellow;
            }

            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.VertexControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem { Header = "Delete item", Tag = args.VertexControl };
                mi.Click += this.MenuItemClickRemoveVertex;
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
                mi.Click += this.MenuItemClickRemoveEdge;
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
                var pos = this.g_zoomctrl.TranslatePoint(e.GetPosition(this.g_zoomctrl), this.g_Area);
                if (this.currentElement != null && this.currentElement.InstanceMetatype == Repo.Metatype.Node)
                {
                    this.CreateNewNode(this.currentElement, pos, modelName);
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

        private void CreateNewNode(Repo.IElement element, Point position, string modelName)
        {
            var model = this.repo.Model(modelName);
            var newNode = model.CreateElement(element) as Repo.INode;
            this.CreateNode(newNode, position);
        }

        private void CreateNode(Repo.INode node, Point position)
        {
            var vertex = new DataVertex(node.Name)
            {
                Node = node,
                VertexType = DataVertex.VertexTypeEnum.Node,
                Picture = node.Class.Shape
            };

            var attributeInfos = node.Attributes.Select(x => new DataVertex.Attribute()
            {
                Name = x.Name,
                Type = x.Kind.ToString(),
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));

            var vc = new VertexControlForGH(vertex);
            vc.SetPosition(position);
            this.dataGraph.AddVertex(vertex);
            this.g_Area.AddVertex(vertex, vc);
            this.DrawNewVertex(vertex);
        }

        private void OnEdgeMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.g_zoomctrl.MouseMove -= this.OnEdgeMouseMove;
            this.ctrlEdg.PreviewMouseUp -= this.OnEdgeMouseUp;
        }

        private void MenuItemClickRemoveVertex(object sender, EventArgs e)
        {
            this.g_Area.RemoveVertexAndEdges(this.ctrlVer.GetDataVertex<DataVertex>());
        }

        private void MenuItemClickRemoveEdge(object sender, EventArgs e)
        {
            this.g_zoomctrl.MouseMove -= this.OnEdgeMouseMove;
            this.g_Area.RemoveEdge(this.ctrlEdg.GetDataEdge<DataEdge>(), true);
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
