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
type DataModel private (name: string, metamodel: IDataModel option) =

    let mutable nodes = []
    let mutable edges = []
    
    let getElements() =
                    let castNode (node: IDataNode) = node :> IDataElement
                    let castEdge (edge: IDataEdge) = edge :> IDataElement
                    (List.map castNode nodes) @ (List.map castEdge edges)

    let mutable properties = Map.empty

    new(name: string) = DataModel(name, None)
    new(name: string, metamodel: IDataModel) = DataModel(name, Some metamodel)

    interface IDataModel with
        
        member this.CreateNode(name, (func: Option<IDataElement>)) =
            let node = DataNode(name, func, this) :> IDataNode
            nodes <- node :: nodes
            node

        member this.CreateNode(name, (``class``:IDataElement)) =
            let node = DataNode(name, ``class``, this) :> IDataNode
            nodes <- node :: nodes
            node

        member this.CreateAssociation(``class``, source, target, targetName) =
            let edge = new DataAssociation(``class``, source, target, targetName, this) :> IAssociation
            edges <- (edge :> IDataEdge) :: edges
            if source.IsSome then
                source.Value.AddOutgoingEdge edge
            if target.IsSome then
                target.Value.AddIncomingEdge edge
            edge

        member this.CreateAssociation(``class``, source, target, targetName) =
            let edge = new DataAssociation(``class``, Some source, Some target, targetName, this) :> IAssociation
            edges <- (edge :> IDataEdge) :: edges
            source.AddOutgoingEdge edge
            target.AddIncomingEdge edge
            edge

        member this.CreateGeneralization(``class``, source, target) =
            let edge = new DataGeneralization(``class``, source, target, this) :> IGeneralization
            if source.IsSome then
                source.Value.AddOutgoingEdge edge
            if target.IsSome then
                target.Value.AddIncomingEdge edge
            edges <- (edge :> IDataEdge) :: edges
            edge

        member this.CreateGeneralization(``class``, source, target) =
            let edge = new DataGeneralization(``class``, Some source, Some target, this) :> IGeneralization
            source.AddOutgoingEdge edge
            target.AddIncomingEdge edge
            edges <- (edge :> IDataEdge) :: edges
            edge

        member this.RemoveElement(element: IDataElement): unit =
            let delete (element: IDataElement) =
                match element with
                | :? IDataNode  ->
                    let node = element :?> IDataNode
                    if (List.contains node nodes) then
                        nodes <- List.except [node] nodes
                    else invalidOp "Model does not contain this element"
                | :? IDataEdge  ->
                    let edge = element :?> IDataEdge
                    if (List.contains edge edges) then
                        match edge.Source with
                        | Some source -> source.DeleteOutgoingEdge edge
                        | _ -> ()
                        match edge.Target with
                        | Some source -> source.DeleteIncomingEdge edge
                        | _ -> ()
                        edges <- List.except [edge] edges
                    else invalidOp "Model does not contain this element"
                | _ -> invalidOp "Unknown type of element"
            delete element
        
        member this.Metamodel
            with get(): IDataModel =
                match metamodel with
                | Some v -> v
                | None -> this :> IDataModel

        member val Name = name with get, set

        member this.Nodes: seq<IDataNode> = nodes |> Seq.ofList

        member this.Edges: seq<IDataEdge> = edges |> Seq.ofList
            
         member this.Elements: IDataElement seq =
            let nodes = ((this :> IDataModel).Nodes |> Seq.cast<IDataElement>)
            let edges = ((this :> IDataModel).Edges |> Seq.cast<IDataElement>)
            Seq.append nodes edges

        member this.Properties
            with get () = properties
            and set v = properties <- v
            
