//(* Copyright 2017-2019 Yurii Litvinov
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

//namespace Repo.CoreMetamodel.Details

//open Repo.DataLayer

///// Initializes repository with Core Metamodel.
//type CoreMetamodelCreator() =
//    interface IModelCreator with
//        member this.CreateIn(repo: IDataRepository): unit =
//            let model = repo.CreateModel "CoreMetamodel"

//            let node = model.CreateNode "Node"

//            let (~+) name = model.CreateNode name

//            let element = +"Element"
//            let edge = +"Edge"
//            let generalization = +"Generalization"
//            let association = +"Association"
//            let stringNode = +"String"

//            let (--//-->) (source: IDataElement) target =
//                model.CreateAssociation(association, source, target, "instanceOf") |> ignore

//            element --//--> node
//            node --//--> node

//            let (--|>) (source: IDataElement) target =
//                model.CreateGeneralization(generalization, source, target) |> ignore

//            node --|> element
//            edge --|> element
//            generalization --|> edge
//            association --|> edge

//            let (--->) (source: IDataElement) (target, name) =
//                model.CreateAssociation(association, source, target, name) |> ignore

//            /// Ontological type of an element --- type from problem domain. 
//            /// E.g. "Species" is an ontological type of "Dog".
//            element ---> (element, "ontologicalType")

//            /// Linguistic type of an element --- type from language and tooling point of view.
//            /// E.g. "Species" and "Dog" are both "Object" and will be drawn as a rectangle on a diagram.
//            element ---> (element, "linguisticType")

//            edge ---> (element, "source")
//            edge ---> (element, "target")
//            association ---> (stringNode, "targetName")

//            ()
