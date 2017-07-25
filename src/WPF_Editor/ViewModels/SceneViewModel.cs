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
        private readonly ZoomControl _zoomControl;
        private readonly GraphArea _graphArea;
        private readonly GXLogicCore _logicCore;
        
        private IScene Scene { get; }

        public SceneViewModel(ZoomControl zoomControl)
        {
            _zoomControl = zoomControl;
            Scene = Models.Scene.CreateScene();
            _graphArea = (zoomControl.Content) as GraphArea;
            if (_graphArea == null)
            {
                throw new ArgumentException("Zoom control doesn't contain an instance of class GraphArea.");
            }
            var graph = new BidirectionalGraph<Node, Edge>();

            _logicCore = new GXLogicCore
            {
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK,
            };
            _logicCore.DefaultLayoutAlgorithmParams =
                _logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            ((KKLayoutParameters)_logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            _logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            _logicCore.DefaultOverlapRemovalAlgorithmParams =
                _logicCore.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)_logicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)_logicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            _logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;
            _logicCore.AsyncAlgorithmCompute = true;
            _logicCore.Graph = graph;
            var repo = RepoFactory.CreateRepo();
            var model = repo.Model("RobotsMetamodel");
            _graphArea.LogicCore = _logicCore;
            _graphArea.SetVerticesDrag(true);
            _graphArea.GenerateGraph();
        }

        public void HandleSingleLeftClick()
        {
            Scene.CreateNode();
            IElement element = Scene.LastCreatedElement;
            if (element is INode)
            {
                var node = new Node((INode)element);
                _logicCore.Graph.AddVertex(node);
                _graphArea.GenerateGraph();
                return;
            }
            // If element isn't selected it won't do anything
            if (element == null)
            {
                
                return;
            }
            throw new NotImplementedException("Cannot handle edges... yet...");
        }
    }
}
