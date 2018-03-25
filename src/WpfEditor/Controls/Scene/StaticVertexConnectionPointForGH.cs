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

namespace WpfEditor.Controls.Scene
{
    using GraphX.Controls;

    /// <summary>
    /// Сontrol for the vertex connection point. It can be of two types: from which the edges can exit (IsSource = true) 
    /// and into which they can enter.
    /// </summary>
    public class StaticVertexConnectionPointForGH : StaticVertexConnectionPoint
    {
        public StaticVertexConnectionPointForGH()
            : base()
        {
        }

        public bool IsSource
        { get; set; }

        public bool IsOccupied
        { get; set; }
    }
}
