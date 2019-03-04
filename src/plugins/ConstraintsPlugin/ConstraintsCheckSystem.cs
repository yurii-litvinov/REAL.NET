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
        private List<Tuple<INode, IModel>> constraints;

        public Repo.IRepo Repo { get; private set; }
        private Model targetModel;
        private Matcher matcher;
        private Graph graph;
        public ConstraintsCheckSystem(Model targetModel, Graph graph)
        {
            this.constraints = new List<Tuple<INode, IModel>>();
            this.Repo = global::Repo.RepoFactory.Create();
            this.targetModel = targetModel;
            this.graph = graph;
            this.matcher = new Matcher(this.Repo.Model(targetModel.ModelName));
        }

        public string AddConstraint(string constraintModelName)
        {
            var constraintModel = this.Repo.Model(constraintModelName);
            if (!matcher.PreCheck(constraintModel))
            {
                throw new Exception(matcher.ErrorMsg);
            }
            this.constraints.Add(new Tuple<INode, IModel>(matcher.root, constraintModel));
            return matcher.root.Name;
        }

        public bool Check()
        {
            var result = true;
            foreach (var element in this.graph.DataGraph.Vertices)
            {
                var node = element.Node;
                foreach (var constraint in this.constraints)
                {
                    if (this.matcher.ElementsAreIdentic(constraint.Item1, node) || constraint.Item1.Class.Name == "AllNodes") 
                    {
                        var isAllowed = matcher.Check(node, constraint.Item1, constraint.Item2);
                        this.targetModel.SetElementAllowed(node, isAllowed);
                        result = result && isAllowed;
                    }
                }
            }
            return result;
        }
    }
}
