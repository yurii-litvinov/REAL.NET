//(* Copyright 2018 Yurii Litvinov
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

//namespace Repo.Serializer

//open System.Collections.Generic
//open System.IO
//open System.IO.Compression
//open Newtonsoft.Json
//open Repo.DataLayer

///// Deserializes repository contents from a file.
//module Deserializer =

//    /// Repository of already deserialized objects. Needs to be maintained for correct reference deserialization.
//    let private unwrappedElements = Dictionary<WrappedElement, IDataElement>()

//    /// Function that creates actual object in repository from the data saved in serialized wrapper object.
//    /// Maintains a repository of unwrapped objects, so if this object was seen already, does not create a new object.
//    let rec private unwrap (element: WrappedElement) (model: IDataModel) =
//        let register key value =
//            unwrappedElements.Add(key, value)
//            value
//        if unwrappedElements.ContainsKey element then
//            unwrappedElements.[element]
//        else
//            let (!) e = unwrap e model
//            match element with
//            | :? WrappedNode as n -> 
//                register n (model.CreateNode n.Name) :> IDataElement
//            | :? WrappedAssociation as a -> 
//                model.CreateAssociation
//                        (
//                        !a.Source, 
//                        !a.Target, 
//                        a.TargetName
//                        ) :> IDataElement
//                |> register a
//            | :? WrappedGeneralization as g -> 
//                model.CreateGeneralization
//                        (
//                        !g.Source, 
//                        !g.Target
//                        ) :> IDataElement
//                |> register g
//            | _ -> failwith "Unknown element type in serialized file, can not deserialize"

//    /// Visitor that takes care of adding new elements and models to a repo, assuming that they are provided in 
//    /// correct order.
//    type private DeserializingVisitor(repo: IDataRepository) =

//        [<DefaultValue>]
//        val mutable currentModel: IDataModel

//        interface Visitor with
//            member this.Visit (association: WrappedAssociation) =
//                unwrap association this.currentModel |> ignore

//            member this.Visit (generalization: WrappedGeneralization) =
//                unwrap generalization this.currentModel |> ignore

//            member this.Visit (node: WrappedNode) =
//                unwrap node this.currentModel |> ignore

//            member this.Visit (model: WrappedModel) =
//                let ontologicalMetamodel = 
//                    repo.Models |> Seq.tryFind (fun m -> m.Name = model.OntologicalMetamodelName)
//                let linguisticMetamodel = 
//                    repo.Models |> Seq.tryFind (fun m -> m.Name = model.LinguisticMetamodelName)

//                this.currentModel <- 
//                    match ontologicalMetamodel, linguisticMetamodel with
//                    | None, None -> repo.CreateModel model.Name
//                    | Some om, Some lm -> repo.CreateModel(model.Name, om, lm)
//                    | _ -> failwith "Ontological and linguistic metamodels should be both set or both unset"

//                this.currentModel.Properties <- Map.ofArray model.Properties

//            member this.Visit (repo: WrappedRepo) =
//                ()

//            member this.Visit (save: SaveFile) =
//                ()

//    /// Loads given file contents into a given repository. Clears repository contents.
//    let load fileName (repo: IDataRepository) =
//        use saveFileStream = File.Open(fileName, FileMode.Open)
//        use saveFileStream = 
//            if Serializer.useCompression then
//                new GZipStream(saveFileStream, CompressionMode.Decompress) :> Stream
//            else
//                saveFileStream :> Stream
//        use reader = new JsonTextReader(new StreamReader(saveFileStream))
//        let serializer = new JsonSerializer();
//        let deserializedRepo = serializer.Deserialize<SaveFile>(reader)

//        repo.Clear()
//        let visitor = DeserializingVisitor(repo)
//        deserializedRepo.Accept visitor
//        unwrappedElements.Clear()
