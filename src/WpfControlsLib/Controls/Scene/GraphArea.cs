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

using GraphX.Controls;
using QuickGraph;
using WpfControlsLib.ViewModel;

namespace WpfControlsLib.Controls.Scene
{
    /// <summary>
    /// Visual representation of a GraphX graph, supposed to be used with GraphX zoom control as a scene to draw
    /// a diagram on.
    /// </summary>
    public class GraphArea : GraphArea<NodeViewModel, EdgeViewModel, BidirectionalGraph<NodeViewModel, EdgeViewModel>>
    {
    }
}
