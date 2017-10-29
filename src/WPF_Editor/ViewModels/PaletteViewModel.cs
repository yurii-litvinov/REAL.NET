/* Copyright 2017
 * Yurii Litvinov
 * Ivan Yarkov
 * Egor Zainullin
 * Denis Sushentsev
 * Arseniy Zavalishin
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System.Collections.ObjectModel;
using System.ComponentModel;
using Repo;
using WPF_Editor.ViewModels.Helpers;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class PaletteViewModel : IPaletteViewModel, INotifyPropertyChanged
    {
        private static IPaletteViewModel _palette;
        private readonly IPaletteMediatorViewModel _paletteMediator;
        private MetamodelElement _selectedElement;

        private PaletteViewModel(IPaletteMediatorViewModel paletteMediator)
        {
            Nodes = new ObservableCollection<MetamodelNode>();
            Edges = new ObservableCollection<MetamodelEdge>();
            Elements = new ObservableCollection<MetamodelElement>();
            _paletteMediator = paletteMediator;
            foreach (var inode in _paletteMediator.MetamodelNodes)
                if (!inode.IsAbstract)
                    Nodes.Add(new MetamodelNode(inode));
            foreach (var iedge in _paletteMediator.MetamodelEdges)
                if (!iedge.IsAbstract)
                    Edges.Add(new MetamodelEdge(iedge));
            foreach (var ielement in _paletteMediator.MetamodelElements)
                if (!ielement.IsAbstract)
                    if (ielement is INode)
                        Elements.Add(new MetamodelNode(ielement as INode));
                    else
                        Elements.Add(new MetamodelEdge(ielement as IEdge));
        }

        //Must be public, because bindings use these properties.
        public ObservableCollection<MetamodelNode> Nodes { get; }

        public ObservableCollection<MetamodelEdge> Edges { get; }

        public ObservableCollection<MetamodelElement> Elements { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MetamodelElement SelectedElement
        {
            get => _selectedElement;

            set
            {
                _selectedElement = value;
                OnPropertyChanged("SelectedElement");
            }
        }

        public static IPaletteViewModel CreatePalette(IPaletteMediatorViewModel paletteMediator = null)
        {
            if (_palette == null)
                _palette = new PaletteViewModel(paletteMediator);
            return _palette;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}