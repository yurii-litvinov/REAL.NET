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
        private SceneCommands commands;
        private IUndoRedoStack undoRedoStack;

        public SceneActionsRegister(SceneCommands commands, IUndoRedoStack undoStack)
        {
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

        public void RegisterDeletingVertex(Repo.INode node)
        {
            // to do
        }
    }
}
