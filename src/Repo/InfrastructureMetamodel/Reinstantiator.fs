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

///// Module that allows to reinstantiate Infrastructure Metametamodel. It is an important and quite complex step in 
///// creation of Infrastructure Metamodel.
//module Repo.InfrastructureMetamodel.Reinstantiator

//open Repo
//open Repo.DataLayer

///// Helper function that creates a copy of a given edge in a current model (assuming that source and target are 
///// already in a model).
//let private reinstantiateEdge (repo: IDataRepository) (model: IDataModel) (edge: IDataEdge) =
//    let infrastructureMetametamodel = repo.Model Consts.infrastructureMetametamodel

//    let languageMetamodel = repo.Model "LanguageMetamodel"
//    let languageMetamodelGeneralization = languageMetamodel.Node "Generalization"
//    let languageMetamodelAssociation = languageMetamodel.Node "Association"

//    // We are interested only in edges that are either generalizations or associations created as "linguistical 
//    // extensions"
//    if edge.OntologicalType = (languageMetamodelGeneralization :> IDataElement)
//        || edge.OntologicalType = (languageMetamodelAssociation :> IDataElement)
//    then
//        let isExactlyOneWithThisName (elementOption: IDataElement option) = 
//            CoreMetamodel.ModelSemantics.FindNodes model (elementOption.Value :?> IDataNode).Name 
//            |> Seq.length = 1

//        if isExactlyOneWithThisName edge.Source && isExactlyOneWithThisName edge.Target then
//            CoreMetamodel.CoreSemanticsHelpers.reinstantiateEdge edge model infrastructureMetametamodel
//        else 
//            failwith "Ambiguous reinstantiation"

///// Helper function that creates a proper instance of a given node in a current model. All instances of Node
///// will remain instances of Node, all attributes will become the instances of Attribute, all values will become
///// instances of corresponding type. Infrastructure Metamodel instantiation semantics will apply --- all attributes
///// will get corresponding slots. But original attributes will be retained since it is reinstantiation.
//let private reinstantiateNode (repo: IDataRepository) (model: IDataModel) (node: IDataNode) =
//    let infrastructureMetametamodel = repo.Model Consts.infrastructureMetametamodel

//    let languageElementSemantics = LanguageMetamodel.Semantics.ElementSemantics infrastructureMetametamodel.LinguisticMetamodel
//    let infrastructureElementSemantics = LanguageMetamodel.Semantics.ElementSemantics infrastructureMetametamodel
//    let metametamodelNode = infrastructureMetametamodel.Node "Node"
//    let metametamodelEnum = infrastructureMetametamodel.Node "Enum"
//    let metametamodelSlot = infrastructureMetametamodel.Node "Slot"
//    let metametamodelString = infrastructureMetametamodel.Node "String"

//    let metametamodelEnumElementsAssociation = 
//        LanguageMetamodel.Semantics.ModelSemantics.FindAssociationWithSource 
//            (infrastructureMetametamodel.Node "Enum") 
//            "elements" 

//    let languageMetamodel = repo.Model "LanguageMetamodel"
//    let languageMetamodelNode = languageMetamodel.Node "Node"
//    let languageMetamodelEnum = languageMetamodel.Node "Enum"

//    let addSlot (element: IDataElement) (attribute: IDataNode) (value: IDataNode) =
//        let model = CoreMetamodel.ElementSemantics.ContainingModel element
//        let slotNode = model.CreateNode(attribute.Name, attribute, metametamodelSlot)
//        let attributeAssociation = attribute.IncomingEdges |> Seq.exactlyOne
//        let attributeTypeAssociation = CoreMetamodel.ElementSemantics.OutgoingAssociation attribute "type"

//        let slotAssociation = infrastructureMetametamodel.Association "slots"
//        let slotValueAssociation = infrastructureMetametamodel.Association "value"

//        model.CreateAssociation
//                (
//                attributeAssociation,
//                slotAssociation,
//                element,
//                slotNode,
//                attribute.Name
//                ) |> ignore

//        model.CreateAssociation
//                (
//                attributeTypeAssociation,
//                slotValueAssociation,
//                slotNode,
//                value,
//                "value"
//                ) |> ignore

//    let instantiateAttribute element (ontologicalType: IDataElement) name value =
//        if languageElementSemantics.HasAttribute name ontologicalType then
//            let attribute = languageElementSemantics.Attribute name ontologicalType
//            addSlot element attribute value
//        else
//            failwithf "Invalid attribute reinstantiation, node: %s, attribute: %s" (node.ToString ()) name

//    let instantiateAttributes (element: IDataElement) (ontologicalType: IDataElement) attributeValues =
//        attributeValues |> Map.iter (instantiateAttribute element ontologicalType)

//    let instantiateNode
//            (name: string)
//            (ontologicalType: IDataNode)
//            (attributeValues: Map<string, IDataNode>) =
//        let instance = model.CreateNode(name, ontologicalType, metametamodelNode)
//        instantiateAttributes instance ontologicalType attributeValues
//        instance

//    if node.OntologicalType = (languageMetamodelNode :> IDataElement) then
//        let toAttributeInfo attribute =
//            { Name = AttributeMetamodel.Semantics.AttributeSemantics.Name attribute;
//                Type = AttributeMetamodel.Semantics.AttributeSemantics.Type attribute;
//                DefaultValue = AttributeMetamodel.Semantics.AttributeSemantics.DefaultValue attribute}

//        let attributes = languageElementSemantics.Attributes metametamodelNode
//        let slots = 
//            attributes
//            |> Seq.map toAttributeInfo
//            |> Seq.map 
//                (fun attr -> 
//                    (attr.Name, model.CreateNode(attr.DefaultValue.Name, attr.Type, metametamodelSlot))
//                    )
//            |> Map.ofSeq

//        let instance = instantiateNode node.Name metametamodelNode slots

//        languageElementSemantics.OwnAttributes node
//        |> Seq.iter 
//            (fun attr ->
//                infrastructureElementSemantics.AddAttribute 
//                    instance
//                    (AttributeMetamodel.Semantics.AttributeSemantics.Name attr)
//                    (AttributeMetamodel.Semantics.AttributeSemantics.Type attr)
//                    (AttributeMetamodel.Semantics.AttributeSemantics.DefaultValue attr)
//            )
//        ()
//    elif node.OntologicalType = (languageMetamodelEnum :> IDataElement) then
//        let instance = model.CreateNode(node.Name, metametamodelEnum, metametamodelEnum)
//        let enumElements = LanguageMetamodel.Semantics.EnumSemantics.Values node
        
//        enumElements 
//        |> Seq.iter (fun n ->
//            let enumLiteral = model.CreateNode(n.Name, metametamodelString, metametamodelString)
//            model.CreateAssociation(
//                metametamodelEnumElementsAssociation,
//                metametamodelEnumElementsAssociation,
//                instance,
//                enumLiteral,
//                "elements"
//            ) |> ignore
//        )

///// Instantiates an exact copy of Infrastructure Metametamodel in a given model. 
//let reinstantiateInfrastructureMetametamodel (repo: IDataRepository) (model: IDataModel) =
//    let infrastructureMetametamodel = repo.Model Consts.infrastructureMetametamodel
//    infrastructureMetametamodel.Nodes |> Seq.iter (reinstantiateNode repo model)
//    infrastructureMetametamodel.Edges |> Seq.iter (reinstantiateEdge repo model)
//    ()
