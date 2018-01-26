namespace EditorPluginInterfaces.Toolbar
{
    /// <summary>
    /// Button on toolbar
    /// </summary>
    public interface IButton
    {
        /// <summary>
        /// Gets action executed by this command
        /// </summary>
        /// <returns>Command's action</returns>
        ICommand GetAction();

        /// <summary>
        /// Does action connected with this button
        /// </summary>
        void DoAction();

        /// <summary>
        /// Gets description related to this button
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets image to show on this button
        /// </summary>
        string Image { get; }
    }
}