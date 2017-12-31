/* Copyright 2017
 * Yurii Litvinov
 * Ivan Yarkov
 * Egor Zainullin
 * Denis Sushentsev
 * Arseniy Zavalishin
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

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
            this._edge = edge;
            this.From = source.Element;
            this.To = target.Element;
        }

        public IElement From
        {
            get => this._edge.From;
            set => this._edge.From = value;
        }

        public IElement To
        {
            get => this._edge.To;
            set => this._edge.To = value;
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