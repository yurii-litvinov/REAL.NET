﻿/* Copyright 2018 REAL.NET group
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

namespace WpfControlsLib.Controls.Scene.Commands
{
    using EditorPluginInterfaces;
    using System;

    /// <summary>
    /// Removes an edge.
    /// </summary>
    internal class RemoveEdgeCommand : ICommand
    {
        private ISceneModel model;
        private Repo.IElement edge;

        /// <summary>
        /// Initializes a new instance of <see cref="RemoveEdgeCommand"/> class.
        /// </summary>
        /// <param name="model">Model from which an edge shall be removed.</param>
        /// <param name="edge">Edge that shall be removed.</param>
        public RemoveEdgeCommand(ISceneModel model, Repo.IElement edge)
        {
            this.model = model;
            this.edge = edge;
        }

        bool ICommand.CanBeUndone => true;

        void ICommand.Execute() => this.model.RemoveElement(this.edge);

        void ICommand.Undo() => this.model.RestoreElement(this.edge);
    }
}
