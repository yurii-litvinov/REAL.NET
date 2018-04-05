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
    using System.Linq;

    /// <summary>
    /// This class provides repo information to constraints window.
    /// </summary>
    public class RepoInfo
    {
        private readonly Repo.IRepo repo;
        private readonly string modelName;

        public RepoInfo(Repo.IRepo inputRepo, string inputModelName)
        {
            this.repo = inputRepo;
            this.modelName = inputModelName;
        }

        /// <summary>
        /// Gets node types of current model.
        /// </summary>
        /// <returns>All node types of current model</returns>
        public List<string> GetNodeTypes()
        {
            var types = new List<string>() { string.Empty, "All" };
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

        /// <summary>
        /// Gets edge types of current model.
        /// </summary>
        /// <returns>All edge types of current model.</returns>
        public List<string> GetEdgeTypes()
        {
            var types = new List<string>() { string.Empty, "All" };
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

        /// <summary>
        /// Gets node attributes by node name.
        /// </summary>
        /// <returns>Attributes of selected node type</returns>
        /// <param name="typeName">Node name</param>
        public List<string> GetNodeAttributes(string typeName)
        {
            var attributes = new List<string>() { string.Empty };
            foreach (var node in this.repo.Model(this.modelName).Nodes)
            {
                if (node.Name.ToString() == typeName)
                {
                    foreach (var attribute in node.Attributes)
                    {
                        attributes.Add(attribute.Name.ToString());
                    }

                    return attributes;
                }
            }

            return attributes;
        }

        /// <summary>
        /// Gets edge attributes
        /// </summary>
        /// <returns>List of attributes of selected edge type.</returns>
        /// <param name="typeName">Edge name</param>
        public List<string> GetEdgeAttributes(string typeName)
        {
            var attributes = new List<string>() { string.Empty };
            foreach (var edge in this.repo.Model(this.modelName).Edges)
            {
                if (edge.Name.ToString() == typeName)
                {
                    foreach (var attribute in edge.Attributes)
                    {
                        attributes.Add(attribute.Name.ToString());
                        var a = attribute.Kind;
                    }

                    return attributes;
                }
            }

            return attributes;
        }

        /// <summary>
        /// Returns type of selected attribute
        /// </summary>
        /// <returns>Attribute type as string</returns>
        /// <param name="obj">Object type (node or edge)</param>
        /// <param name="element">element name</param>
        /// <param name="attribute">attribute name</param>
        public string GetAttributeType(string obj, string element, string attribute)
        {
            if (obj == "Node")
            {
                foreach (var node in this.repo.Model(this.modelName).Nodes)
                {
                    if (node.Name.ToString() == element)
                    {
                        var a = node.Attributes.Single(x => x.Name == attribute);
                        return a.Kind.ToString();
                    }
                }
            }
            else
            {
                foreach (var edge in this.repo.Model(this.modelName).Edges)
                {
                    if (edge.Name.ToString() == element)
                    {
                        var a = edge.Attributes.Single(x => x.Name == attribute);
                        return a.Kind.ToString();
                    }
                }
            }

            return string.Empty;
        }
    }
}
