using System.Collections.Generic;
using GraphX.Measure;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using QuickGraph;
using Repo;

namespace WPF_Editor.ViewModels
{
    // Target and source must be initialized.
    public sealed class Edge : Element, IEdge, IGraphXEdge<Node>
    {
        private readonly IEdge _edge;
        public IElement From { get => _edge.From; set => _edge.From = value; }

        public IElement To { get => _edge.To; set => _edge.To = value; }

        public override IElement Class => _edge.Class;

        public override IEnumerable<IAttribute> Attributes => _edge.Attributes;

        public override bool IsAbstract => _edge.IsAbstract;

        public override Metatype Metatype => _edge.Metatype;

        public override Metatype InstanceMetatype => _edge.InstanceMetatype;

        public override string Shape => _edge.Shape;
        /*Initialize _edge.From and _edge.To*/
        public Edge(IEdge iedge) : base(iedge)
        {
            _edge = iedge;
        }

        public long ID { get; set; }
        public ProcessingOptionEnum SkipProcessing { get; set; }
        public Point[] RoutingPoints { get; set; }
        public bool IsSelfLoop { get; }
        public int? SourceConnectionPointId { get; }
        public int? TargetConnectionPointId { get; }
        public bool ReversePath { get; set; }

        //Fix later. It's going to support edge-edge connection.
        Node IEdge<Node>.Source => From as Node;

        Node IGraphXEdge<Node>.Target
        {
            get => To as Node;
            set => To = value;
        }

        Node IGraphXEdge<Node>.Source
        {
            get => From as Node;
            set => From = value;
        }

        Node IEdge<Node>.Target => To as Node;

        public double Weight { get; set; }
    }
}