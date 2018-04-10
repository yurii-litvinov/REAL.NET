namespace EditorPluginInterfaces
{
    using System;

    /// <summary>
    /// Arguments for new constraints event.
    /// </summary>
    public class AttributesEventArgs : EventArgs
    {
        /// <summary>
        /// //TODO
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// //TODO
        /// </summary>
        public object AttributeValue { get; set; }

        /// <summary>
        /// //TODO
        /// </summary>
        public string AttributeKind { get; set; }

    }
}