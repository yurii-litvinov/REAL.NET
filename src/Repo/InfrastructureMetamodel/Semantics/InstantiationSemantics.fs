//(* Copyright 2019 Yurii Litvinov
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

//namespace Repo.InfrastructureMetamodel.Semantics

//open Repo
//open Repo.DataLayer
//open Repo.InfrastructureMetamodel

///// Helper class that provides low-level operations with a model conforming to Infrastructure Metamodel.
//type InstantiationSemantics(metamodel: IDataModel) =
//    let infrastructureMetamodel = InfrastructureMetamodelHelper(metamodel)
//    let infrastructureMetamodelElementSemantics = ElementSemantics(metamodel)
//    let infrastuctureMetametamodelElementSemantics = ElementSemantics(metamodel.LinguisticMetamodel)

//    /// Determines appropriate semantics for a node based on its linguistic metamodel. There are two cases:
//    /// - nodes from Infrastructure Metamodel use Infrastructure Metametamodel as their linguistic model
//    /// - nodes from subsequent metalayers all use Infrastructure Metamodel as their linguistic model
//    /// So when instantiating node from Infrastructure Metamodel, we need Infrastructure Metametamodel semantics,
//    /// when instantiating node from subsequent metalayers, we need Infrastructure Metamodel semantics.
//    let semantics (node: IDataElement) = 
//        if node.Model = infrastructureMetamodel.Metamodel then
//            infrastuctureMetametamodelElementSemantics
//        else
//            infrastructureMetamodelElementSemantics

//    /// Instantiates given element into given model, using given map to provide values for element attributes.
//    let instantiateElementWithLinguisticType
//            (model: IDataModel)
//            (name: string)
//            (source: IDataElement option)
//            (target: IDataElement option)
//            (ontologicalType: IDataElement)
//            (linguisticType: IDataElement)
//            (attributeValues: Map<string, IDataNode>) =
//        // Type can be from Infrastructure Metamodel or from metamodels of subsequent metalevels, so we need to choose
//        // semantics accordingly.
//        let typeSemantics = semantics ontologicalType
        
//        // All instances are always linguistic instances of Infrastructure Metamodel, so we shall always use its 
//        // semantics.
//        let elementSemantics = infrastructureMetamodelElementSemantics

//        // Linguistic type, on the other hand, is always from Infrastructure Metamodel, so we shall always use
//        // Infrastructure Metametamodel to deal with it.
//        let linguisticTypeSemantics = infrastuctureMetametamodelElementSemantics

//        if typeSemantics.StringSlotValue Consts.isAbstract ontologicalType <> Consts.stringFalse then
//            raise (InvalidSemanticOperationException "Trying to instantiate abstract element")

//        let newElement =
//            if typeSemantics.StringSlotValue Consts.instanceMetatype ontologicalType = Consts.metatypeNode then
//                model.CreateNode(name, ontologicalType, linguisticType) :> IDataElement
//            else
//                model.CreateAssociation(
//                    ontologicalType, 
//                    linguisticType, 
//                    source, 
//                    target, 
//                    name
//                    ) :> IDataElement

//        let ontologicalAttributes = typeSemantics.Attributes ontologicalType

//        let slotValue (attr: IDataNode) =
//            let slotName = AttributeMetamodel.Semantics.AttributeSemantics.Name attr
//            if attributeValues.ContainsKey slotName then 
//                attributeValues.[slotName]
//            else
//                AttributeMetamodel.Semantics.AttributeSemantics.DefaultValue attr

//        ontologicalAttributes 
//        |> Seq.rev 
//        |> Seq.iter (fun a -> elementSemantics.AddSlot newElement a (slotValue a))

//        let linguisticAttributes = linguisticTypeSemantics.Attributes linguisticType

//        linguisticAttributes 
//        |> Seq.filter (fun a -> ontologicalAttributes |> Seq.map (fun attr -> attr.Name) |> Seq.contains a.Name |> not)
//        |> Seq.iter (fun a -> elementSemantics.AddSlot newElement a (slotValue a))

//        newElement

//    /// Instantiates given node into given model, using given map to provide values for element attributes.
//    member this.InstantiateElement
//            (model: IDataModel)
//            (name: string)
//            (ontologicalType: IDataElement)
//            (attributeValues: Map<string, IDataNode>) =
//        let typeSemantics = semantics ontologicalType

//        let linguisticType = 
//            if typeSemantics.StringSlotValue Consts.instanceMetatype ontologicalType = Consts.metatypeNode then
//                infrastructureMetamodel.Node :> IDataElement
//            else
//                infrastructureMetamodel.Association :> IDataElement

//        instantiateElementWithLinguisticType 
//            model 
//            name 
//            None
//            None
//            ontologicalType 
//            linguisticType
//            attributeValues

//    /// Instantiates an association into given model, using given map to provide values for its attributes.
//    member this.InstantiateAssociation
//        (model: IDataModel)
//        (name: string)
//        (source: IDataElement)
//        (target: IDataElement)
//        (ontologicalType: IDataElement)
//        (attributeValues: Map<string, IDataNode>) =

//        let typeSemantics = semantics ontologicalType

//        let linguisticType = 
//            if typeSemantics.StringSlotValue Consts.instanceMetatype ontologicalType = Consts.metatypeNode then
//                infrastructureMetamodel.Node :> IDataElement
//            else
//                infrastructureMetamodel.Association :> IDataElement

//        instantiateElementWithLinguisticType 
//            model 
//            name 
//            (Some source)
//            (Some target)
//            ontologicalType 
//            linguisticType
//            attributeValues

//    /// Creates a new string instance in a given model.
//    member this.InstantiateString (model: IDataModel) (value: string) =
//        instantiateElementWithLinguisticType 
//            model
//            value
//            None
//            None
//            infrastructureMetamodel.String
//            infrastructureMetamodel.String
//            Map.empty
//        :?> IDataNode

//    /// Creates a new int instance in a given model.
//    member this.InstantiateInt (model: IDataModel) (value: string) =
//        match System.Int32.TryParse(value) with
//        | (true, _ ) -> 
//            instantiateElementWithLinguisticType 
//                model 
//                value 
//                None 
//                None
//                infrastructureMetamodel.Int
//                infrastructureMetamodel.Int
//                Map.empty
//            :?> IDataNode
//        | _ -> failwith "Trying to add int node with non-int value"

//    /// Instantiates an element into a given model, using default name and default values for all attributes.
//    member this.Instantiate (model: IDataModel) (ontologicalType: IDataElement) =
//        let name =
//            match ontologicalType with
//            | :? IDataNode as n -> 
//                if n.Name = Consts.generalization then
//                    Consts.generalization
//                else 
//                    "a" + n.Name
//            | :? IDataAssociation as a -> a.TargetName
//            | _ -> raise (InvalidSemanticOperationException
//                    "Trying to instantiate something that should not be instantiated")

//        this.InstantiateElement model name ontologicalType Map.empty

//    member this.Metamodel = infrastructureMetamodel
//    member this.Element = infrastructureMetamodelElementSemantics
