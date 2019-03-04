
namespace ConstraintsMatcher
{
    using Repo;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            if ((this.root.Class.Name == "NotNode") || (this.root.Class.Name == "OrNode") || (this.root.Class.Name == "NoNodes"))
            {
                this.ErrorMsg = "The root should be NodeType from modrl or AllNodes.";
                return false;
            }
            var notNodes = constraintsModel.Nodes.Where(x => x.Class.Name == "NotNode");
            foreach(var notNode in notNodes)
            {
                if (constraintsModel.Edges.Where(x => x.From == notNode).ToList().Count() > 1)
                {
                    return false;
                }
            }
            //TODO check if tree
            return true;
        }

        public bool Check(INode originRoot, INode constraintsRoot, IModel constraintsModel)
        {
            return this.InnerCheck(originRoot, constraintsRoot, constraintsModel);
        }

        private bool InnerCheck(INode targetRoot, INode constraintsRoot, IModel constraintsModel)
        {
            var outTargetEdges = this.targetModel.Edges.Where(x => x.From == targetRoot);
            var outConstraintsEdges = constraintsModel.Edges.Where(x => x.From == constraintsRoot);
            var areIdentic = ElementsAreIdentic(targetRoot, constraintsRoot);
            if (constraintsRoot.Class.Name == "AllNodes")
            {
                areIdentic = true;
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
                var innerCheck = outParentTargetEdges.Any(x => ElementsAreIdentic(constraintParentEdge, x) && this.InnerCheck((INode)x.To, constraintsToRoot, constraintsModel));
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

        public bool ElementsAreIdentic(IElement firstElement, IElement secondElement)
        {
            if (firstElement.Class.Name != secondElement.Class.Name)
            {
                return false;
            }
            foreach(var attr in firstElement.Attributes)
            {
                if (secondElement.Attributes.Where(x => x.Name == attr.Name).FirstOrDefault().StringValue != attr.StringValue)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
