(* Copyright 2017 Yurii Litvinov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. *)

namespace RepoExperimental.InfrastructureSemanticLayer

open RepoExperimental
open RepoExperimental.DataLayer
open RepoExperimental.CoreSemanticLayer

/// Helper functions to work with Infrastructure Metamodel.
module InfrastructureMetamodel =
    /// Searches for an Infrastructure Metamodel in current repository.
    let infrastructureMetamodel (repo: IRepo) =
        let models = 
            repo.Models 
            |> Seq.filter (fun m -> m.Name = "InfrastructureMetamodel")
        
        if Seq.isEmpty models then
            raise (MalformedInfrastructureMetamodelException "Infrastructure Metamodel not found in a repository")
        elif Seq.length models <> 1 then
            raise (MalformedInfrastructureMetamodelException 
                    "There is more than one Infrastructure Metamodel in a repository")
        else
            Seq.head models

    let findNode (repo: IRepo) name =
        let metamodel = infrastructureMetamodel repo
        CoreSemanticLayer.Model.findNode metamodel name

    let findAssociation (repo: IRepo) targetName =
        let metamodel = infrastructureMetamodel repo
        CoreSemanticLayer.Model.findAssociation metamodel targetName

    let private node (repo: IRepo) = findNode repo "Node"

    let private edge (repo: IRepo) = findNode repo "Edge"

    let private generalization (repo: IRepo) = findNode repo "Generalization"

    let isFromInfrastructureMetamodel repo element =
        Element.containingModel repo element = infrastructureMetamodel repo
    
    let isNode repo (element: IElement) =
        if isFromInfrastructureMetamodel repo element then
            element :? INode
        else
            CoreSemanticLayer.Element.isInstanceOf (node repo) element

    let isEdge repo (element: IElement) =
        if isFromInfrastructureMetamodel repo element then
            element :? IAssociation
        else
            CoreSemanticLayer.Element.isInstanceOf (edge repo) element

    let isGeneralization repo (element: IElement) =
        if isFromInfrastructureMetamodel repo element then
            element :? IGeneralization
        else
            CoreSemanticLayer.Element.isInstanceOf (generalization repo) element

    let isRelationship (repo: IRepo) element =
        isEdge repo element || isGeneralization repo element

    let isElement repo element =
        isNode repo element || isRelationship repo element

module Element =
    let attributes repo element =
        if InfrastructureMetamodel.isFromInfrastructureMetamodel repo element then
            // Elements in Infrastructure Metamodel do not have "ontological" attributes, only linguistic ones
            // (they play ontological role for this metamodel, but from the point of view of tools it is 
            // not important).
            // NODE: Actually Infrastructure Metamodel shall be a proper instance of itself and of Language Metamodel.
            Seq.empty
        else
            let attributesAssociation = InfrastructureMetamodel.findAssociation repo "attributes"
            element
            |> Element.outgoingAssociations repo
            |> Seq.filter (Element.isInstanceOf attributesAssociation)
            |> Seq.map (fun l -> l.Target)
            |> Seq.choose id
            |> Seq.filter (fun e -> e :? DataLayer.INode)
            |> Seq.cast<DataLayer.INode>

