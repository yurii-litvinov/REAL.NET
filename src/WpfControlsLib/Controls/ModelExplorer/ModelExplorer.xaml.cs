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

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpfControlsLib.Controls.ModelExplorer
{
    /// <summary>
    /// Shows all elements in currently opened model as a list.
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
            => this.Elements.Add(element.Metatype == Repo.Metatype.Node
                ? (ModelExplorerElement) new ModelExplorerNode(element)
                : new ModelExplorerEdge(element));

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
