namespace WpfControlsLib.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using EditorPluginInterfaces.UndoRedo;
    using WpfControlsLib.Controls.Scene;
    using WpfControlsLib.Controls.Toolbar;

    public class SceneActionsRegister
    {
        private GraphArea scene;
        private SceneCommands commands;
        private IUndoRedoStack undoRedoStack;

        // to do : keep 'deleted' elements here
        private List<Repo.INode> deletedNodes = new List<Repo.INode>();

        public SceneActionsRegister(GraphArea scene, SceneCommands commands, IUndoRedoStack undoStack)
        {
            this.scene = scene;
            this.commands = commands;
            this.undoRedoStack = undoStack;
        }

        public void RegisterAddingVertex(Point position, Repo.INode node)
        {
            Action doAction = () => { this.commands.AddVertexOnScene(position, node); };
            Action undoAction = () => { this.commands.DeleteVertexFromScene(node); };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        public void RegisterDeletingVertex(Repo.INode node, IEnumerable<Tuple<Repo.IEdge, Point[]>> edgesFromNode)
        {
            var found = this.scene.VertexList.ToList()
                .Where(x => x.Key.Node == node);
            if (found.Count() == 0)
            {
                throw new InvalidOperationException("can't find this node");
            }

            this.deletedNodes.Add(node);
            var toDelete = found.First();
            var position = toDelete.Value.GetPosition();
            Action doAction = () => { this.commands.DeleteVertexFromScene(node); };
            Action undoAction = () =>
            {
                this.commands.AddVertexOnScene(position, node);
                //this.RestoreEdges(edgesFromNode);
            };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        private IList<Repo.IEdge> GetAllEdgesFromVertex(Repo.INode node)
        {
            var found = this.scene.EdgesList.ToList()
                .FindAll(x => x.Key.Edge.From == node || x.Key.Edge.To == node)
                .Select(x => x.Key.Edge).ToList();
            return found;
        }

        private void RestoreEdges(IEnumerable<Tuple<Repo.IEdge, Point[]>> pairs)
        {
            foreach (var pair in pairs)
            {
                this.commands.AddEdgeOnScene(pair.Item1, pair.Item2);
            }
        }
    }
}
