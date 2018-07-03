namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;
    using Repo;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SaveAndLoadButtons
    {
        private IConsole console;

        private Repo.IModel model;

        public SaveAndLoadButtons(IConsole console, Repo.IModel model)
        {
            this.console = console;
            this.model = model;
        }

        public IList<IButton> Buttons { get; } = new List<IButton>();

        private void InitButtons()
        {
            void save()
            {
                this.console.SendMessage("action binded to button1 executed");
            }
            var saveCommand = new Command(save);
            var button1 = new Toolbar.Button(saveCommand, "save button", "toSave");
            void load()
            {
                this.console.SendMessage("action binded to button2 executed");
            }
            var loadCommand = new Command(load);
            var button2 = new Toolbar.Button(loadCommand, "load button", "toLoad");
            this.Buttons.Add(button1);
            this.Buttons.Add(button2);
        }

        private string GetPathToLoad()
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.MessageBox.Show(fbd.SelectedPath);
                return fbd.SelectedPath;
            }

            return null;
        }
    }
}
