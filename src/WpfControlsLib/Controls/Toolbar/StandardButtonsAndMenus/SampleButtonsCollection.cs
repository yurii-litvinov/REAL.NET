namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using Controller;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;
    using System.Collections.Generic;

    public class SampleButtonsCollection
    {
        public SampleButtonsCollection(IConsole console, Controller controller)
        {
            var undoAction = new Command(() => { controller.Undo(); });
            var redoAction = new Command(() => { controller.Redo(); });
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
