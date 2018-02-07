using System;
using System.Collections.Generic;
using System.Windows;
using EditorPluginInterfaces;
using PluginManager;
using Repo;
using WpfEditor.Controls.Console;
using WpfEditor.Controls.ModelSelector;
using WpfEditor.Controls.Palette;
using WpfEditor.Controls.Scene;

namespace WpfEditor.View
{
    /// <summary>
    /// Main window of the application, launches on application startup.
    /// </summary>
    internal partial class MainWindow : Window
    {
        private readonly Model.Model model;
        public AppConsoleViewModel Console { get; } = new AppConsoleViewModel();

        public MainWindow()
        {
            // TODO: Fix sequential coupling here.
            this.DataContext = this;
            this.InitializeComponent();

            this.model = new Model.Model();

            this.palette.SetModel(this.model);

            var controller = new Controller.Controller(this.model);

            this.Closed += this.CloseChildrenWindows;

            this.scene.ElementUsed += (sender, args) => this.palette.ClearSelection();
            this.scene.ElementAdded += (sender, args) => this.modelExplorer.NewElement(args.Element);
            this.scene.NodeSelected += (sender, args) => this.attributesView.DataContext = args.Node;

            this.scene.Init(this.model, controller, new PaletteAdapter(this.palette));
            this.modelSelector.Init(this.model);

            this.InitAndLaunchPlugins();
        }

        private void OnModelSelectionChanged(object sender, ModelSelector.ModelSelectedEventArgs args)
        {
            this.scene.Clear();
            this.modelExplorer.Clear();
            this.model.ModelName = args.ModelName;
            this.palette.InitPalette(this.model.ModelName);
            this.scene.Reload();
        }

        private void InitAndLaunchPlugins()
        {
            var libs = new PluginLauncher<PluginConfig>();
            const string folder = "../../../plugins/SamplePlugin/bin";
            var dirs = new List<string>(System.IO.Directory.GetDirectories(folder));
            var config = new PluginConfig(null, null, this.Console, null);
            foreach (var dir in dirs)
            {
                libs.LaunchPlugins(dir, config);
            }
        }

        private void CloseChildrenWindows(object sender, EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                w.Close();
            }
        }

        private void ConstraintsButtonClick(object sender, RoutedEventArgs e)
        {
            //var constraints = new ConstraintsWindow(this.repo, this.repo.Model(this.modelName));
            //constraints.ShowDialog();
        }

        private class PaletteAdapter : IElementProvider
        {
            private readonly Palette palette;

            internal PaletteAdapter(Palette palette)
            {
                this.palette = palette;
            }

            public IElement Element => this.palette.SelectedElement;
        }
    }
}
