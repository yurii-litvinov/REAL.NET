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
    type WrappedElement(element: IElement, wrap: IElement option -> WrappedElement) =
        member this.Class = 
            if element.Class = element then
                null
            else
                wrap(Some element.Class)

    /// Helper class for edge serialization.
    [<JsonObject>]
    type WrappedEdge(edge: IEdge, wrap: IElement option -> WrappedElement) =
        inherit WrappedElement(edge, wrap)
        member this.Source = wrap(edge.Source)
        member this.Target = wrap(edge.Target)

    /// Helper class for association serialization.
    [<JsonObject>]
    type WrappedAssociation(association: IAssociation, wrap: IElement option -> WrappedElement) =
        inherit WrappedEdge(association, wrap)
        member this.TargetName = association.TargetName

    /// Helper class for generalization serialization.
    [<JsonObject>]
    type WrappedGeneralization(generalization: IGeneralization, wrap: IElement option -> WrappedElement) =
        inherit WrappedEdge(generalization, wrap)

    /// Helper class for node serialization.
    [<JsonObject>]
    type WrappedNode(node: INode, wrap: IElement option -> WrappedElement) =
        inherit WrappedElement(node, wrap)
        member this.Name = node.Name

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
                    register n (WrappedNode(n, wrap) :> WrappedElement)
                | :? IAssociation as a -> 
                    register a (WrappedAssociation(a, wrap) :> WrappedElement)
                | :? IGeneralization as g -> 
                    register g (WrappedGeneralization(g, wrap) :> WrappedElement)
                | _ -> failwith "Unknown element type in data repo, can not serialize"

    /// Helper class for model serialization.
    /// Note the "rev" calls in implementation. JSON serialization library does not topologically sort serialized
    /// objects, so we must structure them in a right order --- from first added to model to last.
    /// Luckily, now models are specified by hand in F# code, so newer elements refer to older elements, and they
    /// are added to a model in reverse order (because adding something to head of a list is much faster than to 
    /// a tail).
    [<JsonObject>]
    type SerializedModel(model: IModel) =
        let wrap element = element :> IElement |> Some |> wrap
        member this.Name = model.Name
        member this.MetamodelName = model.Metamodel.Name
        member this.Nodes =
            model.Nodes |> Seq.rev |> Seq.map wrap |> Seq.toArray
        member this.Associations =
            let associations (e: IEdge) =
                match e with
                | :? IAssociation as a -> Some a
                | _ -> None
            model.Edges |> Seq.rev |> Seq.choose associations |> Seq.map wrap |> Seq.toArray
        member this.Generalizations =
            let generalizations (e: IEdge) =
                match e with
                | :? IGeneralization as g -> Some g
                | _ -> None
            model.Edges |> Seq.rev |> Seq.choose generalizations |> Seq.map wrap |> Seq.toArray

    /// Helper class for repository serialization.
    [<JsonObject>]
    type SerializedRepo(repo: IRepo) =
        member this.Models =
            repo.Models |> Seq.rev |> Seq.map SerializedModel |> Seq.toArray

    /// Helper class that represents the entire save file.
    [<JsonObject>]
    type Save(repo: IRepo) =
        member val Version = "0.1" with get
        member val Extensions = Dictionary<string, obj>() with get
        member val Contents = SerializedRepo(repo) with get

    /// Saves given repository contents into a given file name.
    let save fileName repo =
        let save = Save(repo)
        let serializedRepo = 
            JsonConvert.SerializeObject(
                save, 
                Formatting.Indented, 
                JsonSerializerSettings(PreserveReferencesHandling = PreserveReferencesHandling.Objects))
        if useCompression then
            use saveFileStream = File.Create fileName
            use compressionStream = new GZipStream(saveFileStream, CompressionMode.Compress)
            use writer = new StreamWriter(compressionStream)
            writer.Write serializedRepo
        else
            File.WriteAllText(fileName, serializedRepo)

    /// Loads given file contents into a given repository.
    let load fileName repo =
        ()
