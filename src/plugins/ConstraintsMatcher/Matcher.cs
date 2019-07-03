
namespace ConstraintsMatcher
{
    using Repo;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class Matcher
    {
        public Repo.IRepo Repo { get; private set; }

        public string ErrorMsg;

        public INode root; //TODO PLOHO

        private IModel targetModel;
        public Matcher(string modelName)
        {
            this.Repo = global::Repo.RepoFactory.Create();
            this.targetModel = this.Repo.Model(modelName);
        }

        public Matcher(IModel model)
        {
            this.Repo = global::Repo.RepoFactory.Create();
            this.targetModel = model;
        }

        public bool PreCheck(IModel constraintsModel)
        {
            var Nodes = new List<INode>();
            //Find the root
            foreach (var node in constraintsModel.Nodes)
            {
                var inEdges = constraintsModel.Edges.Where(x => x.To == node).ToList();
                if (inEdges.Count() == 0)
                {
                    Nodes.Add(node);
                }
            }
            if ((Nodes.Count() > 1) || (Nodes.Count() == 0))
            {
                this.ErrorMsg = "There should be exactly one root.";
                return false;
            }
            this.root = Nodes.FirstOrDefault();
            // Check if root is correct
            if ((this.root.Class.Name == "NotNode") || (this.root.Class.Name == "OrNode") || (this.root.Class.Name == "NoNodes"))
            {
                this.ErrorMsg = "The root should be NodeType from model or AllNodes.";
                return false;
            }
            // Check ther there is only one outcoming edge from notNode
            var notNodes = constraintsModel.Nodes.Where(x => x.Class.Name == "NotNode");
            foreach(var notNode in notNodes)
            {
                if (constraintsModel.Edges.Where(x => x.From == notNode).ToList().Count() > 1)
                {
                    this.ErrorMsg = "There should be only one outgoing edge from Not Node.";
                    return false;
                }
            }
            //Check if there is no outgoing edges from noNodes
            var noNodes = constraintsModel.Nodes.Where(x => x.Class.Name == "NoNodes");
            foreach (var noNode in noNodes)
            {
                if (constraintsModel.Edges.Where(x => x.From == noNode).ToList().Count() > 0)
                {
                    this.ErrorMsg = "There shouldn't be outgoing edges from NoNodes";
                    return false;
                }
            }
            // Check if this constraint is a tree
            var visited = new HashSet<int>();
            if (!this.CheckIfTree(constraintsModel, this.root, visited))
            {
                this.ErrorMsg = "Constraints graph should have a tree structure.";
                return false;
            }
            
            return true;
        }

        public bool CheckIfTree(IModel constraintsModel, INode root, HashSet<int> visited)
        {
            visited.Add(root.GetHashCode());
            var outgoingEdges = constraintsModel.Edges.Where(x => x.From == root);
            foreach(var edge in outgoingEdges)
            {
                if (visited.Any(x => x == edge.To.GetHashCode()))
                {
                    return false;
                }
                if (!CheckIfTree(constraintsModel, (INode)edge.To, visited))
                {
                    return false;
                }
            }
            return true;
        }
        public bool Check(INode originRoot, INode constraintsRoot, IModel constraintsModel)
        {
                return this.InnerCheck(originRoot, constraintsRoot, constraintsModel);
        }

        public bool AttrCheck(INode originRoot, INode constraintsRoot)
        {
            return this.AttributesAreIdentic(originRoot, constraintsRoot);
        }

        private bool InnerCheck(INode targetRoot, INode constraintsRoot, IModel constraintsModel)
        {
            var outTargetEdges = this.targetModel.Edges.Where(x => x.From == targetRoot);
            var outConstraintsEdges = constraintsModel.Edges.Where(x => x.From == constraintsRoot);
            var areIdentic = ElementsAreIdentic(targetRoot, constraintsRoot);
            if (constraintsRoot.Class.Name == "AllNodes")
            {
                areIdentic = this.AttributesAreIdentic(targetRoot, constraintsRoot);
            }
            if (constraintsRoot.Class.Name == "AnyNodes")
            {

            }
            if (constraintsRoot.Class.Name == "OrNode")
            {
                var targetParentRoot = (INode)this.targetModel.Edges.Where(x => x.To == targetRoot).FirstOrDefault().From;
                outTargetEdges = this.targetModel.Edges.Where(x => x.From == targetParentRoot);
                var result = false;
                foreach(var constraintEdge in outConstraintsEdges)
                {
                    foreach(var targetEdge in outTargetEdges)
                    {
                        var targetToNode = (INode)targetEdge.To;
                        var constraintsToNode = (INode)constraintEdge.To;
                        if (ElementsAreIdentic(targetToNode, constraintsToNode))
                        {
                            result = result || this.InnerCheck(targetToNode, constraintsToNode, constraintsModel);
                        }
                    }
                }
                return result;
            }
            if (constraintsRoot.Class.Name == "NotNode")
            {
                var constraintParentEdge = constraintsModel.Edges.Where(x => x.To == constraintsRoot).FirstOrDefault();
                var constraintsToRoot = (INode)outConstraintsEdges.FirstOrDefault().To;
                var targetParentRoot = (INode)this.targetModel.Edges.Where(x => x.To == targetRoot).FirstOrDefault().From;
                var outParentTargetEdges = this.targetModel.Edges.Where(x => x.From == targetParentRoot);
                var innerCheck = outParentTargetEdges.Any(x => ElementsAreIdentic(x, constraintParentEdge) && this.InnerCheck((INode)x.To, constraintsToRoot, constraintsModel));
                return !innerCheck;
            }
            if ((outConstraintsEdges.Count() > 0)&&(outConstraintsEdges.FirstOrDefault().To.Class.Name == "NoNodes"))
            {
                if (outTargetEdges.Count() > 0)
                    return false;
                return areIdentic;
            }
            if (!areIdentic)
            {
                return false;
            }
            foreach (var constraintsEdge in outConstraintsEdges) //TODO there may not to be any outgoing edges
            {
                if ((outTargetEdges.Count() == 0))
                {
                    if (constraintsEdge.To.Class.Name == "NoNodes" || constraintsEdge.To.Class.Name == "NotNode")
                        return true;
                    else return false;
                }
                var innerCheck = outTargetEdges.Any(x => ElementsAreIdentic(x, constraintsEdge)&& this.InnerCheck((INode)x.To, (INode)constraintsEdge.To, constraintsModel));
                if (!innerCheck)
                {
                    return false;
                }
            }
                return true;
        }

        private bool FindEdge(IEnumerable<IEdge> outTargetEdges, IEdge edge)
        {
            foreach(var targetEdge in outTargetEdges)
            {
                if (ElementsAreIdentic(targetEdge, edge))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ElementsAreIdentic(IElement targetElement, IElement constraintsElement)
        {
            if ((targetElement.Class.Name != constraintsElement.Class.Name)||!AttributesAreIdentic(targetElement, constraintsElement))
            {
                return false;
            }
            return true;
        }

        public bool AttributesAreIdentic(IElement targetElement, IElement constraintsElement)
        {
            if (targetElement.Attributes.Count() != constraintsElement.Attributes.Count())
                return false;
            foreach (var targetAttr in targetElement.Attributes)
            {
                if (targetAttr.StringValue != "Link")
                {
                    var constraintsAttr = constraintsElement.Attributes.Where(x => x.Name == targetAttr.Name).FirstOrDefault();

                    if (constraintsAttr.StringValue == "*")
                        return true;

                    if ((constraintsAttr.StringValue.Length > 3) && (constraintsAttr.StringValue.Substring(0, 2) == "<="))
                    {
                        var value = constraintsAttr.StringValue.Substring(2);
                        return (Convert.ToInt32(targetAttr.StringValue) <= Convert.ToInt32(value));
                    }
                    else if ((constraintsAttr.StringValue.Length > 3) && (constraintsAttr.StringValue.Substring(0, 2) == ">="))
                    {
                        var value = constraintsAttr.StringValue.Substring(2);
                        return (Convert.ToInt32(targetAttr.StringValue) >= Convert.ToInt32(value));
                    }
                    else if ((constraintsAttr.StringValue.Length > 2) && (constraintsAttr.StringValue[0] == '<'))
                    {
                        var value = constraintsAttr.StringValue.Substring(1);
                        return (Convert.ToInt32(targetAttr.StringValue) < Convert.ToInt32(value));
                    }
                    else if ((constraintsAttr.StringValue.Length > 2) && (constraintsAttr.StringValue[0] == '>'))
                    {
                        var value = constraintsAttr.StringValue.Substring(1);
                        return (Convert.ToInt32(targetAttr.StringValue) > Convert.ToInt32(value));
                    }
                    else
                    {
                        Regex regEx = new Regex(constraintsAttr.StringValue);
                        if (!regEx.IsMatch(targetAttr.StringValue))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
