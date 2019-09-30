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

//module Repo.AttributeMetamodel.AttributeSemanticsHelpers

//open Repo.DataLayer
//open Repo.AttributeMetamodel.Semantics

///// Checks if generalization relation is possible between two given elements. Generalization is impossible if child
///// or its descendants reintroduce attributes that are already present within parent generalization hierarchy, but
///// with different types.
///// TODO: Check that child descendants do not have bad attributes.
//let isGeneralizationPossible (elementSemantics: ElementSemantics) (child: IDataElement) (parent: IDataElement) =
//    let getNameTypePairs (attributes: IDataNode seq) =
//        attributes
//        |> Seq.map (fun a -> (a.Name, AttributeSemantics.Type a))
    
//    let parentAttributes = elementSemantics.Attributes parent |> getNameTypePairs
//    let childAttributes = elementSemantics.Attributes child |> getNameTypePairs

//    childAttributes 
//    |> Seq.exists (
//        fun (name, ``type``) -> 
//            parentAttributes 
//            |> Seq.exists (
//                   fun (parentAttributeName, parentAttributeType) -> 
//                       name = parentAttributeName && ``type`` <> parentAttributeType
//               )
//        )
//    |> not

///// Checks that adding given attribute to a given element does not break generalization hierarchy.
///// TODO: Implement.
//let isAttributeAddingPossible 
//        (elementSemantics: ElementSemantics)
//        (element: IDataElement)
//        (attributeName: string)
//        (attributeType: IDataElement) =
//    true