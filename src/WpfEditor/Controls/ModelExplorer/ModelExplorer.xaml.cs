using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Repo;

namespace WpfEditor.Controls.ModelExplorer
{
    /// <summary>
    /// Shows all elements in currently opened model.
    /// </summary>
    public partial class ModelExplorer : UserControl
    {
        public ModelExplorer()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public void Clear() => this.Elements.Clear();

        public ObservableCollection<ModelExplorerElement> Elements { get; } 
            = new ObservableCollection<ModelExplorerElement>();

        public EventHandler<NodeSelectedEventArgs> NodeSelected;
        public EventHandler<EdgeSelectedEventArgs> EdgeSelected;

        private void ElementInBoxSelectedAction(object sender, EventArgs e)
        {
            if (!((this.elementsListBox.SelectedItem as ListBoxItem)?.Content is StackPanel sp))
            {
                return;
            }

            if (sp.Children.Count > 3)
            {
                // TODO: Quite ugly.
                var source = (sp.Children[2] as TextBlock)?.Text;
                var target = (sp.Children[4] as TextBlock)?.Text;
                this.EdgeSelected?.Invoke(this, new EdgeSelectedEventArgs {Source = source, Target = target});
            }
            else
            {
                var name = (sp.Children[2] as TextBlock)?.Text;
                this.NodeSelected?.Invoke(this, new NodeSelectedEventArgs {NodeName = name});
            }
        }

        public void NewElement(Repo.IElement element)
            => this.Elements.Add(element.Metatype == Metatype.Node
                ? (ModelExplorerElement) new ModelExplorerNode(element)
                : new ModelExplorerEdge(element));

        // TODO: Do it in XAML. Why have special DSL for initializing trees of objects and don't use it?
        // TODO: Model-View here.
        public void DrawNewVertex(string vertexName)
        {
            //var lbi = new ListBoxItem();
            //var sp = new StackPanel { Orientation = Orientation.Horizontal };

            //var img = new Image
            //{
            //    Source = new BitmapImage(new Uri("pack://application:,,,/View/Pictures/vertex.png"))
            //};
            //var spaces = new TextBlock { Text = "  " };
            //var tx = new TextBlock { Text = vertexName };

            //sp.Children.Add(img);
            //sp.Children.Add(spaces);
            //sp.Children.Add(tx);
            //lbi.Content = sp;
            //this.elementsListBox.Items.Add(lbi);
        }

        public void DrawNewEdge(string source, string target)
        {
            //var lbi = new ListBoxItem();
            //var sp = new StackPanel { Orientation = Orientation.Horizontal };

            //var img = new Image
            //{
            //    Source = new BitmapImage(new Uri("pack://application:,,,/View/Pictures/edge.png"))
            //};
            //var spaces = new TextBlock { Text = "  " };
            //var tx0 = new TextBlock { Text = source };
            //var tx1 = new TextBlock { Text = " - " };
            //var tx2 = new TextBlock { Text = target };

            //sp.Children.Add(img);
            //sp.Children.Add(spaces);
            //sp.Children.Add(tx0);
            //sp.Children.Add(tx1);
            //sp.Children.Add(tx2);
            //lbi.Content = sp;
            //this.elementsListBox.Items.Add(lbi);
        }

        public class NodeSelectedEventArgs : EventArgs
        {
            public string NodeName { get; set; }
        }

        public class EdgeSelectedEventArgs : EventArgs
        {
            public string Source { get; set; }
            public string Target { get; set; }
        }
    }
}
