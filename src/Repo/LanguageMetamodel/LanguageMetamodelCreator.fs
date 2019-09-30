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

//namespace Repo.LanguageMetamodel

//open Repo.DataLayer
//open Repo.AttributeMetamodel

///// Initializes repository with Language Metamodel, which is used as a language to define Infrastructure Metamodel,
///// which in turn is used to define all other metamodels and closely coupled with editor capabilities.
//type LanguageMetamodelCreator() =
//    interface IModelCreator with
//        member this.CreateIn(repo: IDataRepository): unit =

//            let builder = AttributeSemanticsModelBuilder(repo, Consts.languageMetamodel)
//            let (--->) (source: IDataElement) (target, name) = builder +---> (source, target, name)
//            let (--|>) (source: IDataNode) (target: IDataNode) = builder +--|> (source, target)

//            builder.ReinstantiateAttributeMetamodel ()

//            let enum = builder + "Enum"
//            enum --|> builder.Node "Element"
//            enum ---> (builder.Node "String", "elements")

//            ()
