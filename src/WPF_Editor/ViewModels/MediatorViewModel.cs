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

using System.Collections.Generic;
using Repo;
using WPF_Editor.ViewModels.Helpers;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class MediatorViewModel : ISceneMediatorViewModel, IPaletteMediatorViewModel
    {
        private static MediatorViewModel _mediator;
        private readonly IModel _currentModel;
        private readonly IPaletteViewModel _palette;

        private readonly IRepo _repo = RepoFactory.CreateRepo();
        private ISceneViewModel _scene;


        private MediatorViewModel()
        {
            var currentMetamodel = this._repo.Model("RobotsMetamodel");
            this._currentModel = this._repo.CreateModel("New model based on RobotsMetamodel", currentMetamodel);
            this._scene = SceneViewModel.CreateScene(this);
            this._palette = PaletteViewModel.CreatePalette(this);
        }

        public IEnumerable<IElement> MetamodelElements => this._currentModel.Metamodel.Elements;
        public IEnumerable<INode> MetamodelNodes => this._currentModel.Metamodel.Nodes;
        public IEnumerable<IEdge> MetamodelEdges => this._currentModel.Metamodel.Edges;

        public MetamodelElement GetSelectedMetamodelType()
        {
            return this._palette.SelectedElement;
        }

        public ModelNode GetModelNode(MetamodelNode metaNode)
        {
            return new ModelNode(this._currentModel.CreateElement(metaNode.Element) as INode);
        }

        public ModelEdge GetModelEdge(MetamodelEdge metaEdge, ModelNode source, ModelNode target)
        {
            return new ModelEdge(this._currentModel.CreateElement(metaEdge.Element) as IEdge, source, target);
        }


        public static MediatorViewModel CreateMediator()
        {
            if (_mediator is null)
                _mediator = new MediatorViewModel();
            return _mediator;
        }
    }
}