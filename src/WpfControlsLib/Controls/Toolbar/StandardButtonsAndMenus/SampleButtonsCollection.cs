namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using System;
    using System.Collections.Generic;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;
    using EditorPluginInterfaces.UndoRedo;

    public class SampleButtonsCollection
    {
        public SampleButtonsCollection(IConsole console, IUndoRedoStack undoRedoStack)
        {
            var undoAction = new Command(() => { undoRedoStack.Undo(); });
            var redoAction = new Command(() => { undoRedoStack.Redo(); });
            var undoImage = "pack://application:,,,/" + "View/Pictures/Toolbar/undo.png";
            var redoImage = "pack://application:,,,/" + "View/Pictures/Toolbar/redo.png";
            var undoButton = new Button(undoAction, "Undo button", undoImage);
            var redoButton = new Button(redoAction, "Redo Button", redoImage);

            this.SampleButtons.Add(undoButton);
            this.SampleButtons.Add(redoButton);
        }

        public IList<IButton> SampleButtons { get; private set; } = new List<IButton>();
    }
}
