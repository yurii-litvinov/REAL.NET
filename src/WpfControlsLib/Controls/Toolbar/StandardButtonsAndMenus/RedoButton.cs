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

namespace WpfControlsLib.Controls.Toolbar.StandardButtonsAndMenus
{
    using EditorPluginInterfaces.Toolbar;
    using EditorPluginInterfaces.UndoRedo;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for redo button.
    /// </summary>
    public class RedoButton : Button
    {
        public RedoButton(IUndoRedoStack undoRedoStack, string description, string image, bool isEnabled)
            : base(new Command(() => { undoRedoStack.Redo(); }), description, image, isEnabled);

        public RedoButton(IUndoRedoStack undoRedoStack, string description, string image)
            : this(undoRedoStack, description, image, true);
    }
}