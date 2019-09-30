//(* Copyright 2017 Yurii Litvinov
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License. *)

//namespace Repo.InfrastructureMetamodel

//open Repo.DataLayer
//open Repo.LanguageMetamodel

///// Initializes repository with Infrastructure Metametamodel, which defines a data scheme required from all other 
///// models to properly work with Infrastructure Semantics. Closely coupled with WPF editor.
//type InfrastructureMetametamodelCreator() =
//    interface IModelCreator with
//        member this.CreateIn(repo: IDataRepository): unit =
//            let metamodel = repo.Model "LanguageMetamodel"
//            let builder = LanguageSemanticsModelBuilder(repo, Consts.infrastructureMetametamodel, metamodel)

//            let (--->) (source: IDataElement) (target, name) = builder +---> (source, target, name)

//            builder.ReinstantiateLanguageMetamodel ()

//            let boolean = builder.AddEnum Consts.boolean [Consts.stringTrue; Consts.stringFalse]
//            let int = builder + "Int"
//            let double = builder + "Double"

//            let modelNode = builder + "Model"
//            let repoNode = builder + "Repo"

//            let metatype = builder.AddEnum "Metatype" [Consts.metatypeNode; Consts.metatypeEdge]

//            let element = builder.Node "Element"

//            repoNode ---> (modelNode, "models")
//            modelNode ---> (element, "elements")

//            builder.AddAttribute element "shape"
//            builder.AddAttributeWithType element boolean (builder.Node Consts.stringFalse) Consts.isAbstract
//            builder.AddAttributeWithType element metatype (builder.Node Consts.metatypeNode) Consts.instanceMetatype

//            let association = builder.Node "Association"
//            builder.AddAttribute association "name"

//            ()
