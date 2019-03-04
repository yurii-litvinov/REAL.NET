/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace AirSim.View
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using AirSimLib;
    using EditorPluginInterfaces;
    using PluginManager;
    using Repo;
    using WpfControlsLib.Controls.Console;
    using WpfControlsLib.Controls.ModelSelector;
    using WpfControlsLib.Controls.Scene;
    using Palette = WpfControlsLib.Controls.Palette.Palette;

    /// <summary>
    /// Main window of the application, launches on application startup.
    /// </summary>
    internal partial class MainWindow
    {
        private readonly WpfControlsLib.Model.Model model = new WpfControlsLib.Model.Model();
        private readonly WpfControlsLib.Controller.Controller controller = new WpfControlsLib.Controller.Controller();

        public AppConsoleViewModel Console { get; } = new AppConsoleViewModel();

        private CancellationToken cancellationToken;
        private CancellationTokenSource token;

        public MainWindow()
        {
            // TODO: Fix sequential coupling here.
            this.DataContext = this;
            this.InitializeComponent();

            this.palette.SetModel(this.model);

            this.Closed += this.CloseChildrenWindows;

            this.scene.ElementManipulationDone += (sender, args) => this.palette.ClearSelection();
            this.scene.ElementAdded += (sender, args) => this.modelExplorer.NewElement(args.Element);
            this.scene.NodeSelected += (sender, args) => this.attributesView.DataContext = args.Node;
            this.scene.EdgeSelected += (sender, args) => this.attributesView.DataContext = args.Edge;

            this.scene.Init(this.model, this.controller, new PaletteAdapter(this.palette));
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
            var config = new PluginConfig(this.model, null, null, this.Console, null, null);
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
            // var constraints = new ConstraintsWindow(this.repo, this.repo.Model(this.modelName));
            // constraints.ShowDialog();
        }

        private async void ExecuteButtonClick(object sender, RoutedEventArgs e)
        {
            this.stopButton.IsEnabled = true;
            this.executeButton.IsEnabled = false;
            this.token = new CancellationTokenSource();
            this.cancellationToken = this.token.Token;
            var codeExe = new CodeExecution();
            var t = this.model.Repo.Model("AirSimModel");
            void Action(string str) => this.Dispatcher.Invoke(() => this.Console.SendMessage(str));
            await Task.Factory.StartNew(() => codeExe.Execute(t, Action), this.cancellationToken);
            this.stopButton.IsEnabled = false;
            this.executeButton.IsEnabled = true;
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            this.token.Cancel();
            this.Console.SendMessage("Stop execution of code");
            this.stopButton.IsEnabled = false;
            this.executeButton.IsEnabled = true;
        }

        private void AttributesViewCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            => this.scene.ChangeEdgeLabel(((TextBox)e.EditingElement).Text);

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