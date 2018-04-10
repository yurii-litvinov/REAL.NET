/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace WpfControlsLib.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EditorPluginInterfaces;
    using WpfControlsLib.ViewModel;

    /// <summary>
    /// Model in MVC architecture. Wraps repository, provides operations like adding or removing models, edges and
    /// nodes (note that "model" is used with two different meanings around the project -- a visual model consisting
    /// of nodes and edges and a model in MVC sense) and notifications for all concerned tools about changes in repo.
    /// This class is a ground truth about visual model currently edited and is supposed to be used by all tools and
    /// parts of an editor who need to listen for visual model changes and/or modify visual model.
    /// </summary>
    public class Model : IModel
    {
        private List<NodeViewModel> nodesList = new List<NodeViewModel>();

        private List<EdgeViewModel> edgesList = new List<EdgeViewModel>();

        public Model()
        {
            this.Repo = global::Repo.RepoFactory.CreateRepo();
            this.Constraints = new Constraints.Constraints();
            this.Constraints.OnAmountChange += (sender, args) => this.OnAmountConstraintsChange(args.ObjName);
            this.Constraints.OnAttributeChange += (sender, args) =>
                this.OnAttributeConstraintsChange(args.ObjName, args.AttributeName);
        }

        public event EventHandler<VertexEventArgs> NewVertex;

        public event EventHandler<EdgeEventArgs> NewEdge;

        public Constraints.Constraints Constraints { get; set; }

        public List<NodeViewModel> NodesList
        {
            get
            {
                return this.nodesList;
            }

            set
            {
                this.nodesList = value;
            }
        }

        public List<EdgeViewModel> EdgesList
        {
            get
            {
                return this.edgesList;
            }

            set
            {
                this.edgesList = value;
            }
        }

public string ModelName { get; set; }

        public string ErrorMsg { get; set; }

        public Repo.IRepo Repo { get; }

        public void OnAmountConstraintsChange(string objName)
        {
            switch (objName)
            {
                case "Node":
                    foreach (var node in this.nodesList)
                    {
                        node.IsAllowed = this.Constraints.CheckNode(this.nodesList, node.Name);
                    }

                    break;
                case "Edge":
                    foreach (var edge in this.edgesList)
                    {
                        edge.IsAllowed
                            = this.Constraints.CheckEdge(this.edgesList, edge.Source.Name, edge.Target.Name);
                    }

                    break;
            }
        }

        public void OnAttributeConstraintsChange(string objName, string attributeName)
        {
            switch (objName)
            {
                case "Node":
                    foreach (var node in this.nodesList)
                    {
                        var attribute = node.Attributes.FirstOrDefault(x => x.Name == attributeName);
                        if (attribute != null)
                        {
                            attribute.HasAllowedValue
                                = this.Constraints.AllowSetNodeAttributeValue(node, attribute, attribute.Value);
                        }
                    }

                    break;
                case "Edge":
                    foreach (var edge in this.edgesList)
                    {
                        var attribute = edge.Attributes.Single(x => x.Name == attributeName);
                        attribute.HasAllowedValue
                            = this.Constraints.AllowSetEdgeAttributeValue(edge, attribute, attribute.Value);
                    }

                    break;
            }
        }

        public void OnNodeAttributeChanged(NodeViewModel node, AttributeViewModel attribute, string newValue)
        {
            attribute.HasAllowedValue = this.Constraints.AllowSetNodeAttributeValue(node, attribute, newValue);
        }

        public void OnEdgeAttributeChanged(EdgeViewModel edge, AttributeViewModel attribute, string newValue)
        {
            attribute.HasAllowedValue = this.Constraints.AllowSetEdgeAttributeValue(edge, attribute, newValue);
        }

        // TODO name
        public bool ConstraintsCheck()
        {
            var check = this.Constraints.Check(this.edgesList, this.nodesList);
            this.ErrorMsg = this.Constraints.ErrorMsg;
            return check;
        }

        public void CreateNode(Repo.IElement element)
        {
                var model = this.Repo.Model(this.ModelName);
                var newNode = model.CreateElement(element) as Repo.INode;
                this.RaiseNewVertex(newNode);
        }

        public void CreateEdge(Repo.IEdge edge, Repo.IElement prevVer, Repo.IElement ctrlVer)
        {
            this.RaiseNewEdge(edge, prevVer, ctrlVer);
        }

        public bool EdgeIsAllowed(Repo.IElement prevVer, Repo.IElement ctrlVer)
        {
                return this.Constraints.CheckEdge(
                    this.edgesList, prevVer.Name.ToString(), ctrlVer.Name.ToString());
        }

        public bool NodeIsAllowed(string nodeName)
        {
            return this.Constraints.CheckNode(this.nodesList, nodeName);
        }

        private void RaiseNewVertex(Repo.INode node)
        {
            var args = new VertexEventArgs
            {
                Node = node
            };
            this.NewVertex?.Invoke(this, args);
        }

        private void RaiseNewEdge(Repo.IEdge edge, Repo.IElement prevVer, Repo.IElement ctrlVer)
        {
            var args = new EdgeEventArgs
            {
                Edge = edge,
                Source = prevVer,
                Target = ctrlVer
            };
            this.NewEdge?.Invoke(this, args);
        }
    }
}
