namespace EditorPluginInterfaces
{
    using System;

    /// <summary>
    /// Arguments for new constraints event.
    /// </summary>
    public class ConstraintsEventArgs : EventArgs
    {
        /// <summary>
        /// //TODO
        /// </summary>
        public string ObjName { get; set; }

        /// <summary>
        /// //TODO
        /// </summary>
        public string AttributeName { get; set; }

    }
}