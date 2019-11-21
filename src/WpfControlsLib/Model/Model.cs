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

namespace WpfControlsLib.Model
{
    using System;
    using EditorPluginInterfaces;

    /// <summary>
    /// Model in MVC architecture. Wraps repository, provides operations like adding or removing models, edges and
    /// nodes (note that "model" is used with two different meanings around the project -- a visual model consisting
    /// of nodes and edges and a model in MVC sense) and notifications for all concerned tools about changes in repo.
    /// This class is a ground truth about visual model currently edited and is supposed to be used by all tools and
    /// parts of an editor who need to listen for visual model changes and/or modify visual model.
    /// </summary>
    public class Model : IModel
    {
        private bool hasUnsavedChanges = false;

        public Model()
        {
            this.Repo = global::Repo.RepoFactory.Create();
        }

        public event EventHandler<VertexEventArgs> NewVertexAdded;

        public event EventHandler<EdgeEventArgs> NewEdgeAdded;

        public event EventHandler<ElementEventArgs> ElementRemoved;

        public event EventHandler<ElementEventArgs> ElementCheck;

        public event EventHandler<EventArgs> FileSaved;

        public event EventHandler<EventArgs> UnsavedChanges;

        /// <summary>
        /// Notifies all views that there are changes so massive that the model shall be completely reloaded 
        /// (for example, when creating new or opening existing file).
        /// </summary>
        public event EventHandler<EventArgs> Reinit;

        public string ModelName { get; set; }

        public string ErrorMessage { get; set; }

        public Repo.IRepo Repo { get; private set; }

        /// <summary>
        /// Name of a file with which we are working now. Empty if we just created a new model and did not saved it 
        /// yet.
        /// </summary>
        public string CurrentFileName { get; private set; } = string.Empty;

        /// <summary>
        /// Flag that is true if current file contains unsaved changes.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get => hasUnsavedChanges;

            private set {
                if (hasUnsavedChanges && !value)
                {
                    hasUnsavedChanges = value;
                    FileSaved?.Invoke(this, EventArgs.Empty);
                }
                else if (!hasUnsavedChanges && value)
                {
                    hasUnsavedChanges = value;
                    UnsavedChanges?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Clears the contents of current repository and creates new empty one. Things like "Do you want to save 
        /// changes?" dialog or even new model selection are not supported yet.
        /// </summary>
        public void New()
        {
            this.Repo = global::Repo.RepoFactory.Create();
            this.CurrentFileName = "";
            this.HasUnsavedChanges = false;
            this.Reinit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Opens a given save file and reinitializes a repository with it.
        /// </summary>
        /// <param name="fileName">Name of the save to open.</param>
        public void Open(string fileName)
        {
            this.Repo = global::Repo.RepoFactory.Load(fileName);
            this.CurrentFileName = fileName;
            this.HasUnsavedChanges = false;
            this.Reinit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Saves current repository into current file. Current file name must be set before this operation is called.
        /// </summary>
        public void Save()
        {
            if (this.CurrentFileName == string.Empty)
            {
                throw new InvalidOperationException("Current file name is not set in the model, saving not possible");
            }

            this.Repo.Save(CurrentFileName);
            this.HasUnsavedChanges = false;
        }

        /// <summary>
        /// Saves current repository into a given file, changes current file.
        /// </summary>
        /// <param name="fileName">File to save repository to (with path, absolute or relative to a working directory).
        /// </param>
        public void SaveAs(string fileName)
        {
            this.CurrentFileName = fileName;
            this.Save();
        }

        public void CreateNode(Repo.IElement element)
        {
            if (string.IsNullOrEmpty(this.ModelName))
            {
                throw new InvalidOperationException("Current model name is not set");
            }

            var _ = element ?? throw new ArgumentNullException(nameof(element));

            var model = this.Repo.Model(this.ModelName);

                var newNode = model.CreateElement(element) as Repo.INode;
                HasUnsavedChanges = true;
                this.RaiseNewVertex(newNode);
        }

        public void CreateEdge(Repo.IEdge edge, Repo.IElement source, Repo.IElement destination)
        {
                var model = this.Repo.Model(this.ModelName);
                var newEdge = model.CreateElement(edge as Repo.IElement) as Repo.IEdge;
                newEdge.Name = "a" + edge.Name;
                newEdge.From = source;
                newEdge.To = destination;
                HasUnsavedChanges = true;
                this.RaiseNewEdge(newEdge, newEdge.From, newEdge.To);
        }

        public void RemoveElement(Repo.IElement element)
        {
            var model = this.Repo.Model(this.ModelName);
            model.DeleteElement(element);
            HasUnsavedChanges = true;
            this.RaiseElementRemoved(element);
        }

        public void SetElementAllowed(Repo.IElement element, bool isAllowed)
            => this.RaiseElementCheck(element, isAllowed);

        private void RaiseNewVertex(Repo.INode node)
        {
            var args = new VertexEventArgs
            {
                Node = node
            };

            this.NewVertexAdded?.Invoke(this, args);
        }

        private void RaiseNewEdge(Repo.IEdge edge, Repo.IElement prevVer, Repo.IElement ctrlVer)
        {
            var args = new EdgeEventArgs
            {
                Edge = edge,
                Source = prevVer,
                Target = ctrlVer
            };

            this.NewEdgeAdded?.Invoke(this, args);
        }

        private void RaiseElementRemoved(Repo.IElement element)
        {
            var args = new ElementEventArgs
            {
                Element = element
            };

            this.ElementRemoved?.Invoke(this, args);
        }

        private void RaiseElementCheck(Repo.IElement element, bool isAllowed)
        {
            var args = new ElementEventArgs
            {
                Element = element,
                IsAllowed = isAllowed
            };


            this.ElementCheck?.Invoke(this, args);
        }
    }
}
