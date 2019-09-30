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

///// Helper class for working with Infrastructure Metamodel. Provides most commonly used metamodel nodes and edges.
//type InfrastructureMetamodelHelper(metamodel: IDataModel) =
    
//    let findNode name = metamodel.Node name

//    let element = findNode "Element"
//    let node = findNode "Node"
//    let association = findNode "Association"
//    let generalization = findNode "Generalization"
//    let attribute = findNode "Attribute"
//    let slot = findNode "Slot"
//    let stringNode = findNode "String"
//    let booleanNode = findNode "Boolean"
//    let intNode = findNode "Int"

//    let attributesAssociation = CoreMetamodel.ModelSemantics.FindAssociationWithSource element "attributes"
//    let attributeTypeAssociation = CoreMetamodel.ModelSemantics.FindAssociationWithSource attribute "type"
//    let attributeDefaultValueAssociation = 
//        CoreMetamodel.ModelSemantics.FindAssociationWithSource attribute "defaultValue"

//    let slotsAssociation = AttributeMetamodel.Semantics.ModelSemantics.FindAssociationWithSource element "slots"
//    let valueAssociation = AttributeMetamodel.Semantics.ModelSemantics.FindAssociationWithSource slot "value"

//    let isLinguisticType (element: IDataElement) (linguisticType: IDataElement) =
//        element.LinguisticType = linguisticType

//    let getMetamodel (element: IDataElement) = 
//        if element.Model = metamodel then  
//            metamodel.LinguisticMetamodel
//        else
//            metamodel

//    member this.Metamodel = metamodel

//    member this.Node = node

//    member this.Association = association

//    member this.Generalization = generalization

//    member this.Attribute = attribute
//    member this.String = stringNode
//    member this.Int = intNode
//    member this.Boolean = booleanNode

//    member this.AttributesAssociation = attributesAssociation
//    member this.AttributeTypeAssociation = attributeTypeAssociation
//    member this.AttributeDefaultValueAssociation = attributeDefaultValueAssociation

//    member this.IsFromInfrastructureMetamodel element =
//        CoreMetamodel.ElementSemantics.ContainingModel element = metamodel

//    member this.IsNode element =
//        isLinguisticType element <| (getMetamodel element).Node "Node"

//    member this.IsAssociation element =
//        isLinguisticType element <| (getMetamodel element).Node "Association"

//    member this.IsGeneralization element =
//        isLinguisticType element <| (getMetamodel element).Node "Generalization"

//    member this.IsEdge element =
//        this.IsAssociation element || this.IsGeneralization element

//    member this.IsElement element =
//        this.IsNode element || this.IsEdge element

//    member this.IsAttribute element =
//        isLinguisticType element <| this.Metamodel.Node "Attribute"

//    member this.IsSlot element =
//        isLinguisticType element <| this.Metamodel.Node "Slot"

//    member this.IsAttributeAssociation association =
//        isLinguisticType association attributesAssociation

//    member this.IsAttributeTypeAssociation association =
//        isLinguisticType association attributeTypeAssociation

//    member this.IsAttributeDefaultValueAssociation association =
//        isLinguisticType association attributeDefaultValueAssociation

//    member this.IsSlotAssociation association =
//        isLinguisticType association slotsAssociation

//    member this.IsSlotValueAssociation association =
//        isLinguisticType association valueAssociation
