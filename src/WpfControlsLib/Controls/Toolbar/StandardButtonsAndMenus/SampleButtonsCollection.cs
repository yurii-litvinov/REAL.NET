namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using System;
    using System.Collections.Generic;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;

    public class SampleButtonsCollection
    {
        public SampleButtonsCollection(IConsole console)
        {
            Action action1 = () => { console.SendMessage("Test undo button"); };
            var command1 = new Command(action1);
            var image1 = "pack://application:,,,/" + "View/Pictures/Toolbar/undo.png";
            var button1 = new Button(command1, "Undo button", image1);
            Action action2 = () => { console.SendMessage("Test redo button"); };
            var command2 = new Command(action2);
            var image2 = "pack://application:,,,/" + "View/Pictures/Toolbar/redo.png";
            var button2 = new Button(command2, "Redo button", image2);
            this.SampleButtons.Add(button1);
            this.SampleButtons.Add(button2);
        }

        public IList<IButton> SampleButtons { get; private set; } = new List<IButton>();
    }
}
