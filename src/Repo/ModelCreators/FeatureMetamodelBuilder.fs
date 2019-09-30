//(* Copyright 2019 Yurii Litvinov
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*     http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License. *)

//namespace Repo.Metamodels

//open Repo
//open Repo.DataLayer
//open Repo.InfrastructureMetamodel

//(*
///// Initializes repository with Robots Metamodel, first testing metamodel of a real language.
//type FeatureMetamodelBuilder() =
//   interface IModelBuilder with
//       member this.Build(repo: IDataRepository): unit =
//           let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
//           let metamodel = infrastructure.Metamodel.Model

//           let find name = metamodel.Node name
//           let findAssociation node name = CoreMetamodel.ModelSemantics.FindAssociationWithSource node name

//           let metamodelElement = find "Element"
//           let metamodelNode = find "Node"
//           let metamodelGeneralization = find "Generalization"
//           let metamodelAssociation = find "Association"
//           let metamodelAttribute = find "Attribute"

//           let model = repo.CreateModel("FeatureMetamodel", metamodel)

//           let (~+) (name, shape, isAbstract) =
//               let node = infrastructure.Instantiate model metamodelNode :?> IDataNode
//               node.Name <- name
//               infrastructure.Element.SetAttributeValue node "shape" shape
//               infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
//               infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

//               node

//           let (--|>) (source: IDataElement) target =
//               model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

//           let (--->) (source: IDataElement) (target, targetName, linkName) =
//               let edge = infrastructure.Instantiate model metamodelAssociation :?> IDataAssociation
//               edge.Source <- Some source
//               edge.Target <- Some target
//               edge.TargetName <- targetName

//               infrastructure.Element.SetAttributeValue edge "shape" "View/Pictures/edge.png"
//               infrastructure.Element.SetAttributeValue edge "isAbstract" "false"
//               infrastructure.Element.SetAttributeValue edge "instanceMetatype" "Metatype.Edge"
//               infrastructure.Element.SetAttributeValue edge "name" linkName

//               edge

//           let feature = +("Feature", "", true)
//           let abstractFeature = +("AbstractFeature", "", false)
//           let concreteFeature = +("ConcreteFeature", "", false)

//           let link = feature ---> (feature, "target", "Link")

//           //initialNode --|> abstractNode
//           abstractFeature --|> feature
//           concreteFeature --|> feature

//           ()
//*)