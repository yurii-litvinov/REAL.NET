using System;
using System.Collections.Generic;
using System.Linq;
using GraphX.Measure;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using QuickGraph;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class PaletteEdge : IEdge{
        private readonly IEdge _edgeImplementation;
        public string Name
        {
            get { return _edgeImplementation.Name; }
            set { _edgeImplementation.Name = value; }
        }

        public IElement Class
        {
            get { return _edgeImplementation.Class; }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return _edgeImplementation.Attributes; }
        }

        public bool IsAbstract
        {
            get { return _edgeImplementation.IsAbstract; }
        }

        public Metatype Metatype
        {
            get { return _edgeImplementation.Metatype; }
        }

        public Metatype InstanceMetatype
        {
            get { return _edgeImplementation.InstanceMetatype; }
        }

        public string Shape
        {
            get { return _edgeImplementation.Shape; }
        }

        public IElement From
        {
            get { return _edgeImplementation.From; }
            set { _edgeImplementation.From = value; }
        }

        public IElement To
        {
            get { return _edgeImplementation.To; }
            set { _edgeImplementation.To = value; }
        }

        public PaletteEdge(IEdge iedge)
        {
            _edgeImplementation = iedge;
        }
    }
    public sealed class Edge : Element, IEdge, IGraphXEdge<Node>
    {
        public IElement From { get; set; }
        public IElement To { get; set; }
        
        public Edge(IEdge iedge, Element from, Element to) : base(iedge)
        {
            
        }

        #region IGraphXEdge<Node> implementation
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
        #endregion
    }
}