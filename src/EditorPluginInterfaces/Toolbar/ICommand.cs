namespace EditorPluginInterfaces.Toolbar
{
    /// <summary>
    /// Pattern command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// True, if it can be undone
        /// </summary>
        bool IsUndoType { get; }

        /// <summary>
        /// Execute this command
        /// </summary>
        void Execute();

        /// <summary>
        /// Undo this command
        /// </summary>
        void Undo();
    }
}