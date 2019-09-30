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

//namespace Repo.AttributeMetamodel.Semantics

//open Repo
//open Repo.DataLayer

///// Helper class that provides semantic operations on models conforming to Attribute Metamodel.
//type InstantiationSemantics(metamodel: IDataModel) =
//    let elementHelper = ElementSemantics(metamodel)

//    let node = metamodel.Node "Node"
//    let association = metamodel.Node "Association"

//    /// Adds a new slot which is an instance of attribute with a given name to a given element and assigns 
//    /// it given value. ontologicalType is an ontological type of an element.
//    let instantiateAttribute element (ontologicalType: IDataElement) (name: string) value =
//        if elementHelper.HasAttribute name ontologicalType then
//            let attribute = elementHelper.Attribute name ontologicalType
//            elementHelper.AddSlot element attribute value
//        else
//            raise <| AttributeNotFoundException name

//    /// Adds a new instances of attributes whose names and initial values are provided in attributeValues into element.
//    /// ontologicalType is an ontological type of an element.
//    let instantiateAttributes (element: IDataElement) (ontologicalType: IDataElement) attributeValues =
//        attributeValues |> Map.iter (instantiateAttribute element ontologicalType)

//    /// Instantiates given node into given model, using given map to provide values for element attributes.
//    member this.InstantiateNode
//            (model: IDataModel)
//            (name: string)
//            (ontologicalType: IDataNode)
//            (attributeValues: Map<string, IDataNode>) =
//        let instance = model.CreateNode(name, ontologicalType, node)
//        instantiateAttributes instance ontologicalType attributeValues
//        instance

//    /// Instantiates given edge into given model, using given map to provide values for element attributes.
//    /// Rules for instantiation are the same as for instantiation of nodes.
//    member this.InstantiateAssociation 
//            (model: IDataModel)
//            (source: IDataNode)
//            (target: IDataNode)
//            (ontologicalType: IDataElement)
//            (attributeValues: Map<string, IDataNode>) =

//        let name =
//            match ontologicalType with
//            | :? IDataNode as n -> n.Name
//            | :? IDataAssociation as a -> a.TargetName
//            | _ -> failwith "Incorrect association ontological type"

//        let instance = 
//            model.CreateAssociation (
//                ontologicalType, 
//                association, 
//                source, 
//                target, 
//                name
//            )

//        instantiateAttributes instance ontologicalType attributeValues
//        instance
