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
using WPF_Editor.ViewModels.Helpers;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class SceneViewModel : ISceneViewModel
    {
        private ZoomControl _zoomControl;
        private GraphArea _graphArea;
        private GXLogicCore _logicCore;
        private static ISceneViewModel _scene;
        private readonly ISceneMediatorViewModel _sceneMediator;

        public static ISceneViewModel CreateScene(ISceneMediatorViewModel sceneMediator = null)
        {
            if (_scene == null)
            {
                _scene = new SceneViewModel(sceneMediator);
            }
            return _scene;
        }

        private SceneViewModel(ISceneMediatorViewModel sceneMediator)
        {
            _sceneMediator = sceneMediator;
        }
        public void InitializeScene(ZoomControl zoomControl)
        {
            _zoomControl = zoomControl;
            _graphArea = (zoomControl.Content) as GraphArea;
            if (_graphArea == null)
            {
                throw new ArgumentException("Zoom control doesn't contain an instance of class GraphArea.");
            }
            var graph = new BidirectionalGraph<ModelNode, ModelEdge>();

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
            _graphArea.LogicCore = _logicCore;
            _graphArea.SetVerticesDrag(true);
            _graphArea.GenerateGraph();
        }



        public void HandleSingleLeftClick()
        {
            ModelElement element = _sceneMediator.GetInstanceOfSelectedType();
            if (element is ModelNode)
            {
                var node = (ModelNode)element;
                _logicCore.Graph.AddVertex(node);
                _graphArea.GenerateGraph();
                return;
            }
            // If element isn't selected it won't do anything
            if (element == null)
            {

                return;
            }

        }

    }
}
