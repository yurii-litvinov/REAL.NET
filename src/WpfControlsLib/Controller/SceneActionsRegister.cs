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
                throw new InvalidOperationException("can't find this node on scene");
            }

            var toDelete = found.First();
            var position = toDelete.Value.GetPosition();
            Action doAction = () => { this.commands.DeleteVertexFromScene(node); };
            Action undoAction = () =>
            {
                this.commands.AddVertexOnScene(position, node);
                this.RestoreEdges(edgesFromNode);
            };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        public void RegisterAddingEdge(Repo.IEdge edge, Point[] points)
        {
            Action doAction = () => { this.commands.AddEdgeOnScene(edge, points); };
            Action undoAction = () => { this.commands.DeleteEdgeFromScene(edge); };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        public void RegisterDeletingEdge(Repo.IEdge edge)
        {
            var found = this.scene.EdgesList.ToList()
                .FindAll(x => x.Key.Edge == edge);
            if (found.Count == 0)
            {
                throw new InvalidOperationException("can't find this edge on scene");
            }

            var edgePair = found[0];
            var points = this.ConvertToWinPoints(edgePair.Key.RoutingPoints);
            Action doAction = () => { this.commands.DeleteEdgeFromScene(edge); };
            Action undoAction = () => { this.commands.AddEdgeOnScene(edge, points); };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        private void RestoreEdges(IEnumerable<Tuple<Repo.IEdge, Point[]>> pairs)
        {
            foreach (var pair in pairs)
            {
                this.commands.AddEdgeOnScene(pair.Item1, pair.Item2);
            }
        }

        private Point[] ConvertToWinPoints(GraphX.Measure.Point[] points) => points?.Select(r => new Point(r.X, r.Y)).ToArray();
    }
}
