using GraphX.Measure;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using QuickGraph;
using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public class ModelEdge : ModelElement, IEdge, IGraphXEdge<ModelNode>
    {
        private readonly IEdge _edge;

        public ModelEdge(IEdge edge, ModelElement source, ModelElement target) : base(edge)
        {
            _edge = edge;
            From = source.Element;
            To = target.Element;
        }

        public IElement From
        {
            get => _edge.From;
            set => _edge.From = value;
        }

        public IElement To
        {
            get => _edge.To;
            set => _edge.To = value;
        }

        #region IGraphXEdge<ModelNode> implemetation

        public long ID { get; set; }
        public ProcessingOptionEnum SkipProcessing { get; set; }
        public Point[] RoutingPoints { get; set; }
        public bool IsSelfLoop { get; }
        public int? SourceConnectionPointId { get; }
        public int? TargetConnectionPointId { get; }
        public bool ReversePath { get; set; }

        ModelNode IEdge<ModelNode>.Source => ((IGraphXEdge<ModelNode>) this).Source;
        ModelNode IEdge<ModelNode>.Target => ((IGraphXEdge<ModelNode>) this).Target;

        ModelNode IGraphXEdge<ModelNode>.Target { get; set; }

        ModelNode IGraphXEdge<ModelNode>.Source { get; set; }


        public double Weight { get; set; }

        #endregion
    }
}