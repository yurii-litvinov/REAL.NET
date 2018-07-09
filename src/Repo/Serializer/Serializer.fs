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

/// Serializes/deserializes repository contents into a file.
module Serializer =
    /// Internal switch that allows to turn compression on and off. By default it is on, but turning it off greatly
    /// helps debugging.
    let private useCompression = false

    /// Helper class for element serialization. Allows null value, denoting the absense of value in serialized file.
    /// Null as a Class is a special value denoting that this element is a class for itself 
    /// (so it is CoreMetametamodel Node). All other elements must have actual classes, so can not be null.
    [<JsonObject>]
    [<AllowNullLiteral>]
    type WrappedElement(isSerializing: bool, element: IElement, wrap: IElement option -> WrappedElement) =
        let mutable deserializedClass: WrappedElement = null

        member this.Class
            with get () =
                if isSerializing then
                    if element.Class = element then
                        null
                    else
                        wrap(Some element.Class)
                else
                    deserializedClass
            and set source =
                deserializedClass <- source

    /// Helper class for edge serialization.
    [<JsonObject>]
    type WrappedEdge(isSerializing: bool, edge: IEdge, wrap: IElement option -> WrappedElement) =
        inherit WrappedElement(isSerializing, edge, wrap)
        let mutable deserializedSource: WrappedElement = null
        let mutable deserializedTarget: WrappedElement = null

        member this.Source
            with get () =
                if isSerializing then
                    wrap(edge.Source)
                else
                    deserializedSource
            and set source =
                deserializedSource <- source

        member this.Target
            with get () =
                if isSerializing then
                    wrap(edge.Target)
                else
                    deserializedTarget
            and set target =
                deserializedTarget <- target

    /// Helper class for association serialization.
    [<JsonObject>]
    type WrappedAssociation(isSerializing: bool, association: IAssociation, wrap: IElement option -> WrappedElement) =
        inherit WrappedEdge(isSerializing, association, wrap)
        let mutable deserializedTargetName = ""
        member this.TargetName 
            with get () =
                if isSerializing then
                    association.TargetName
                else
                    deserializedTargetName
            and set name =
                deserializedTargetName <- name

    /// Helper class for generalization serialization.
    [<JsonObject>]
    type WrappedGeneralization(isSerializing: bool, generalization: IGeneralization, wrap: IElement option -> WrappedElement) =
        inherit WrappedEdge(isSerializing, generalization, wrap)

    /// Helper class for node serialization.
    [<JsonObject>]
    type WrappedNode(isSerializing: bool, node: INode, wrap: IElement option -> WrappedElement) =
        inherit WrappedElement(isSerializing, node, wrap)
        let mutable deserializedName = ""
        member this.Name 
            with get () =
                if isSerializing then
                    node.Name
                else
                    deserializedName
            and set name =
                deserializedName <- name

    /// Repository of objects to be serialized. Needs to be maintained for correct reference serialization.
    let private wrappedElements = Dictionary<IElement, WrappedElement>()

    /// Function that wraps element from repository to its corresponding wrapper object for serialization.
    /// Maintains a repository of wrapped objects, so if this object was seen already, does not create a new object.
    let rec private wrap (element: IElement option) =
        let register key value =
            wrappedElements.Add(key, value)
            value
        match element with
        | None -> null
        | Some e -> 
            if wrappedElements.ContainsKey e then
                wrappedElements.[e]
            else
                match e with
                | :? INode as n -> 
                    register n (WrappedNode(true, n, wrap) :> WrappedElement)
                | :? IAssociation as a -> 
                    register a (WrappedAssociation(true, a, wrap) :> WrappedElement)
                | :? IGeneralization as g -> 
                    register g (WrappedGeneralization(true, g, wrap) :> WrappedElement)
                | _ -> failwith "Unknown element type in data repo, can not serialize"

    /// Repository of already deserialized objects. Needs to be maintained for correct reference deserialization.
    let private unwrappedElements = Dictionary<WrappedElement, IElement>()

    /// Function that creates actual object in repository from the data saved in serialized wrapper object.
    /// Maintains a repository of unwrapped objects, so if this object was seen already, does not create a new object.
    let rec private unwrap (element: WrappedElement) (model: IModel) =
        let register key value =
            unwrappedElements.Add(key, value)
            value
        if unwrappedElements.ContainsKey element then
            unwrappedElements.[element]
        else
            let (!) e = unwrap e model
            let (!!) e = if e = null then None else Some(unwrap e model)
            match element with
            | :? WrappedNode as n -> 
                if n.Class = null then
                    register n (model.CreateNode(n.Name, None)) :> IElement
                else
                    register n (model.CreateNode(n.Name, !n.Class)) :> IElement
            | :? WrappedAssociation as a -> 
                register a (model.CreateAssociation(!a.Class, !!a.Source, !!a.Target, a.TargetName) :> IElement)
            | :? WrappedGeneralization as g -> 
                register g (model.CreateGeneralization(!g.Class, !!g.Source, !!g.Target) :> IElement)
            | _ -> failwith "Unknown element type in serialized file, can not deserialize"

    /// Helper class for model serialization.
    /// Note the "rev" calls in implementation. JSON serialization library does not topologically sort serialized
    /// objects, so we must structure them in a right order --- from first added to model to last.
    /// Luckily, now models are specified by hand in F# code, so newer elements refer to older elements, and they
    /// are added to a model in reverse order (because adding something to head of a list is much faster than to 
    /// a tail).
    [<JsonObject>]
    type SerializedModel(isSerializing: bool, model: IModel) =
        let wrap element = element :> IElement |> Some |> wrap
        let mutable deserializedNodes: WrappedElement [] = [||]
        let mutable deserializedGeneralizations: WrappedElement [] = [||]
        let mutable deserializedAssociations: WrappedElement [] = [||]
        let mutable deserializedName = ""
        let mutable deserializedMetamodelName = ""

        member this.Name 
            with get () =
                if isSerializing then
                    model.Name
                else
                    deserializedName
            and set name =
                deserializedName <- name

        member this.MetamodelName
            with get () =
                if isSerializing then
                    model.Metamodel.Name
                else
                    deserializedMetamodelName
            and set name =
                deserializedMetamodelName <- name

        member this.Nodes
            with get () =
                if isSerializing then
                    model.Nodes |> Seq.rev |> Seq.map wrap |> Seq.toArray
                else
                    deserializedNodes
            and set nodes =
                deserializedNodes <- nodes

        member this.Associations 
            with get () =
                if isSerializing then
                    let associations (e: IEdge) =
                        match e with
                        | :? IAssociation as a -> Some a
                        | _ -> None
                    model.Edges |> Seq.rev |> Seq.choose associations |> Seq.map wrap |> Seq.toArray
                else
                    deserializedAssociations
            and set associations = 
                deserializedAssociations <- associations

        member this.Generalizations
            with get () =
                if isSerializing then
                    let generalizations (e: IEdge) =
                        match e with
                        | :? IGeneralization as g -> Some g
                        | _ -> None
                    model.Edges |> Seq.rev |> Seq.choose generalizations |> Seq.map wrap |> Seq.toArray
                else 
                    deserializedGeneralizations 
            and set generalizations = 
                deserializedGeneralizations <- generalizations

        member this.Populate (model: IModel) =
            for node in this.Nodes do
                unwrap node model |> ignore
            for association in this.Associations do
                unwrap association model |> ignore
            for generalization in this.Generalizations do
                unwrap generalization model |> ignore

    /// Helper class for repository serialization.
    [<JsonObject>]
    type SerializedRepo(isSerializing: bool, repo: IRepo) =
        
        let mutable deserializedModels: SerializedModel [] = [||]
        
        member this.Models 
            with get () =
                if isSerializing then
                    let serializeModel m = SerializedModel(true, m)
                    repo.Models |> Seq.rev |> Seq.map serializeModel |> Seq.toArray
                else
                    deserializedModels
            and set models =
                deserializedModels <- models

        member this.Populate (repo: IRepo) =
            for model in this.Models do
                let metamodel = repo.Models |> Seq.tryFind (fun m -> m.Name = model.MetamodelName)
                let restoredModel = 
                    match metamodel with
                    | None -> repo.CreateModel model.Name
                    | Some m -> repo.CreateModel(model.Name, m)
                model.Populate restoredModel

    /// Helper class that represents the entire save file.
    [<JsonObject>]
    type Save(isSerializing: bool, repo: IRepo) =
        member val Version = "0.1" with get, set
        member val Extensions = Dictionary<string, obj>() with get, set
        member val Contents = SerializedRepo(isSerializing, repo) with get, set
        member this.Populate (repo: IRepo) =
            this.Contents.Populate repo

    /// Saves given repository contents into a given file name.
    let save fileName repo =
        let save = Save(true, repo)
        let serializedRepo = 
            JsonConvert.SerializeObject(
                save, 
                Formatting.Indented, 
                JsonSerializerSettings(
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                    ))
        if useCompression then
            use saveFileStream = File.Create fileName
            use compressionStream = new GZipStream(saveFileStream, CompressionMode.Compress)
            use writer = new StreamWriter(compressionStream)
            writer.Write serializedRepo
        else
            File.WriteAllText(fileName, serializedRepo)
        wrappedElements.Clear()

    /// Loads given file contents into a given repository.
    let load fileName (repo: IRepo) =
        let serializedContents = 
            if useCompression then
                use saveFileStream = File.Open(fileName, FileMode.Open)
                use compressionStream = new GZipStream(saveFileStream, CompressionMode.Decompress)
                use reader = new StreamReader(compressionStream)
                reader.ReadToEnd()
            else
                File.ReadAllText fileName
        let deserializedRepo = 
            JsonConvert.DeserializeObject<Save>(
                serializedContents,
                JsonSerializerSettings(
                        TypeNameHandling = TypeNameHandling.Auto
                        ))
        repo.Clear()
        deserializedRepo.Populate repo
        unwrappedElements.Clear()
