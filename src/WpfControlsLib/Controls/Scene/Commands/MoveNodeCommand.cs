using EditorPluginInterfaces;
using Repo;

namespace WpfControlsLib.Controls.Scene.Commands
{
    internal class MoveNodeCommand : ICommand
    {
        public MoveNodeCommand(ISceneModel model, INode node, VisualPoint newPosition)
        {
            this.model = model;
            this.node = node;
            this.positionBeforeMoving = node.VisualInfo.Position;
            this.positionAfterMoving = newPosition;
        }

        private readonly ISceneModel model;
        
        private readonly INode node;

        private readonly VisualPoint positionBeforeMoving;
        
        private readonly VisualPoint positionAfterMoving;
        
        public bool CanBeUndone => true;
        
        public void Execute()
        {
            var visual = node.VisualInfo.Copy();
            visual.Position = positionAfterMoving;
            model.UpdateNodeVisual(node, visual);
        }

        public void Undo()
        {
            var visual = node.VisualInfo.Copy();
            visual.Position = positionBeforeMoving;
            model.UpdateNodeVisual(node, visual);
        }
    }
}