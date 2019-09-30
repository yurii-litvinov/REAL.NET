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

//namespace Repo.InfrastructureMetamodel.Semantics

//open Repo
//open Repo.DataLayer
//open Repo.InfrastructureMetamodel

///// Helper for working with elements in Infrastructure Metamodel terms.
//type ElementSemantics(metamodel: IDataModel) =
//    inherit LanguageMetamodel.Semantics.ElementSemantics(metamodel)

//    /// Returns underlying metamodel.
//    member this.Metamodel = metamodel

//    /// Returns string representation of an element.
//    member this.ToString (node: IDataElement) =
        
//        let slotValue slot =
//            CoreMetamodel.ElementSemantics.OutgoingAssociation slot "value"
//            |> fun a -> a.Target.Value :?> IDataNode

//        let name (element: IDataElement) = 
//            match element with
//            | :? IDataNode as n -> n.Name
//            | :? IDataAssociation as a -> a.TargetName
//            | _ -> "Generalization"

//        let result = sprintf "Name: %s\n" <| name node
//        let result = result + (sprintf "Ontological type: %s\n" <| node.OntologicalType.ToString ())
//        let result = result + (sprintf "Linguistic type: %s\n" <| node.LinguisticType.ToString())
//        let result = result + "Attributes:\n"
//        let attributes =
//            this.OwnAttributes node
//            |> Seq.map 
//                (fun attr -> 
//                    sprintf "    %s: %s\n" attr.Name (AttributeMetamodel.Semantics.AttributeSemantics.Type attr).Name)
//            |> fun s -> if Seq.isEmpty s then "" else Seq.reduce (+) s
//        let result = result + attributes

//        let result = result + "Slots:\n"
//        let slots =
//            this.Slots node
//            |> Seq.map (fun slot -> sprintf "    %s = %s\n" slot.Name (slotValue slot).Name)
//            |> fun s -> if Seq.isEmpty s then "" else Seq.reduce (+) s
//        result + slots