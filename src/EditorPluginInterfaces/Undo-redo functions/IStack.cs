using EditorPluginInterfaces.Toolbar;

namespace EditorPluginInterfaces.Undo_redo_functions
{
    /// <summary>
    /// Interface for stack of undo/redo commands.
    /// </summary>
    public interface IStack
    {
        /// <summary>
        /// Gets a value indicating whether undo stack is available.
        /// </summary>
        bool IsUndoAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether redo stack is available.
        /// </summary>
        bool IsRedoAvailable { get; }

        /// <summary>
        /// Handling a command.
        /// </summary>
        /// <param name="command">Command for handling.</param>
        void HandleCommand(ICommand command);

        /// <summary>
        /// Undo a command.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo a command.
        /// </summary>
        void Redo();
    }
}