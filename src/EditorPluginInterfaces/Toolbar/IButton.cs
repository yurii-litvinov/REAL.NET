using System;

namespace EditorPluginInterfaces.Toolbar
{
    /// <summary>
    /// Button on toolbar
    /// </summary>
    public interface IButton
    {
        /// <summary>
        /// Raised when button's visibility changed
        /// </summary>
        event EventHandler ButtonEnabledChanged;

        /// <summary>
        /// Gets action executed by this command
        /// </summary>
        /// <returns>Command's action</returns>
        ICommand Command { get; }

        /// <summary>
        /// Does action connected with this button
        /// </summary>
        void DoAction();

        /// <summary>
        /// Sets button enabled
        /// </summary>
        void SetEnabled();

        /// <summary>
        /// Sets button disabled
        /// </summary>
        void SetDisabled();

        /// <summary>
        /// Gets description related to this button
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets image to show on this button
        /// </summary>
        string Image { get; }

        /// <summary>
        /// Gets is this button enabled
        /// </summary>
        bool IsEnabled { get; }
    }
}