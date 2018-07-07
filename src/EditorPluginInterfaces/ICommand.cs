namespace EditorPluginInterfaces
{
    /// <summary>
    /// Command interface, like in "Command" pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// True, if it can be undone
        /// </summary>
        bool CanBeUndone { get; }

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