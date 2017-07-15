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

namespace RepoExperimental.CoreSemanticLayer

open RepoExperimental
open RepoExperimental.DataLayer

/// Helper functions for working with Core Metamodel.
module CoreMetamodel =
    /// Searches for a Core Metamodel in current repository.
    let coreMetamodel (repo: IRepo) = 
        let models = 
            repo.Models 
            |> Seq.filter (fun m -> m.Name = "CoreMetamodel")
        
        if Seq.isEmpty models then
            raise (MalformedCoreMetamodelException "Core Metamodel not found in a repository")
        elif Seq.length models <> 1 then
            raise (MalformedCoreMetamodelException "There is more than one Core Metamodel in a repository")
        else
            Seq.head models

/// Helper functions for working with models.
module Model =
    /// Searches for a given node in a given model by name. Assumes that it exists and there is only one node with
    /// that name.
    let findNode (model: IModel) name = 
        let nodes = model.Nodes |> Seq.filter (fun m -> m.Name = name)
        
        if Seq.isEmpty nodes then
            raise (InvalidSemanticOperationException <| sprintf "Node %s not found in model %s" name model.Name)
        elif Seq.length nodes <> 1 then
            raise (InvalidSemanticOperationException 
                <| sprintf "Node %s appears more than once in model %s" name model.Name)
        else
            Seq.head nodes

    /// Searches for a given association in a given model by target name. Assumes that it exists and there is only one 
    /// association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociation (model: IModel) targetName = 
        let associations = 
            model.Edges 
            |> Seq.filter (fun m -> m :? IAssociation && (m :?> IAssociation).TargetName = targetName)
        
        if Seq.isEmpty associations then
            raise (InvalidSemanticOperationException <| sprintf "Edge %s not found in model %s" targetName model.Name)
        elif Seq.length associations <> 1 then
            raise (InvalidSemanticOperationException 
                <| sprintf "Edge %s appears more than once in model %s" targetName model.Name)
        else
            Seq.head associations :?> IAssociation

/// Helper functions for element semantics
module Element =
    /// Returns a model containing given element.
    /// Throws InvalidSemanticOperationException if there is no such model or there is more than one model which 
    /// contains given element.
    let private getContainingModel (repo: IRepo) element =
        let models = repo.Models |> Seq.filter (fun m -> Seq.contains element m.Elements)

        if Seq.isEmpty models then
            raise (InvalidSemanticOperationException "Element not found in repository")
        elif Seq.length models <> 1 then
            raise (InvalidSemanticOperationException "Element belongs to more than one model, REAL.NET \
                   does not allow this")
        else
            Seq.head models

    /// Returns all outgoing relationships for an element.
    let outgoingRelationships (repo: IRepo) element = 
        let isOutgoingRelationship: DataLayer.IElement -> bool = function
        | :? DataLayer.IRelationship as a -> a.Source = Some element 
        | _ -> false

        (getContainingModel repo element).Edges |> Seq.filter isOutgoingRelationship

    /// Returns all outgoing generalizations for an element.
    let outgoingGeneralizations (repo: IRepo) element = 
        outgoingRelationships repo element |> Seq.filter (fun r -> r :? IGeneralization) |> Seq.cast<IGeneralization>

    /// Returns all outgoing associations for an element.
    let outgoingAssociations (repo: IRepo) element = 
        outgoingRelationships repo element |> Seq.filter (fun r -> r :? IAssociation) |> Seq.cast<IAssociation>

    /// Returns an attribute (target node of an outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    let attribute (repo: IRepo) element name = 
        let attributes = 
            outgoingAssociations repo element 
            |> Seq.filter (fun a -> a.TargetName = name)
            |> Seq.map (fun a -> a.Target)
            |> Seq.choose id

        if Seq.isEmpty attributes then
            raise (InvalidSemanticOperationException <| sprintf "Attribute %s not found for an element" name)
        elif Seq.length attributes <> 1 then
            raise (InvalidSemanticOperationException <| sprintf "Attribute %s has multiplicity more than 1" name)
        else
            let attribute = Seq.head attributes
            match attribute with
            | :? INode as result -> result
            | _ -> raise (InvalidSemanticOperationException 
                <| sprintf "Attribute %s is not a node (which is possible but not used and not supported in v1" name)

    /// Returns string value of a given attribute.
    let attributeValue (repo: IRepo) element name = 
        (attribute repo element name).Name

    /// Returns true if an 'instance' is an (indirect) instance of a 'class'.
    let rec isInstanceOf (``class``: IElement) (instance: IElement) =
        if instance.Class = ``class`` then
            true
        elif instance.Class = instance then
            false
        else
            isInstanceOf ``class`` instance.Class

/// Helper functions for node semantics.
module Node =
    /// Returns name of a node.
    /// Throws InvalidSemanticOperationException if given element is not node so it does not have a name.
    let name (element: IElement) =
        if not <| element :? INode then
            raise (InvalidSemanticOperationException "Only nodes have a name in REAL.NET")
        (element :?> INode).Name
