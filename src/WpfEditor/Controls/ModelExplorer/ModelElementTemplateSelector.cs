using System.Windows;
using System.Windows.Controls;

namespace WpfEditor.Controls.ModelExplorer
{
    internal class ModelElementTemplateSelector: DataTemplateSelector
    {
        public DataTemplate NodeTemplate { get; set; }
        public DataTemplate EdgeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) 
            => item is ModelExplorerNode ? this.NodeTemplate : this.EdgeTemplate;
    }
}
