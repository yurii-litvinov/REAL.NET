namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using System;
    using System.Collections.Generic;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;

    public class SampleButtonsCollection
    {
        private IConsole console;

        public SampleButtonsCollection(IConsole console)
        {
            Action action1 = () => { console.SendMessage("action binded to button1 executed"); };
            var command1 = new Command(action1);
            var button1 = new Button(command1, "sample button", string.Empty);
            Action action2 = () => { console.SendMessage("action binded to button2 executed"); };
            var command2 = new Command(action2);
            var button2 = new Button(command2, "sample button", string.Empty);
            this.SampleButtons.Add(button1);
            this.SampleButtons.Add(button2);
        }

        public IList<IButton> SampleButtons { get; private set; } = new List<IButton>();
    }
}
