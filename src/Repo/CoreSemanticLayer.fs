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

namespace Repo.CoreSemanticLayer

open Repo
open Repo.DataLayer

/// Helper functions for working with repository.
module Repo =
    /// Searches model by name in a repository, throws InvalidSemanticOperationException if not found or multiple 
    /// models found.
    let findModel (repo: IRepo) (name: string) =
        let models = 
            repo.Models 
            |> Seq.filter (fun m -> m.Name = name)
        
        if Seq.isEmpty models then
            raise (ModelNotFoundException name)
        elif Seq.length models <> 1 then
            raise (MultipleModelsWithGivenNameException name)
        else
            Seq.head models

    /// Searches for a Core Metamodel in current repository.
    let coreMetamodel (repo: IRepo) = 
        findModel repo "CoreMetametamodel"

/// Helper functions for element semantics.
module Element =
    /// Returns a model containing given element.
    /// Throws InvalidSemanticOperationException if there is no such model or there is more than one model which 
    /// contains given element.
    let containingModel (repo: IRepo) element =
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

        (containingModel repo element).Edges |> Seq.filter isOutgoingRelationship

    /// Returns all outgoing generalizations for an element.
    let outgoingGeneralizations (repo: IRepo) element = 
        outgoingRelationships repo element |> Seq.filter (fun r -> r :? IGeneralization) |> Seq.cast<IGeneralization>

    /// Returns all outgoing associations for an element.
    let outgoingAssociations (repo: IRepo) element = 
        outgoingRelationships repo element |> Seq.filter (fun r -> r :? IAssociation) |> Seq.cast<IAssociation>

    /// Returns a sequence of all parents (in terms of generalization hierarchy) for given element.
    /// Most special node is the first in resulting sequence, most general is the last.
    let rec parents repo element =
        outgoingGeneralizations repo element
        |> Seq.map (fun g -> g.Target)
        |> Seq.choose id
        |> Seq.map (fun p -> Seq.append (Seq.singleton p) (parents repo p))
        |> Seq.concat

    /// Returns a sequence of attributes in a given element, ignoring generalization hierarchy.
    let private strictElementAttributes (repo: IRepo) element =
        outgoingAssociations repo element 
        |> Seq.map (fun a -> (a.TargetName, a))
        |> Seq.map (fun (name, a) -> (name, a.Target))
        |> Seq.choose (fun (name, n) -> if n.IsSome then Some (name, n.Value) else None)

    /// Searches for attribute in generalization hierarchy.
    let attributes (repo: IRepo) element = 
        let thisElementAttributes e = strictElementAttributes repo e
        parents repo element |> Seq.map (strictElementAttributes repo) |> Seq.concat |> Seq.append (thisElementAttributes element)

    /// Returns an attribute (target node of an outgoing association with given target name).
    /// Throws InvalidSemanticOperationException if there is no such association or there is more than one.
    let rec attribute (repo: IRepo) element name = 
        let attributes' = 
            attributes repo element 
        
        let attributes = 
            attributes'
            |> Seq.filter (fun (attrName, attr) -> attrName = name)
            |> Seq.map (fun (attrName, attr) -> attr)
    
        if Seq.isEmpty attributes then
            raise (AttributeNotFoundException name)
        else
            /// Here we use first found attribute, it is the deepest in inheritance hierarchy.
            /// NOTE: Multiple inheritance can lead to conflicting attributes, but multiple inheritance
            /// is not supported in v1.
            let attribute = Seq.head attributes
            match attribute with
            | :? INode as result -> result
            | _ -> raise (InvalidSemanticOperationException 
                <| sprintf "Attribute %s is not a node (which is possible but not used and not supported in v1)" name)

    /// Returns string value of a given attribute.
    let attributeValue (repo: IRepo) element name = 
        (attribute repo element name).Name

    /// Sets attribute value for given attribute.
    let setAttributeValue (repo: IRepo) element name value = 
        (attribute repo element name).Name <- value

    /// Adds a new attribute to an element.
    let addAttribute (repo: IRepo) element name ``class`` attributeAssociationClass value =
        let model = containingModel repo element
        let attributeNode = model.CreateNode(value, ``class``)
        model.CreateAssociation(attributeAssociationClass, element, attributeNode, name) |> ignore

    /// Returns true if an attribute with given name is present in given element.
    let hasAttribute repo element name =
        attributes repo element |> Seq.filter (fun (attrName, _) -> attrName = name) |> Seq.isEmpty |> not

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

    /// Searches for a given node in a given model by name, returns None if not found or found more than one.
    let tryFindNode (model: IModel) name = 
        let nodes = model.Nodes |> Seq.filter (fun m -> m.Name = name)        
        if Seq.isEmpty nodes || Seq.length nodes <> 1 then
            None
        else
            Some <| Seq.head nodes

    /// Searches for a given association in a given model by target name and additional predicate. Assumes that it 
    /// exists and there is only one such association. Throws InvalidSemanticOperationException if not.
    let findAssociationWithPredicate (model: IModel) targetName predicate = 
        let associations = 
            model.Edges 
            |> Seq.filter 
                (fun e -> e :? IAssociation 
                          && (predicate (e :?> IAssociation)) 
                          && (e :?> IAssociation).TargetName = targetName
                )
        
        if Seq.isEmpty associations then
            raise (InvalidSemanticOperationException <| sprintf "Edge %s not found in model %s" targetName model.Name)
        elif Seq.length associations <> 1 then
            raise (InvalidSemanticOperationException 
                <| sprintf "Edge %s appears more than once in model %s" targetName model.Name)
        else
            Seq.head associations :?> IAssociation

    /// Searches for a given association in a given model by target name. Assumes that it exists and there is only one 
    /// association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociation (model: IModel) targetName = 
        findAssociationWithPredicate model targetName (fun _ -> true)

    /// Searches for a given association starting in a given element with a given name. Assumes that it exists and 
    /// there is only one association with that name. Throws InvalidSemanticOperationException if not.
    let findAssociationWithSource (model: IModel) (element: IElement) targetName = 
        findAssociationWithPredicate model targetName (fun a -> a.Source = Some element)
