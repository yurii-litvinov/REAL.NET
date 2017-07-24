using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using QuickGraph;
using Repo;
using WPF_Editor.Models.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class GraphArea : GraphArea<Node, Edge, BidirectionalGraph<Node, Edge>> { }
    public class GXLogicCore : GXLogicCore<Node, Edge, BidirectionalGraph<Node, Edge>> { }
    
    public class SceneViewModel
    {
        public GraphArea GraphArea { get; }
        public GXLogicCore LogicCore { get; }
        public IScene Scene { get; }

        public SceneViewModel(GraphArea graphArea)
        {
            Scene = Models.Scene.CreateScene();
            GraphArea = graphArea;
            var graph = new BidirectionalGraph<Node, Edge>();

            LogicCore = new GXLogicCore
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK,
            };
            LogicCore.DefaultLayoutAlgorithmParams =
                LogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            ((KKLayoutParameters)LogicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            LogicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            LogicCore.DefaultOverlapRemovalAlgorithmParams =
                LogicCore.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)LogicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)LogicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            LogicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;
            LogicCore.AsyncAlgorithmCompute = true;
            LogicCore.Graph = graph;

            GraphArea.LogicCore = LogicCore;
            GraphArea.GenerateGraph();
        }

        public void HandleSingleLeftClick()
        {
            var mousePosition = Mouse.GetPosition(null);
            Scene.CreateNode();
            IElement element = Scene.LastCreatedElement;
            if (element is INode)
            {
                var node = new Node((INode)element);
                var vertexControl = new VertexControl(node);
                vertexControl.SetPosition(mousePosition);

                LogicCore.Graph.AddVertex(node);
                GraphArea.GenerateGraph();
                return;
            }
            if (element is IEdge)
            {
                var edge = new Edge((IEdge)element);
                LogicCore.Graph.AddEdge(edge);
                GraphArea.GenerateGraph();
            }
            throw new NotImplementedException();
        }
    }
}
