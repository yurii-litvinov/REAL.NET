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

namespace WpfControlsLib.Controller
{
    using System;

    /// <summary>
    /// Arguments for undo/redo availability change events.
    /// </summary>
    public class UndoRedoAvailabilityChangedArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoAvailabilityChangedArgs"/> class.
        /// </summary>
        /// <param name="isAvailable">Availability of an operation.</param>
        public UndoRedoAvailabilityChangedArgs(bool isAvailable) => this.IsAvailable = isAvailable;

        /// <summary>
        /// Gets a value indicating whether operation is available.
        /// </summary>
        public bool IsAvailable { get; }
    }
}