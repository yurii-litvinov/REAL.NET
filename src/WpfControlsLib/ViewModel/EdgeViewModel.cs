﻿using GraphX.Measure;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Common.Models;
using Repo;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfControlsLib.ViewModel
{
    public class EdgeViewModel : EdgeBase<NodeViewModel>, INotifyPropertyChanged, IGraphXEdge<NodeViewModel>
    {
        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;
        private IList<AttributeViewModel> attributes = new List<AttributeViewModel>();
        private SolidColorBrush brush = Brushes.Blue;
        private bool isAllowed = true;

        private static readonly SolidColorBrush invalidEdgeBrush = Brushes.Red;
        private static readonly SolidColorBrush defaultEdgeBrush = Brushes.Blue;

        private string text;

        public EdgeViewModel(IEdge edge, NodeViewModel source, NodeViewModel target, double weight = 1)
            : base(source, target, weight)
        {
            this.Edge = edge;
            this.Angle = 90;
            // TO remove this hack, color should be saved in Repo
            if (this.Edge?.Class.Name == "TaggedLink")
            {
                this.Color = Brushes.Green;
            }
            else
            {
                this.Color = defaultEdgeBrush;    
            }
            
        }

        public EdgeViewModel(NodeViewModel source, NodeViewModel target, double weight = 1)
            : this(null, source, target, weight){ }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum EdgeTypeEnum
        {
            Generalization,
            Association,
            Type,
            Attribute,
        }

        public SolidColorBrush Color
        {
            get => this.brush;

            set
            {
                this.brush = value;
                this.OnPropertyChanged(nameof(this.Color));
            }
        }

        public bool IsAllowed
        {
            get => this.isAllowed;

            set
            {
                this.isAllowed = value;
                // TODO: Color property can be set by an user, and user color will be forgotten after IsAllowed change.
                this.Color = value ? defaultEdgeBrush : invalidEdgeBrush;
            }
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        public override Point[] RoutingPoints { get; set; }

        public int IndexOfInflectionPoint { get; set; }

        public IEdge Edge { get; }

        public string Text
        {
            get => this.text;

            set
            {
                this.text = value;
                this.OnPropertyChanged(nameof(this.Text));
            }
        }

        public IList<AttributeViewModel> Attributes
        {
            get => this.attributes;

            set
            {
                this.attributes = value;
                this.OnPropertyChanged();
            }
        }

        public string ToolTipText { get; set; }

        public EdgeTypeEnum EdgeType
        {
            get => this.edgeType;

            set
            {
                this.edgeType = value;
                this.OnPropertyChanged(nameof(this.EdgeType));
            }
        }

        public void OnPropertyChanged(string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString() => this.Text;
    }
}
