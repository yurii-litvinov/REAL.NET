namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using Controller;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;
    using System.Collections.Generic;

    public class SampleButtonsCollection
    {
        private IButton undoButton;
        private IButton redoButton;
        
        public SampleButtonsCollection(IConsole console, Controller controller)
        {
            var undoAction = new Command(() => { controller.Undo(); });
            var redoAction = new Command(() => { controller.Redo(); });
            var undoImage = "pack://application:,,,/" + "View/Pictures/Toolbar/undo.png";
            var redoImage = "pack://application:,,,/" + "View/Pictures/Toolbar/redo.png";
            undoButton = new Button(undoAction, "Undo button", undoImage, false);
            redoButton = new Button(redoAction, "Redo Button", redoImage, false);
            controller.UndoAvailabilityChanged += OnUndoAvailabilityChanged;
            controller.RedoAvailabilityChanged += OnRedoAvailabilityChanged;
            this.SampleButtons.Add(undoButton);
            this.SampleButtons.Add(redoButton);
        }

        private void OnRedoAvailabilityChanged(object sender, UndoRedoAvailabilityChangedArgs e)
        {
            if (e.IsAvailable)
            {
                redoButton.SetEnabled();
            }
            else
            {
                redoButton.SetDisabled();
            }
        }

        private void OnUndoAvailabilityChanged(object sender, UndoRedoAvailabilityChangedArgs e)
        {
            if (e.IsAvailable)
            {
                undoButton.SetEnabled();
            }
            else
            {
                undoButton.SetDisabled();
            }
        }

        public IList<IButton> SampleButtons { get; private set; } = new List<IButton>();
    }
}
