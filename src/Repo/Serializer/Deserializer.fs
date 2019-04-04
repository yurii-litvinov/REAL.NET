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

open System.Collections.Generic
open System.IO
open System.IO.Compression
open Newtonsoft.Json
open Repo.DataLayer

/// Deserializes repository contents from a file.
module Deserializer =

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

    /// Visitor that takes care of adding new elements and models to a repo, assuming that they are provided in 
    /// correct order.
    type private DeserializingVisitor(repo: IRepo) =

        [<DefaultValue>]
        val mutable currentModel: IModel

        interface Visitor with
            member this.Visit (association: WrappedAssociation) =
                unwrap association this.currentModel |> ignore

            member this.Visit (generalization: WrappedGeneralization) =
                unwrap generalization this.currentModel |> ignore

            member this.Visit (node: WrappedNode) =
                unwrap node this.currentModel |> ignore

            member this.Visit (model: WrappedModel) =
                let metamodel = repo.Models |> Seq.tryFind (fun m -> m.Name = model.MetamodelName)
                this.currentModel <- 
                    match metamodel with
                    | None -> repo.CreateModel model.Name
                    | Some m -> repo.CreateModel(model.Name, m)
                this.currentModel.Properties <- Map.ofArray model.Properties

            member this.Visit (repo: WrappedRepo) =
                ()

            member this.Visit (save: Save) =
                ()

    /// Loads given file contents into a given repository. Clears repository contents.
    let load fileName (repo: IRepo) =
        use saveFileStream = File.Open(fileName, FileMode.Open)
        use saveFileStream = 
            if Serializer.useCompression then
                new GZipStream(saveFileStream, CompressionMode.Decompress) :> Stream
            else
                saveFileStream :> Stream
        use reader = new JsonTextReader(new StreamReader(saveFileStream))
        let serializer = new JsonSerializer();
        let deserializedRepo = serializer.Deserialize<Save>(reader)

        repo.Clear()
        let visitor = DeserializingVisitor(repo)
        deserializedRepo.Accept visitor
        unwrappedElements.Clear()
