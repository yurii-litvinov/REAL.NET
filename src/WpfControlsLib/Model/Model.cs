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
    using EditorPluginInterfaces;

    /// <summary>
    /// Model in MVC architecture. Wraps repository, provides operations like adding or removing models, edges and
    /// nodes (note that "model" is used with two different meanings around the project -- a visual model consisting
    /// of nodes and edges and a model in MVC sense) and notifications for all concerned tools about changes in repo.
    /// This class is a ground truth about visual model currently edited and is supposed to be used by all tools and
    /// parts of an editor who need to listen for visual model changes and/or modify visual model.
    /// </summary>
    public class Model : IModel
    {
        public Model()
        {
            this.Repo = global::Repo.RepoFactory.CreateRepo();
            this.Constraints = new Constraints.Constraints();
        }

        public event EventHandler<VertexEventArgs> NewVertex;

        public event EventHandler<EdgeEventArgs> NewEdge;

        public Constraints.Constraints Constraints { get; set; }

        public string ModelName { get; set; }

        public string ErrorMsg { get; set; }

        public Repo.IRepo Repo { get; }

        // TODO name
        public bool ConstraintsCheck()
        {
            this.Constraints.Check(this.Repo.Model(this.ModelName).Edges, this.Repo.Model(this.ModelName).Nodes);
            return true;

        }

        public void CreateNode(Repo.IElement element)
        {
            if (this.Constraints.AllowCreateOrExistNode(this.Repo.Model(this.ModelName).Nodes, "a" + element.Name.ToString())) // TODO something more pretty than +"a"
            {
                var model = this.Repo.Model(this.ModelName);
                var newNode = model.CreateElement(element) as Repo.INode;
                this.RaiseNewVertex(newNode);
            }
            else
            {
                this.ErrorMsg = "Can't create new node according to constraints.";

                // TODO
            }
        }

        public void CreateEdge(Repo.IEdge edge, Repo.IElement prevVer, Repo.IElement ctrlVer)
        {
            if (this.Constraints.AllowCreateOrExistEdge(this.Repo.Model(this.ModelName).Edges, prevVer.Name.ToString(), ctrlVer.Name.ToString()))
            {
                // TODO: Well, actually create edge.
                this.RaiseNewEdge(edge, prevVer, ctrlVer);
            }
            else
            {
                this.ErrorMsg = "Can't create new edge according to constraints.";

                // TODO
            }
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
