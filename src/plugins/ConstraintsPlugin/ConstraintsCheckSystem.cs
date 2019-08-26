using ConstraintsMatcher;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfControlsLib.Model;

namespace ConstraintsPlugin
{
    class ConstraintsCheckSystem
    {
        // nodeName and modelName
        private List<Constraint> constraints;

        private Model targetModel;
        private IModel repoModel;
        private Matcher matcher;
        private Graph graph;
        public ConstraintsCheckSystem(Model targetModel, Graph graph)
        {
            this.constraints = new List<Constraint>();
            this.targetModel = targetModel;
            this.repoModel = targetModel.Repo.Model(targetModel.ModelName);
            this.graph = graph;
            this.matcher = new Matcher(repoModel);
        }

        public string AddConstraint(Model constraintModel, int unitHash)
        {
            var repoConstraintModel = constraintModel.Repo.Model(constraintModel.ModelName);
            if (!matcher.PreCheck(repoConstraintModel))
            {
                throw new Exception(matcher.ErrorMsg);
            }
            this.constraints.Add(new Constraint { Root = matcher.root, Tree = repoConstraintModel, UnitHash = unitHash });
            return matcher.root.Name;
        }

        public void DeleteConstraint(string constraintModelName, int unitHash)
        {
            // var constraintModel = this.Repo.Model(constraintModelName);
            this.constraints.RemoveAll(x => x.UnitHash == unitHash);
        }

        public bool Check()
        {
            var result = true;
            foreach (var node in this.repoModel.Nodes) //foreach (var element in this.graph.DataGraph.Vertices)
            {
                if (constraints.Count() == 0)
                {
                    this.targetModel.SetElementAllowed(node, true);
                }
                else
                {
                    //var node = element.Node;
                    foreach (var constraint in this.constraints)
                    {
                        var isAllowed = true;
                        if (this.matcher.ElementsAreIdentic(constraint.Root, node) || constraint.Root.Class.Name == "AllNodes")
                        {
                            isAllowed = matcher.Check(node, constraint.Root, constraint.Tree);
                            this.targetModel.SetElementAllowed(node, isAllowed);
                        }
                        else if ((node.Class.Name == constraint.Root.Class.Name) && (constraint.Tree.Nodes.Count() == 1))
                        {
                            isAllowed = matcher.AttrCheck(node, constraint.Root);
                            this.targetModel.SetElementAllowed(node, isAllowed);
                        }
                        else
                        {
                            this.targetModel.SetElementAllowed(node, true);
                        }
                        result = result && isAllowed;
                    }
                }
            }
            return result;
        }
    }

    struct Constraint
    {
        public INode Root { get; set; }
        public IModel Tree { get; set; }
        public int UnitHash { get; set; }
    }
}
