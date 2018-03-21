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
    using System;
    using System.Collections.Generic;

    public class RepoInfo
    {
        private readonly Repo.IRepo repo;
        private readonly string modelName;

        public RepoInfo(Repo.IRepo inputRepo, string inputModelName)
        {
            this.repo = inputRepo;
            this.modelName = inputModelName;
        }

        public IEnumerable<Repo.IEdge> GetEdges()
        {
            return this.repo.Model(this.modelName).Edges;
        }

        public IEnumerable<Repo.INode> GetNodes()
        {
            return this.repo.Model(this.modelName).Nodes;
        }

        public List<string> GetNodeTypes()
        {
            var types = new List<string>();
            types.Add("All");
            foreach (var node in this.repo.Model(this.modelName).Nodes)
            {
                var typeName = Convert.ToString(node.Name);
                if (!types.Contains(typeName))
                {
                    types.Add(typeName);
                }
            }

            return types;
        }

        public List<string> GetEdgeTypes()
        {
            var types = new List<string>();
            types.Add("All");
            foreach (var edge in this.repo.Model(this.modelName).Edges)
            {
                var typeName = Convert.ToString(edge.Name);
                if (!types.Contains(typeName))
                {
                    types.Add(typeName);
                }
            }

            return types;
        }
    }
}
