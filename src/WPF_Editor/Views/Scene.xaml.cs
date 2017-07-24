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
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Models;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using QuickGraph;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    /// <summary>
    /// Interaction logic for Scene.xaml
    /// </summary>
    public partial class Scene : UserControl
    {
        public class GraphAreaExample : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> { }

        //Graph data class
        public class GraphExample : BidirectionalGraph<DataVertex, DataEdge> { }

        //Logic core class
        public class GXLogicCoreExample : GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> { }

        //Vertex data object
        public class DataVertex : VertexBase
        {
            /// <summary>
            /// Some string property for example purposes
            /// </summary>
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        //Edge data object
        public class DataEdge : EdgeBase<DataVertex>
        {
            public DataEdge(DataVertex source, DataVertex target, double weight = 1)
                : base(source, target, weight)
            {
            }

            public DataEdge()
                : base(null, null, 1)
            {
            }

            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        public Scene()
        {
            InitializeComponent();
        

            DataContext = new SceneViewModel();
            Random Rand = new Random();

            //Create data graph object
            var graph = new Views.GraphExample();

            //Create and add vertices using some DataSource for ID's
            for (int i = 0; i < 100; i++)
            {
                graph.AddVertex(new Views.DataVertex() { ID = i, Text = i.ToString() });
            }

            var vlist = graph.Vertices.ToList();
            //Generate random edges for the vertices
            foreach (var item in vlist)
            {
                if (Rand.Next(0, 50) > 25) continue;
                var vertex2 = vlist[Rand.Next(0, graph.VertexCount - 1)];
                graph.AddEdge(new Views.DataEdge(item, vertex2, Rand.Next(1, 50))
                    { Text = $"{item} -> {vertex2}" });
            }


            var logicCore = new Views.GXLogicCoreExample { DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            //Now we can set optional parameters using AlgorithmFactory
            //NOTE: default parameters can be automatically created each time you change Default algorithms
            logicCore.DefaultLayoutAlgorithmParams =
                logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Setup optional params
            logicCore.DefaultOverlapRemovalAlgorithmParams =
                logicCore.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = true;
            logicCore.Graph = graph;
            //Finally assign logic core to GraphArea object
            gg_Area.LogicCore = logicCore;
            gg_Area.GenerateGraph();
        }
    }
}
