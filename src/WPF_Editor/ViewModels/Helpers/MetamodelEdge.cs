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
using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public class MetamodelEdge : MetamodelElement, IEdge
    {
        public MetamodelEdge(IEdge iedge) : base(iedge)
        {
            this._edge = iedge;
        }

        private readonly IEdge _edge;

        public IElement From
        {
            get => this._edge.From;
            set => throw new NotImplementedException("Cannot change source element of metamodel edge.");
        }

        public IElement To
        {
            get => this._edge.To;
            set => throw new NotImplementedException("Cannot change target element of metamodel edge.");
        }
    }
}