module Operations =
    let private attributeLink repo element attribute =
        let attributeLinks = 
            Element.outgoingAssociations repo element
            |> Seq.filter (fun a -> a.Target = Some attribute)

        if Seq.isEmpty attributeLinks then
            raise (InvalidSemanticOperationException "Attribute link not found for attribute")
        elif Seq.length attributeLinks <> 1 then
            raise (InvalidSemanticOperationException "Attribute connected with multiple links to a node, model error.")
        else
            Seq.head attributeLinks

    // Hardcoded attribute values for all Infrastructure Metamodel nodes, to be able to instantiate them. Actually
    // to be instantiable Infrastructure Metamodel shall be an instance of Infrastructure Metamodel, but it will
    // not be done until orthogonal metamodelling will be fully supported (not in v1).
    let private attributesMap =
        Map.empty
            .Add("Element", [("shape", "Pictures/Vertex.png"); ("isAbstract", "true"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Node"); ("name", "Element")] |> Map.ofList)
            .Add("Node", [("shape", "Pictures/Vertex.png"); ("isAbstract", "false"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Node"); ("name", "Node")] |> Map.ofList)
            .Add("Relationship", [("shape", "Pictures/Edge.png"); ("isAbstract", "true"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Edge"); ("name", "Relationship")] |> Map.ofList)
            .Add("Generalization", [("shape", "Pictures/Edge.png"); ("isAbstract", "false"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Edge"); ("name", "Generalization")] |> Map.ofList)
            .Add("Edge", [("shape", "Pictures/Edge.png"); ("isAbstract", "false"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Edge"); ("name", "Edge")] |> Map.ofList)
            .Add("Attribute", [("shape", "Pictures/Vertex.png"); ("isAbstract", "true"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Node"); ("name", "Attribute")] |> Map.ofList)
            .Add("Model", [("shape", "Pictures/Vertex.png"); ("isAbstract", "true"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Node"); ("name", "Model")] |> Map.ofList)
            .Add("Repo", [("shape", "Pictures/Vertex.png"); ("isAbstract", "true"); ("metatype", "Metatype.Node"); ("instanceMetatype", "Metatype.Node"); ("name", "Repo")] |> Map.ofList)

    let private attributeTypesMap = [("shape", "String"); ("isAbstract", "Boolean"); ("instanceMetatype", "Metatype")] |> Map.ofList

    let private copyAttribute repo element (``class``: IElement) name =
        if not <| InfrastructureMetamodel.isFromInfrastructureMetamodel repo ``class`` then
            let attributeClassNode = Element.attribute repo ``class`` name
            let kind = Element.attributeValue repo attributeClassNode "kind"
            let attributeAssociation = InfrastructureMetamodel.findAssociation repo name
            let kindNode = InfrastructureMetamodel.findNode repo kind
            let defaultValue = Element.attributeValue repo ``class`` name
            Element.addAttribute repo element name kindNode attributeAssociation defaultValue
        else
            // Here we create attribute by not copying, but instead creating new attribute, because Infrastructure 
            // Metamodel has attributes in form of Core Metamodel.
            let attributeClassNode = Element.attribute repo ``class`` name
            let kind = attributeTypesMap.[name]
            let attributeAssociation = InfrastructureMetamodel.findAssociation repo name
            let kindNode = InfrastructureMetamodel.findNode repo kind
            let defaultValue = attributesMap.[(``class`` :?> INode).Name].[name]
            Element.addAttribute repo element name kindNode attributeAssociation defaultValue

    let private copySimpleAttribute repo element (``class``: IElement) name =
        let attributeClassNode = Element.attribute repo ``class`` name
        let attributeAssociation = InfrastructureMetamodel.findAssociation repo name
        let defaultValue = Element.attributeValue repo ``class`` name
        Element.addAttribute repo element name attributeClassNode attributeAssociation defaultValue

    let private addAttribute repo (model: IModel) (element: IElement) (attributeClass: INode) =
        let attributeLink = attributeLink repo element.Class attributeClass
        let attributeNode = model.CreateNode(attributeClass.Name, attributeClass)
        model.CreateAssociation(attributeLink, element, attributeNode, attributeClass.Name) |> ignore

        copySimpleAttribute repo attributeNode attributeClass "stringValue"
        copySimpleAttribute repo attributeNode attributeClass "kind"

    let private attributeValue repo (element: IElement) name =
        if InfrastructureMetamodel.isFromInfrastructureMetamodel repo element then
            if element :? INode then
                attributesMap.[(element :?> INode).Name].[name]
            else
                raise (InvalidSemanticOperationException 
                    "Trying to get attribute value from an edge in an Infrastructure Model. It shall not be needed.")
        else
            Element.attributeValue repo element name

    let instantiate (repo: IRepo) (model: IModel) (``class``: IElement) =
        if attributeValue repo ``class`` "isAbstract" = "true" then
            raise (InvalidSemanticOperationException "Trying to instantiate abstract node")
        
        let name = 
            match ``class`` with
            | :? INode as n -> "a" + n.Name
            | :? IAssociation as a -> a.TargetName
            | _ -> raise (InvalidSemanticOperationException 
                    "Trying to instantiate something that should not be instantiated")

        let newElement = 
            if attributeValue repo ``class`` "instanceMetatype" = "Metatype.Node" then
                model.CreateNode(name, ``class``) :> IElement
            else
                model.CreateAssociation(``class``, None, None, name) :> IElement

        let attributes = Element.attributes repo ``class``
        attributes |> Seq.iter (addAttribute repo model newElement)

        let copyAttribute = 
            if InfrastructureMetamodel.isFromInfrastructureMetamodel repo ``class`` then
                copyAttribute repo newElement ``class``
            else
                // For now metamodel does not contain linguistic attributes ("shape", "isAbstract" and so on) as
                // proper attributes, since we have no way to tell them from "true" attributes that can be edited.
                // Actually, attributes shall have potency and be copied as proper attributes if their potency allows.
                copySimpleAttribute repo newElement ``class``

        copyAttribute "shape"
        copyAttribute "isAbstract"
        copyAttribute "instanceMetatype"

        newElement
