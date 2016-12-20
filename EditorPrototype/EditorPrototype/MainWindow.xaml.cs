using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using GraphX.Controls;
using GraphX.Controls.Models;
using QuickGraph;

namespace EditorPrototype
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataVertex newVertex;
        public static DataEdge newEdge;
        public static VertexControl ctrlVer;
        public static EdgeControl ctrlEdg;
        public static GraphExample dataGraph;
        public static VertexCreationWindow VertexWindow;
        public static EdgeCreationWindow EdgeWindow;
        public static bool close;
        public MainWindow()
        {
            InitializeComponent();
            close = false;
            dataGraph = new GraphExample();
            VertexWindow = new VertexCreationWindow();
            EdgeWindow = new EdgeCreationWindow();
            VertexWindow.VertexAddButClicked += DrawNewVertex;
            EdgeWindow.EdgeAddButClicked += DrawNewEdge;
            var logic = new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>();
            g_Area.LogicCore = logic;
            logic.Graph = dataGraph;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog;
            g_Area.VertexSelected += VertexSelectedAction;
            g_Area.EdgeSelected += EdgeSelectedAction;
            elementsListBox.MouseDoubleClick += ElementInBoxSelectedAction;

            ZoomControl.SetViewFinderVisibility(g_zoomctrl, Visibility.Visible);
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

        private void DrawNewVertex(object sender, EventArgs e)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Vertex.png"));
            TextBlock spaces = new TextBlock() { Text = "  " };
            TextBlock tx = new TextBlock() { Text = newVertex.ToString() };
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            elementsListBox.Items.Add(lbi);

            /*var vc1 = new VertexControl(newVertex);
            g_Area.AddVertex(newVertex, vc1);
            g_zoomctrl.ZoomToFill();*/
            
            DrawGraph(sender, e);
        }

        private void DrawNewEdge(object sender, EventArgs e)
        {
            ListBoxItem lbi = new ListBoxItem();
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Edge.png"));
            TextBlock spaces = new TextBlock() { Text = "  " };
            TextBlock tx0 = new TextBlock() { Text = newEdge.Source.ToString()};
            TextBlock tx1 = new TextBlock() { Text = " - " };
            TextBlock tx2 = new TextBlock() { Text = newEdge.Target.ToString()};
            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx0);
            sp.Children.Add(tx1);
            sp.Children.Add(tx2);
            lbi.Content = sp;
            elementsListBox.Items.Add(lbi);
            
            DrawGraph(sender, e);
        }

        private void VertexSelectedAction(object sender, VertexSelectedEventArgs args)
        {
            ctrlVer = args.VertexControl;
            elementsTextBlock.Text = "Type: Vertex\n\rName: " + ctrlVer.GetDataVertex<DataVertex>().Name 
                + "\n\rKey: " + ctrlVer.GetDataVertex<DataVertex>().Key;
            
            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.VertexControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem() { Header = "Delete item", Tag = args.VertexControl };
                mi.Click += MenuItemClickVert;
                args.VertexControl.ContextMenu.Items.Add(mi);
                args.VertexControl.ContextMenu.IsOpen = true;
            }
        }

        private void EdgeSelectedAction(object sender, EdgeSelectedEventArgs args)
        {
            ctrlEdg = args.EdgeControl;
            elementsTextBlock.Text = "Type: Edge\n\rText: " + ctrlEdg.GetDataEdge<DataEdge>().Text 
                + "\n\rSource: " + ctrlEdg.GetDataEdge<DataEdge>().Source.Name + "\n\rTarget: " 
                + ctrlEdg.GetDataEdge<DataEdge>().Target.Name;
            
            if (args.MouseArgs.RightButton == MouseButtonState.Pressed)
            {
                args.EdgeControl.ContextMenu = new ContextMenu();
                var mi = new MenuItem() { Header = "Delete item", Tag = args.EdgeControl };
                mi.Click += MenuItemClickEdge;
                args.EdgeControl.ContextMenu.Items.Add(mi);
                args.EdgeControl.ContextMenu.IsOpen = true;
            }
        }

        private void MenuItemClickVert(object sender, EventArgs e)
        {
            dataGraph.RemoveVertex(ctrlVer.GetDataVertex<DataVertex>());
            DrawGraph(sender, e);
        }

        private void MenuItemClickEdge(object sender, EventArgs e)
        {
            dataGraph.RemoveEdge(ctrlEdg.GetDataEdge<DataEdge>());
            DrawGraph(sender, e);
        }

        private void CloseChildrenWindows(object sender, EventArgs e)
        {
            close = true;
            foreach (Window w in Application.Current.Windows)
                w.Close();
        }

        private void DrawGraph(object sender, EventArgs e)
        {
            g_Area.GenerateGraph(dataGraph);
            g_zoomctrl.ZoomToFill();
        }
                
        private void entityAddBut_Click(object sender, RoutedEventArgs e)
        {
            EntitySelectWindow SelectWindow = new EntitySelectWindow();
            SelectWindow.Owner = this;
            SelectWindow.ShowDialog();
        }
    }
}
