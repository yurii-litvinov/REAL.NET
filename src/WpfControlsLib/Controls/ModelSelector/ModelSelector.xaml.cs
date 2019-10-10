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
using System.ComponentModel;

namespace WpfControlsLib.Controls.ModelSelector
{
    /// <summary>
    /// Lists all models in a repository and provides means to select a model for editing.
    /// </summary>
    public partial class ModelSelector : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Names of all models in a repository that can be edited.
        /// </summary>
        public ObservableCollection<string> ModelNames { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Event that is raised when model selection is changed (by user or programmatically).
        /// </summary>
        public event EventHandler<ModelSelectedEventArgs> ModelSelected;

        private bool selectorVisibility = true;

        /// <summary>
        /// Creates a new instance of the <see cref="ModelSelector"/> class.
        /// </summary>
        public ModelSelector()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes model selector with given data model. Models are listed by model selector only of they are
        /// sufficiently low in metalevel hierarchy (i.e. are instances of InfrastructureMetamodel).
        /// </summary>
        /// <param name="dataModel">Model with repository containing models.</param>
        public void Init(WpfControlsLib.Model.Model dataModel)
        {
            bool IsBasedOnInfrastructureMetamodel(Repo.IModel model)
            {
                var metamodel = model.Metamodel;
                while (metamodel != metamodel.Metamodel)
                {
                    if (metamodel.Name == "InfrastructureMetamodel")
                    {
                        return true;
                    }

                    metamodel = metamodel.Metamodel;
                }

                return false;
            }

            this.ModelNames.Clear();

            foreach (var currentModel in dataModel.Repo.Models)
            {
                if (IsBasedOnInfrastructureMetamodel(currentModel) && currentModel.IsVisible)
                {
                    this.ModelNames.Add(currentModel.Name);
                }
            }

            this.modelsComboBox.SelectionChanged += this.OnModelSelectionChanged;
            this.modelsComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Shows if this element should be visible at the moment.
        /// </summary>
        public bool SelectorVisibility
        {
            get => selectorVisibility;
            set
            {
                selectorVisibility = value;
                OnPropertyChanged("SelectorVisibility");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeModel(int newModel)
            => this.modelsComboBox.SelectedIndex = newModel;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnModelSelectionChanged(object sender, SelectionChangedEventArgs args)
            => this.ModelSelected?.Invoke(this,
                new ModelSelectedEventArgs { ModelName = this.modelsComboBox.SelectedItem?.ToString() });

        /// <summary>
        /// Event arguments for <see cref="ModelSelector.ModelSelected"/> event. Contains name of a selected model.
        /// </summary>
        public class ModelSelectedEventArgs : EventArgs
        {
            public string ModelName { get; set; }
        }
    }
}
