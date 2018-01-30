using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
            this.elementsListBox.MouseDoubleClick += this.ElementInBoxSelectedAction;

            this.Closed += this.CloseChildrenWindows;

            this.scene.ElementUsed += (sender, args) => this.palette.ClearSelection();
            this.scene.DrawNewEdge += (sender, args) => this.DrawNewEdge(args.Source, args.Target);
            this.scene.DrawNewVertex += (sender, args) => this.DrawNewVertex(args.VertName);
            this.scene.NodeSelected += (sender, args) => this.attributesView.DataContext = args.Node;

            this.scene.Init(this.model, controller, new PaletteAdapter(this.palette));
            this.modelSelector.Init(this.model);

            this.InitAndLaunchPlugins();
        }

        private void OnModelSelectionChanged(object sender, ModelSelector.ModelSelectedEventArgs args)
        {
            this.scene.Clear();
            this.elementsListBox.Items.Clear();
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

        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            var sp = (this.elementsListBox.SelectedItem as ListBoxItem)?.Content as StackPanel;
            if (sp == null)
            {
                return;
            }

            if (sp.Children.Count > 3)
            {
                // TODO: Quite ugly.
                var source = (sp.Children[2] as TextBlock)?.Text;
                var target = (sp.Children[4] as TextBlock)?.Text;
                this.scene.SelectEdge(source, target);
            }
            else
            {
                var name = (sp.Children[2] as TextBlock)?.Text;
                this.scene.SelectNode(name);
            }
        }

        private void CloseChildrenWindows(object sender, EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                w.Close();
            }
        }

        // TODO: Do it in XAML. Why have special DSL for initializing trees of objects and don't use it?
        private void DrawNewVertex(string vertexName)
        {
            var lbi = new ListBoxItem();
            var sp = new StackPanel { Orientation = Orientation.Horizontal };

            var img = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/View/Pictures/vertex.png"))
            };
            var spaces = new TextBlock { Text = "  " };
            var tx = new TextBlock { Text = vertexName };

            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
        }

        private void DrawNewEdge(string source, string target)
        {
            var lbi = new ListBoxItem();
            var sp = new StackPanel { Orientation = Orientation.Horizontal };

            var img = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/View/Pictures/edge.png"))
            };
            var spaces = new TextBlock { Text = "  " };
            var tx0 = new TextBlock { Text = source };
            var tx1 = new TextBlock { Text = " - " };
            var tx2 = new TextBlock { Text = target };

            sp.Children.Add(img);
            sp.Children.Add(spaces);
            sp.Children.Add(tx0);
            sp.Children.Add(tx1);
            sp.Children.Add(tx2);
            lbi.Content = sp;
            this.elementsListBox.Items.Add(lbi);
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
