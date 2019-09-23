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

using Repo;

namespace WpfControlsLib.ViewModel
{
    /// <summary>
    /// View model for an attribute to be displayed in property editor.
    /// </summary>
    public class AttributeViewModel
    {
        private string value;
        private readonly IAttribute attribute;

        public AttributeViewModel(IAttribute attribute, string name, string type)
        {
            this.attribute = attribute;
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; }

        public string Type { get; }

        public string Value
        {
            get => this.value;

            set
            {
                this.attribute.StringDefaultValue = value;
                this.value = value;
            }
        }
    }
}
