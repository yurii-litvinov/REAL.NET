namespace WpfControlsLib.ViewModel
{
    using System;

    /// <summary>
    /// Arguments for attribute events (attribute change).
    /// </summary>
    public class AttributeEventArgs : EventArgs
    {
        /// <summary>
        /// //New attribute value
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// //Attribute name
        /// </summary>
        public string AttributeName { get; set; }
    }
}