/* Copyright 2018 REAL.NET group
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
 
namespace WpfControlsLib.Constraints
{
    using System.Collections.Generic;
    using System.Linq;
    using Repo;

    /// <summary>
    /// Class for methods, working with constraint violations
    /// </summary>
    public class Constraints
    {
        private string modelName;

        public Constraints()
        {
            this.NodesAmount = 100;
            this.EdgesAmount = 100;
        }

        public int NodesAmount { get; set; }

        public int EdgesAmount { get; set; }

        public bool AllowCreateEdge(IEnumerable<IEdge> edges)
        {
            return this.Check(edges.Count() + 1, this.EdgesAmount);
        }

        public bool AllowCreateNode(IEnumerable<INode> nodes)
        {
            return this.Check(nodes.Count() + 1, this.NodesAmount);
        }

        public bool CheckEdges(IEnumerable<IEdge> edges)
        {
            return this.Check(edges.Count(), this.EdgesAmount);
        }

        public bool CheckNodes(IEnumerable<INode> nodes)
        {
            return this.Check(nodes.Count(), this.NodesAmount);
        }

        private bool Check(int a, int b)
        {
            return a < b;
        }
    }
}