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
    using System.Text.RegularExpressions;
    using EditorPluginInterfaces;
    using Repo;
    using WpfControlsLib.ViewModel;

    /// <summary>
    /// Class that keeps and checks model constraints.
    /// </summary>
    public class Constraints
    {

        private Dictionary<ConstraintElement, (int, int)> amountConstraints
            = new Dictionary<ConstraintElement, (int, int)>();

        private Dictionary<ConstraintElement, object> attributeConstraints
            = new Dictionary<ConstraintElement, object>();

        private List<ConstraintItem> constraintsItems = new List<ConstraintItem>();

        public Constraints()
        {
        }

        public event EventHandler<ConstraintsEventArgs> OnAmountChange;

        public event EventHandler<ConstraintsEventArgs> OnAttributeChange;

        public List<ConstraintItem> ConstraintsList
        {
            get => this.constraintsItems;
        }

        public string ErrorMsg { get; set; }

        public bool AddAmountConstraint(ConstraintElement constraint, object value)
        {
            var args = new ConstraintsEventArgs
            {
                ObjName = constraint.ObjectType
            };
            var amount = (System.ValueTuple<int, int>)value;
            if (this.amountConstraints.ContainsKey(constraint))
            {
                this.amountConstraints[constraint] = amount;
                this.OnAmountChange(this, args);
                return false;
            }
            else
            {
                this.amountConstraints.Add(constraint, amount);
                this.OnAmountChange(this, args);
                return true;
            }
        }

        public void DeleteAmountConstraint(ConstraintElement constraint)
        {
            if (this.amountConstraints.ContainsKey(constraint))
            {
                this.amountConstraints.Remove(constraint);
                this.RemoveConstraintItem(constraint);
                var args = new ConstraintsEventArgs
                {
                    ObjName = constraint.ObjectType
                };
                this.OnAmountChange(this, args);
            }
        }

        public void DeleteAttributeConstraint(ConstraintElement constraint)
        {
            if (this.attributeConstraints.ContainsKey(constraint))
            {
                this.attributeConstraints.Remove(constraint);
                this.RemoveConstraintItem(constraint);
                var args = new ConstraintsEventArgs
                {
                    ObjName = constraint.ObjectType,
                    AttributeName = constraint.AttributeName
                };
                this.OnAttributeChange(this, args);
            }
        }

        public bool AddAttributeConstraint(ConstraintElement constraint, object value)
        {
            var args = new ConstraintsEventArgs
            {
                ObjName = constraint.ObjectType,
                AttributeName = constraint.AttributeName
            };
            if (this.attributeConstraints.ContainsKey(constraint))
            {
                this.attributeConstraints[constraint] = value;
                this.OnAttributeChange(this, args);
                return false;
            }
            else
            {
                this.attributeConstraints.Add(constraint, value);
                this.OnAttributeChange(this, args);
                return true;
            }
        }

        public bool CheckEdge(List<EdgeViewModel> edges, string prevVerName, string ctrlVerName, bool isCreating = true)
        {
            // Check amount of certain edges from prevVertex to prevVertex
            if (this.amountConstraints.Keys.Any(x => x.ElementType1 == prevVerName && x.ElementType2 == ctrlVerName))
            {
                var amountOfExistingEdges = edges.Count(x => (x.Source.Name == prevVerName)
                    && (x.Target.Name == ctrlVerName)) + Convert.ToInt32(isCreating);
                var constraint = new ConstraintElement
                {
                    ObjectType = "Edge",
                    ElementType1 = prevVerName,
                    ElementType2 = ctrlVerName
                };
                var from = this.amountConstraints[constraint].Item1;
                var to = this.amountConstraints[constraint].Item2;
                return (from <= amountOfExistingEdges + Convert.ToInt32(isCreating))
                    && (amountOfExistingEdges + Convert.ToInt32(isCreating) <= to);
            }

            // Check amount of edges from anyVertex to ctrlVertex
            if (this.amountConstraints.Keys.Any(x => x.ElementType1 == "All" && x.ElementType2 == ctrlVerName))
            {
                var amountOfExistingEdges = edges.Count(x => (x.Target.Name == ctrlVerName)) + Convert.ToInt32(isCreating);
                var constraint = new ConstraintElement
                {
                    ObjectType = "Edge",
                    ElementType1 = "All",
                    ElementType2 = ctrlVerName
                };
                var from = this.amountConstraints[constraint].Item1;
                var to = this.amountConstraints[constraint].Item2;
                return (from <= amountOfExistingEdges + Convert.ToInt32(isCreating))
                    && (amountOfExistingEdges + Convert.ToInt32(isCreating) <= to);
            }

            // Check amount of edges from prevVertex to anyVertex
            if (this.amountConstraints.Keys.Any(x => x.ElementType1 == prevVerName && x.ElementType2 == "All"))
            {
                var amountOfExistingEdges = edges.Count(x => (x.Source.Name == prevVerName)) + Convert.ToInt32(isCreating);
                var constraint = new ConstraintElement
                {
                    ObjectType = "Edge",
                    ElementType1 = prevVerName,
                    ElementType2 = "All"
                };
                var from = this.amountConstraints[constraint].Item1;
                var to = this.amountConstraints[constraint].Item2;
                return (from <= amountOfExistingEdges + Convert.ToInt32(isCreating))
                    && (amountOfExistingEdges + Convert.ToInt32(isCreating) <= to);
            }

            // Check general amount of edges
            if (this.amountConstraints.Keys.Any(x => x.ObjectType == "Edge" && x.ElementType1 == "All"))
            {
                var amountOfExistingEdges = edges.Count() + Convert.ToInt32(isCreating);
                var constraint = new ConstraintElement
                {
                    ObjectType = "Edge",
                    ElementType1 = "All",
                    ElementType2 = string.Empty
                };
                var from = this.amountConstraints[constraint].Item1;
                var to = this.amountConstraints[constraint].Item2;
                return (from <= amountOfExistingEdges + Convert.ToInt32(isCreating))
                    && (amountOfExistingEdges + Convert.ToInt32(isCreating) <= to);
            }

            return true;
        }

        public bool CheckNode(List<NodeViewModel> nodes, string nodeName, bool isCreating = true)
        {
            // Check amount of certain nodeTypes
            if (this.amountConstraints.Keys.Any(x => x.ObjectType == "Node" && x.ElementType1 == nodeName && x.ElementType2 == string.Empty))
            {
                var amountOfExistingNodes = nodes.Count(x => x.Name == nodeName) + Convert.ToInt32(isCreating);
                var constraint = new ConstraintElement
                {
                    ObjectType = "Node",
                    ElementType1 = nodeName,
                    ElementType2 = string.Empty
                };
                var from = this.amountConstraints[constraint].Item1;
                var to = this.amountConstraints[constraint].Item2;
                return (from <= amountOfExistingNodes + Convert.ToInt32(isCreating))
                    && (amountOfExistingNodes + Convert.ToInt32(isCreating) <= to);
            }

            // Check general amount of nodes
            if (this.amountConstraints.Keys.Any(x => x.ObjectType == "Node" && x.ElementType1 == "All" && x.ElementType2 == string.Empty))
            {
                var amountOfExistingNodes = nodes.Count(x => x.Name == nodeName) + Convert.ToInt32(isCreating);
                var constraint = new ConstraintElement
                {
                    ObjectType = "Node",
                    ElementType1 = "All",
                    ElementType2 = string.Empty
                };
                var from = this.amountConstraints[constraint].Item1;
                var to = this.amountConstraints[constraint].Item2;
                return (from <= amountOfExistingNodes + Convert.ToInt32(isCreating))
                    && (amountOfExistingNodes + Convert.ToInt32(isCreating) <= to);
            }

            return true;
        }

        public bool AllowSetEdgeAttributeValue(EdgeViewModel edge, AttributeViewModel attribute, string newValue)
        {
            if (this.attributeConstraints.Keys.Any(x => x.ObjectType == "Edge" && x.ElementType1 == edge.EdgeType.ToString()
                                                                            && x.AttributeName == attribute.Name))
            {
                var constraint = new ConstraintElement
                {
                    ObjectType = "Edge",
                    ElementType1 = edge.EdgeType.ToString(),
                    AttributeName = attribute.Name
                };
                if (attribute.Type == "Boolean")
                {
                    return newValue.ToLower() == this.attributeConstraints[constraint].ToString().ToLower();
                }

                if (attribute.Type == "Int")
                {
                    var constraintValue = (ValueTuple<int, int>)this.attributeConstraints[constraint];
                    return (constraintValue.Item1 <= Convert.ToInt32(newValue)) 
                        && (Convert.ToInt32(newValue) <= constraintValue.Item2);
                }

                if (attribute.Type == "String")
                {
                    var constraintValue = this.attributeConstraints[constraint].ToString();
                    Regex regex = new Regex(constraintValue);
                    return regex.IsMatch(newValue);
                }
            }

            return true;
        }

        public bool AllowSetNodeAttributeValue(NodeViewModel node, AttributeViewModel attribute, string newValue)
        {
            if (this.attributeConstraints.Keys.Any(x => x.ObjectType == "Node"
                            && x.ElementType1 == node.Name && x.AttributeName == attribute.Name))
            {
                var constraint = new ConstraintElement
                {
                    ObjectType = "Node",
                    ElementType1 = node.Name,
                    ElementType2 = "",
                    AttributeName = attribute.Name
                };
                if (attribute.Type == "Boolean")
                {
                    return newValue.ToLower() == this.attributeConstraints[constraint].ToString().ToLower();
                }

                if (attribute.Type == "Int")
                {
                    var constraintValue = (ValueTuple<int, int>)this.attributeConstraints[constraint];
                    return (constraintValue.Item1 <= Convert.ToInt32(newValue)) 
                                        && (Convert.ToInt32(newValue) <= constraintValue.Item2);
                }

                if (attribute.Type == "String")
                {
                    var constraintValue = this.attributeConstraints[constraint].ToString();
                    Regex regex = new Regex(constraintValue);
                    return regex.IsMatch(newValue);
                }
            }

                return true;
        }

        public bool Check(List<EdgeViewModel> edges, List<NodeViewModel> nodes)
        {
            this.ErrorMsg = string.Empty;
            var sb = new System.Text.StringBuilder(string.Empty);
            sb.Append("-------------------------------");
            sb.Append(Environment.NewLine);
            sb.Append("Check at ");
            sb.Append(DateTime.Now.TimeOfDay);
            sb.Append(Environment.NewLine);
            var result = true;

            var errLines = new List<string>();
            foreach (var edge in edges)
            {
                foreach (var attribute in edge.Attributes)
                {
                    if (!this.AllowSetEdgeAttributeValue(edge, attribute, attribute.Value))
                    {
                        var line = "ERROR: Constraints attribute error at edge " + edge.Source.Name + " -> "
                            + edge.Target.Name + ", check the " + attribute.Name + " value.";
                        if (!errLines.Contains(line))
                        {
                            errLines.Add(line);
                        }

                        errLines.Add(line);
                        result = false;
                    }
                }

                if (!this.CheckEdge(edges, edge.Source.Name, edge.Target.Name, false))
                {
                    var line = "ERROR: Constraints amount error at edge " + edge.Source.Name + " -> " + edge.Target.Name;
                    if (!errLines.Contains(line))
                    {
                        errLines.Add(line);
                    }

                    result = false;
                }
            }

            foreach (var node in nodes)
            {
                foreach (var attribute in node.Attributes)
                {
                    if (!this.AllowSetNodeAttributeValue(node, attribute, attribute.Value))
                    {
                        var line = "ERROR: Constraints amount error at node " + node.Name
                            + ", check the " + attribute.Name + " value.";
                        if (!errLines.Contains(line))
                        {
                            errLines.Add(line);
                        }

                        result = false;
                    }
                }

                if (!this.CheckNode(nodes, node.Name, false))
                {
                    var line = "ERROR: Constraints amount error at node " + node.Name;
                    if (!errLines.Contains(line))
                    {
                        errLines.Add(line);
                    }

                    result = false;
                }
            }

            errLines.ForEach(x => sb.Append(x + Environment.NewLine));
            this.ErrorMsg = sb.ToString();
            return result;
        }

        private void RemoveConstraintItem(ConstraintElement constraints)
        {
            var item = this.ConstraintsList.FirstOrDefault(x => (x.ObjectType == constraints.ObjectType)
                && (x.ElementTypes.Item1 == constraints.ElementType1)
                && ((x.ElementTypes.Item2 == constraints.ElementType2) || (x.AttributeName == constraints.AttributeName)));
            this.ConstraintsList.Remove(item);
        }

        public struct ConstraintElement
        {
            public string ObjectType;
            public string ElementType1;
            public string ElementType2;
            public string AttributeName;
        }
    }
}