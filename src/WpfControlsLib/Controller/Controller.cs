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

namespace WpfControlsLib.Controller
{
    using GraphX.Controls;
    using ViewModel;

    /// <summary>
    /// Controller in MVC architecture. Supposed to be handling commands, but for now it modifies model
    /// by itself.
    /// </summary>
    public class Controller
    {
        private readonly Model.Model model;

        public Controller(Model.Model model)
        {
            this.model = model;
        }

        public void NewNode(Repo.IElement node)
        {
            this.model.CreateNode(node);
        }

        public void NewEdge(Repo.IElement edge, VertexControl prevVer, VertexControl ctrlVer)
        {
            this.model.CreateEdge(
                edge as Repo.IEdge,
                (prevVer?.Vertex as NodeViewModel)?.Node,
                (ctrlVer?.Vertex as NodeViewModel)?.Node);
        }

        public void RemoveElement(Repo.IElement element) => this.model.RemoveElement(element);
    }
}