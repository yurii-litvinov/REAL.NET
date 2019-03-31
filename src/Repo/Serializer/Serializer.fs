(* Copyright 2018 Yurii Litvinov
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

namespace Repo.Serializer

open System.IO
open System.IO.Compression
open System.Collections.Generic
open Newtonsoft.Json
open Repo.DataLayer

/// Serializes repository contents into a file.
module Serializer =
    /// Internal switch that allows to turn compression on and off. By default it is on, but turning it off greatly
    /// helps debugging.
    let useCompression = false

    /// Repository of objects to be serialized. Needs to be maintained for correct reference serialization.
    let private wrappedElements = Dictionary<IElement, WrappedElement>()

    /// Helper function that registers new element in a repository.
    let private register key value =
        wrappedElements.Add(key, value)
        value

    /// Function that wraps element from repository to its corresponding wrapper object for serialization.
    /// Maintains a repository of wrapped objects, so if this object was seen already, does not create a new object.
    let rec private wrap (element: IElement option) =
        match element with
        | None -> null
        | Some e -> 
            if wrappedElements.ContainsKey e then
                wrappedElements.[e]
            else
                match e with
                | :? INode as n -> wrapNode n :> WrappedElement
                | :? IAssociation as a -> wrapAssociation a :> WrappedElement
                | :? IGeneralization as g -> wrapGeneralization g :> WrappedElement
                | _ -> failwith "Unknown element type in data repo, can not serialize"

    /// Function that adds IElement-specific to already constructed data object.
    and private wrapElement (wrappedElement: WrappedElement) (element: IElement) =
        if element.Class = element then 
            wrappedElement.Class <- null
        else
            wrappedElement.Class <- wrap (Some element.Class)

    /// Function that adds IEdge-specific to already constructed data object.
    and private wrapEdge (wrappedEdge: WrappedEdge) (edge: IEdge) =
        wrapElement wrappedEdge edge
        wrappedEdge.Source <- wrap edge.Source
        wrappedEdge.Target <- wrap edge.Target

    /// Function that wraps INode from a repository into data object, registers it if needed and returns wrapped 
    /// object. If this INode was seen already, returns old object from repository.
    and private wrapNode (node: INode) =
        if wrappedElements.ContainsKey node then
            wrappedElements.[node] :?> WrappedNode
        else
            let wrappedNode = WrappedNode()
            wrapElement wrappedNode node
            wrappedNode.Name <- node.Name
            register node wrappedNode

    /// Function that wraps IAssociation from a repository into data object, registers it if needed and returns wrapped
    /// object. If this IAssociation was seen already, returns old object from repository.
    and private wrapAssociation (association: IAssociation) =
        if wrappedElements.ContainsKey association then
            wrappedElements.[association] :?> WrappedAssociation
        else
            let wrappedAssociation = WrappedAssociation()
            wrapEdge wrappedAssociation association
            wrappedAssociation.TargetName <- association.TargetName
            register association wrappedAssociation

    /// Function that wraps IGeneralization from a repository into data object, registers it if needed and returns 
    /// wrapped object. If this IGeneralization was seen already, returns old object from repository.
    and private wrapGeneralization (generalization: IGeneralization) =
        if wrappedElements.ContainsKey generalization then
            wrappedElements.[generalization] :?> WrappedGeneralization
        else
            let wrappedGeneralization = WrappedGeneralization()
            wrapEdge wrappedGeneralization generalization
            register generalization wrappedGeneralization

    /// Function that wraps a model and all its contents into data object.
    let private wrapModel (model: IModel) =
        let wrappedModel = WrappedModel ()
        wrappedModel.Name <- model.Name
        wrappedModel.MetamodelName <- model.Metamodel.Name
        wrappedModel.Properties <- Map.toArray model.Properties
        wrappedModel.Nodes <- model.Nodes |> Seq.rev |> Seq.map wrapNode |> Seq.toArray

        let associations (e: IEdge) =
            match e with
            | :? IAssociation as a -> Some a
            | _ -> None
        wrappedModel.Associations <- 
            model.Edges 
            |> Seq.rev 
            |> Seq.choose associations 
            |> Seq.map wrapAssociation 
            |> Seq.toArray

        let generalizations (e: IEdge) =
            match e with
            | :? IGeneralization as a -> Some a
            | _ -> None
        wrappedModel.Generalizations <- 
            model.Edges 
            |> Seq.rev 
            |> Seq.choose generalizations 
            |> Seq.map wrapGeneralization 
            |> Seq.toArray

        wrappedModel

    /// Function that wraps entire repository into data object.
    let private wrapRepo (repo: IRepo) =
        let save = Save ()
        save.Contents.Models <- repo.Models |> Seq.rev |> Seq.map wrapModel |> Seq.toArray
        save

    /// Saves given repository contents into a given file name.
    let save fileName repo =
        let save = wrapRepo repo

        use saveFileStream = File.Create fileName
        use saveFileStream = 
            if useCompression then
                new GZipStream(saveFileStream, CompressionMode.Compress) :> Stream
            else
                saveFileStream :> Stream

        use writer = new JsonTextWriter(new StreamWriter(saveFileStream))
        let serializer = new JsonSerializer();
        serializer.Formatting <- Formatting.Indented
        serializer.PreserveReferencesHandling <- PreserveReferencesHandling.Objects
        serializer.Serialize(writer, save)

        wrappedElements.Clear()
