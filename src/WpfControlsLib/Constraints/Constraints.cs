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
    using Repo;

    /// <summary>
    /// Class for methods, working with constraint violations
    /// </summary>
    public class Constraints
    {
        private Dictionary<ValueTuple<string, string, string>, int> amountConstraints = new Dictionary<ValueTuple<string, string, string>, int>();

        public Constraints()
        {
        }

        public string ErrorMsg { get; set; }

        public bool Add(ValueTuple<string, string, string> constraints, int amount)
        {
            if (this.amountConstraints.ContainsKey(constraints))
            {
                this.amountConstraints[constraints] = amount;
                return false;
            }
            else
            {
                this.amountConstraints.Add(constraints, amount);
                return true;
            }
        }

        public bool AllowCreateOrExistEdge(IEnumerable<IEdge> edges, string prevVerName, string ctrlVerName, bool isCreating = true)
        {
            if (this.amountConstraints.Keys.Any(x => x.Item2 == prevVerName && x.Item3 == ctrlVerName))
            {
                return edges.Count(x => (x.From.ToString() == prevVerName) && (x.To.ToString() == ctrlVerName)) + Convert.ToInt32(isCreating) <= this.amountConstraints[new ValueTuple<string, string, string>("Node", prevVerName, ctrlVerName)];
            }

            if (this.amountConstraints.Keys.Any(x => x.Item2 == "All" && x.Item3 == ctrlVerName))
            {
                return edges.Count(x => (x.To.ToString() == ctrlVerName)) + Convert.ToInt32(isCreating) <= this.amountConstraints[new ValueTuple<string, string, string>("Node", "All", ctrlVerName)];
            }

            if (this.amountConstraints.Keys.Any(x => x.Item2 == prevVerName && x.Item3 == "All"))
            {
                return edges.Count(x => (x.From.ToString() == prevVerName)) + Convert.ToInt32(isCreating) <= this.amountConstraints[new ValueTuple<string, string, string>("Node", prevVerName, "All")];
            }

            if (this.amountConstraints.Keys.Any(x => x.Item1 == "EdgeViewModel" && x.Item3 == string.Empty))
            {
                return edges.Count() + Convert.ToInt32(isCreating) <= this.amountConstraints[new ValueTuple<string, string, string>("EdgeViewModel", "All", string.Empty)];
            }

                return true;
        }

        public bool AllowCreateOrExistNode(IEnumerable<INode> nodes, string nodeName, bool isCreating = true)
        {
            if (this.amountConstraints.Keys.Any(x => x.Item1 == nodeName && x.Item2 == string.Empty))
            {
                return nodes.Count(x => x.Name == nodeName) + Convert.ToInt32(isCreating) <= this.amountConstraints[new ValueTuple<string, string, string>("Node", nodeName, string.Empty)];
            }

            if (this.amountConstraints.Keys.Any(x => x.Item1 == "All" && x.Item2 == string.Empty))
            {
                return nodes.Count() + Convert.ToInt32(isCreating) <= this.amountConstraints[new ValueTuple<string, string, string>("Node", "All", string.Empty)];
            }

            return true;
        }

        public bool Check(IEnumerable<IEdge> edges, IEnumerable<INode> nodes)
        {
            this.ErrorMsg = string.Empty;
            var sb = new System.Text.StringBuilder(string.Empty);
            var result = true;

            foreach (var edge in edges)
            {
                if (!this.AllowCreateOrExistEdge(edges, edge.From.ToString(), edge.ToString(), false))
                {
                    sb.Append("Constraints amount error at edge ");
                    sb.Append(edge.From.ToString()); //TODO delete duplicatind errors
                    sb.Append(" -> ");
                    sb.Append(edge.To.ToString());
                    result = false;
                }
            }

            foreach (var node in nodes)
            {
                if (!this.AllowCreateOrExistNode(nodes, node.Name, false))
                {
                    sb.Append("Constraints amount error at node ");
                    sb.Append(node.Name); //TODO delete duplicatind errors
                    result = false;
                }
            }

            return result;
        }
    }
}