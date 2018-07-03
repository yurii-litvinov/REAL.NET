namespace WpfControlsLib.Controls.Scene.EventArguments
{
    using System;

    /// <summary>
    /// Arguments for removed element event.
    /// </summary>
    public class ElementRemovedEventArgs : EventArgs
    {
        public Repo.IElement Element { get; set; }
    }
}