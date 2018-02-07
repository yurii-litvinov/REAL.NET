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

namespace WpfEditor.Controls.ModelExplorer
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Template selector that selects data template for model explorer element list based on a type of an element
    /// --- node or edge.
    /// </summary>
    internal class ModelElementTemplateSelector: DataTemplateSelector
    {
        public DataTemplate NodeTemplate { get; set; }
        public DataTemplate EdgeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) 
            => item is ModelExplorerNode ? this.NodeTemplate : this.EdgeTemplate;
    }
}
