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

namespace Repo.DataLayer

open System
open System.ComponentModel

/// Implementation of model interface in data layer. Contains nodes and edges in list, implements
/// CRUD operations and keeps consistency.
type DataModel private (name: string, metamodel: IModel option) =

    let mutable nodes = []
    let mutable edges = []

    let mutable properties = Map.empty

    new(name: string) = DataModel(name, None)
    new(name: string, metamodel: IModel) = DataModel(name, Some metamodel)

    interface IModel with
        member this.CreateNode(name, (func: Option<IElement>)) =
            let node = DataNode(name, func, this) :> INode
            nodes <- node :: nodes
            node

        member this.CreateNode(name, (``class``:IElement)) =
            let node = DataNode(name, ``class``, this) :> INode
            nodes <- node :: nodes
            node

        member this.CreateAssociation(``class``, source, target, targetName) =
            let edge = new DataAssociation(``class``, source, target, targetName, this) :> IAssociation
            edges <- (edge :> IEdge) :: edges
            if source.IsSome then
                source.Value.AddOutgoingEdge edge
            if target.IsSome then
                target.Value.AddIncomingEdge edge
            edge

        member this.CreateAssociation(``class``, source, target, targetName) =
            let edge = new DataAssociation(``class``, Some source, Some target, targetName, this) :> IAssociation
            edges <- (edge :> IEdge) :: edges
            source.AddOutgoingEdge edge
            target.AddIncomingEdge edge
            edge

        member this.CreateGeneralization(``class``, source, target) =
            let edge = new DataGeneralization(``class``, source, target, this) :> IGeneralization
            if source.IsSome then
                source.Value.AddOutgoingEdge edge
            if target.IsSome then
                target.Value.AddIncomingEdge edge
            edges <- (edge :> IEdge) :: edges
            edge

        member this.CreateGeneralization(``class``, source, target) =
            let edge = new DataGeneralization(``class``, Some source, Some target, this) :> IGeneralization
            source.AddOutgoingEdge edge
            target.AddIncomingEdge edge
            edges <- (edge :> IEdge) :: edges
            edge

        member this.MarkElementDeleted(element: IElement): unit =           
            let mark (element: IElement) = 
                match element with
                | :? INode -> let elementToDelete = nodes |> List.find (fun x -> x = (element :?> INode))
                              elementToDelete.IsMarkedDeleted <- true
                | _ ->        let elementToDelete = edges |> List.find (fun x -> x = (element :?> IEdge))
                              elementToDelete.IsMarkedDeleted <- true                                                            
            mark element
        
        member this.UnmarkDeletedElement(element: IElement): unit =
             match element with
                | :? INode -> let elementToRestore = nodes |> List.find (fun x -> x = (element :?> INode))
                              elementToRestore.IsMarkedDeleted <- false
                | _ ->        let elementToRestore = edges |> List.find (fun x -> x = (element :?> IEdge))
                              elementToRestore.IsMarkedDeleted <- false 

        member this.Elements: IElement seq =
            let nodes = (nodes |> List.filter (fun node -> not node.IsMarkedDeleted) |> Seq.cast<IElement>)
            let edges = (edges |> List.filter (fun edge -> not edge.IsMarkedDeleted) |> Seq.cast<IElement>)
            Seq.append nodes edges

        member this.Metamodel
            with get(): IModel =
                match metamodel with
                | Some v -> v
                | None -> this :> IModel

        member val Name = name with get, set

        member this.Nodes: seq<INode> =
            nodes |> List.filter (fun node -> not node.IsMarkedDeleted) |> Seq.ofList

        member this.Edges: seq<IEdge> =
            edges |> List.filter (fun edge -> not edge.IsMarkedDeleted) |> Seq.ofList

        member this.Properties
            with get () = properties
            and set v = properties <- v
            
