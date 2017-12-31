/* Copyright 2017
 * Yurii Litvinov
 * Ivan Yarkov
 * Egor Zainullin
 * Denis Sushentsev
 * Arseniy Zavalishin
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System;
using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public abstract class MetamodelElement : IElement
    {
        protected MetamodelElement(IElement element)
        {
            this.Element = element;
        }

        public IElement Element { get; }

        public string Name
        {
            get => this.Element.Name;
            set => throw new NotImplementedException($"Cannot change name of {this.Metatype} from metamodel");
        }

        public IElement Class => this.Element.Class;

        public IEnumerable<IAttribute> Attributes => this.Element.Attributes;

        public bool IsAbstract => this.Element.IsAbstract;

        public Metatype Metatype => this.Element.Metatype;

        public Metatype InstanceMetatype => this.Element.InstanceMetatype;

        public string Shape => this.Element.Shape;

        public override string ToString()
        {
            return this.Name;
        }
    }
}