/* Copyright 2018 REAL.NET group
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

namespace EditorPluginInterfaces
{
    using System;

    /// <summary>
    /// Arguments for new edge event.
    /// </summary>
    public class EdgeEventArgs : EventArgs
    {
        /// <summary>
        /// Metatype of an edge.
        /// TODO: Nobody cares about metatype, give us actual created edge!
        /// </summary>
        public Repo.IEdge Edge { get; set; }

        /// <summary>
        /// Source node for a created edge.
        /// </summary>
        public Repo.IElement Source { get; set; }

        /// <summary>
        /// Target node for a created edge.
        /// </summary>
        public Repo.IElement Target { get; set; }
    }
}