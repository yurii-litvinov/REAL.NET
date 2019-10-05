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

namespace WpfEditor.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using EditorPluginInterfaces;
    using PluginManager;
    using Repo;
    using WpfControlsLib.Controls.Console;
    using WpfControlsLib.Controls.ModelSelector;
    using WpfControlsLib.Controls.Scene;
    using WpfControlsLib.Controls.Toolbar;
    using WpfControlsLib.Controls.AttributesPanel;
    using Palette = WpfControlsLib.Controls.Palette.Palette;

    /// <summary>
    /// Main window of the application, launches on application startup.
    /// </summary>
    internal partial class MainWindow : INotifyPropertyChanged
    {
        private readonly WpfControlsLib.Model.Model model;
        private readonly WpfControlsLib.Controller.Controller controller;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<string> OnModelChanged;

        public AppConsoleViewModel Console { get; } = new AppConsoleViewModel();

        public ToolbarViewModel Toolbar { get; } = new ToolbarViewModel();

        public AttributesPanelViewModel AttributesPanel { get; } = new AttributesPanelViewModel();

        public string WindowTitle
        {
            get
            {
                var fileName = (model == null || model.CurrentFileName == "")
                    ? "(unsaved)"
                    : model.CurrentFileName;
                var unsavedChanges = model?.HasUnsavedChanges == true ? "*" : "";
                return $"REAL.NET {fileName} {unsavedChanges}";
            }
        }
        
        public MainWindow()
        {
            // TODO: Fix sequential coupling here.
            this.DataContext = this;
            this.InitializeComponent();

            this.model = new WpfControlsLib.Model.Model();

            this.model.Reinit += this.Reinit;

            this.model.FileSaved += (_, __) => NotifyTitleChanged();
            this.model.UnsavedChanges += (_, __) => NotifyTitleChanged();

            // Notifying window first time, to initialize title.
            NotifyTitleChanged();

            this.palette.SetModel(this.model);

            this.controller = new WpfControlsLib.Controller.Controller();

            this.Closed += this.CloseChildrenWindows;

            this.scene.ElementManipulationDone += (sender, args) => this.palette.ClearSelection();
            this.scene.ElementAdded += (sender, args) => this.modelExplorer.NewElement(args.Element);
            this.scene.ElementRemoved += (sender, args) => this.modelExplorer.RemoveElement(args.Element);
            this.scene.NodeSelected += (sender, args) => this.AttributesPanel.Attributes = args.Node.Attributes;
            this.scene.EdgeSelected += (sender, args) => this.AttributesPanel.Attributes = args.Edge.Attributes;

            this.scene.Init(this.model, this.controller, new PaletteAdapter(this.palette));

            this.InitAndLaunchPlugins();
            this.InitToolbar();

            this.modelSelector.Init(this.model);
            this.modelSelector.ChangeModel(2);
        }

        private void Reinit(object sender, EventArgs e)
        {
            this.SelectModel(this.modelSelector.ModelNames[2]);
        }

        private void OnModelSelectionChanged(object sender, ModelSelector.ModelSelectedEventArgs args)
        {
            SelectModel(args.ModelName);
        }

        private void SelectModel(string modelName)
        {
            this.scene.Clear();
            this.modelExplorer.Clear();
            this.model.ModelName = modelName;
            this.palette.InitPalette(this.model.ModelName);
            this.scene.Reload();
            this.OnModelChanged?.Invoke(this.model.ModelName);
        }

        private void InitToolbar()
        {
            this.Console.Messages.Add("Initializing ToolBar");
            var sample = new WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus.SampleButtonsCollection(this.Console, this.controller);
            var buttons = sample.SampleButtons;
            foreach (var button in buttons)
            {
                this.Toolbar.AddButton(button);
            }
        }

        private void InitAndLaunchPlugins()
        {
            var libs = new PluginLauncher<PluginConfig>();
            const string folder = "../../../plugins";
            var pluginDirs = new List<string>(System.IO.Directory.GetDirectories(folder));
            foreach (var plugindir in pluginDirs)
            {
                var dirs = new List<string>(System.IO.Directory.GetDirectories(plugindir + "/bin"));
                var config = new PluginConfig(this.model, null, null, this.Console, null, this.leftPanelGrid);
                config.ChangeModelSelectorVisibility = (x) => { this.modelSelector.SelectorVisibility = x; };
                config.ChangeModel = this.SelectModel;
                foreach (var dir in dirs)
                {
                    libs.LaunchPlugins(dir, config);
                }

                if (config.OnMainModelChanged != null)
                {
                    this.OnModelChanged += new Action<string>(config.OnMainModelChanged);
                }
            }
        }

        private void CloseChildrenWindows(object sender, EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                w.Close();
            }
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

        private void NotifyTitleChanged() => this.PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(this.WindowTitle))
            );

        private void OnCanExecuteForAlwaysExecutable(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnNewExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.model.New();
            this.controller.ClearHistory();
            NotifyTitleChanged();
        }

        private void OnOpenExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".rns",
                Filter = "Real.NET Saves (.rns)|*.rns"
            };

            var result = dialog.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                model.Open(dialog.FileName);
                this.controller.ClearHistory();
                NotifyTitleChanged();
            }
        }

        private void OnSaveExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (this.model.CurrentFileName == string.Empty)
            {
                this.SaveAs();
            }
            else
            {
                this.model.Save();
            }
        }

        private void OnSaveAsExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            SaveAs();
        }

        private void OnQuitExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveAs()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".rns",
                Filter = "Real.NET Saves|*.rns"
            };

            var result = dialog.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                model.SaveAs(dialog.FileName);
            }
        }
    }
}