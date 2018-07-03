namespace WpfControlsLib.Controls.Scene.EventArguments
{
    using System;

    /// <summary>
    /// Arguments for added element event.
    /// </summary>
    public class ElementAddedEventArgs : EventArgs
    {
        public Repo.IElement Element { get; set; }
    }
}