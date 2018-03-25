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
    using System.Windows;
    using System.Windows.Controls;
    using GraphX.Controls;

    [TemplatePart(Name = "Source_vcproot", Type = typeof(Panel))]
    [TemplatePart(Name = "Target_vcproot", Type = typeof(Panel))]
    public class VertexControlForGH : VertexControl
    {
        public VertexControlForGH(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true)
            : base(vertexData, tracePositionChange, bindToDataObject)
        {
        }

        public Panel VCPSourceRoot { get; protected set; }

        public Panel VCPTargetRoot { get; protected set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.VCPSourceRoot = this.VCPSourceRoot ?? this.FindDescendant<Panel>("Source_vcproot");
            this.VCPTargetRoot = this.VCPTargetRoot ?? this.FindDescendant<Panel>("Target_vcproot");
        }
    }
}
