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

/// Implementation of model interface in data layer. Contains nodes and edges in list, implements
/// CRUD operations and keeps consistency.
type DataModel private (name: string, metamodel: IDataModel option) =

    let mutable nodes = []
    let mutable edges = []

    let mutable properties = Map.empty

    /// Helper function to choose only associatinons from sequence of edges.
    let chooseAssociation: IDataEdge -> IDataAssociation option = 
        function
        | :? IDataAssociation as a -> Some a
        | _ -> None

    new(name: string) = DataModel(name, None)
    new(name: string, metamodel: IDataModel) = DataModel(name, Some metamodel)

    interface IDataModel with
        member this.CreateNode name =
            let node = DataNode(name, this) :> IDataNode
            nodes <- node :: nodes
            node

        member this.CreateNode(name, (``class``:IDataElement)) =
            let node = DataNode(name, ``class``, this) :> IDataNode
            nodes <- node :: nodes
            node

        member this.CreateAssociation(``class``, source, target, targetName) =
            let edge = new DataAssociation(``class``, source, target, targetName, this) :> IDataAssociation
            edges <- (edge :> IDataEdge) :: edges
            if source.IsSome then
                source.Value.AddOutgoingEdge edge
            if target.IsSome then
                target.Value.AddIncomingEdge edge
            edge

        member this.CreateAssociation(``class``, source, target, targetName) =
            let edge = new DataAssociation(``class``, Some source, Some target, targetName, this) :> IDataAssociation
            edges <- (edge :> IDataEdge) :: edges
            source.AddOutgoingEdge edge
            target.AddIncomingEdge edge
            edge

        member this.CreateGeneralization(``class``, source, target) =
            let edge = new DataGeneralization(``class``, source, target, this) :> IDataGeneralization
            if source.IsSome then
                source.Value.AddOutgoingEdge edge
            if target.IsSome then
                target.Value.AddIncomingEdge edge
            edges <- (edge :> IDataEdge) :: edges
            edge

        member this.CreateGeneralization(``class``, source, target) =
            let edge = new DataGeneralization(``class``, Some source, Some target, this) :> IDataGeneralization
            source.AddOutgoingEdge edge
            target.AddIncomingEdge edge
            edges <- (edge :> IDataEdge) :: edges
            edge

        member this.DeleteElement(element: IDataElement): unit =
            match element with
            | :? IDataNode ->
                nodes <- nodes |> List.except [element :?> IDataNode]
            | _ -> edges <- edges |> List.except [element :?> IDataEdge]
            edges |> List.iter (fun e ->
                if e.Source = Some element then e.Source <- None
                if e.Target = Some element then e.Target <- None
                )

        member this.Elements: IDataElement seq =
            let nodes = (nodes |> Seq.cast<IDataElement>)
            let edges = (edges |> Seq.cast<IDataElement>)
            Seq.append nodes edges

        member this.Metamodel
            with get(): IDataModel =
                match metamodel with
                | Some v -> v
                | None -> this :> IDataModel

        member val Name = name with get, set

        member this.Nodes: seq<IDataNode> =
            nodes |> Seq.ofList

        member this.Edges: seq<IDataEdge> =
            edges |> Seq.ofList
        
        member this.Node (name: string): IDataNode = 
            let filtered = nodes |> List.filter (fun x -> x.Name = name)
            match filtered with
            | [] -> raise (Repo.ElementNotFoundException name)
            | _::_::_ -> raise (Repo.MultipleElementsException name)
            | [x] -> x

        member this.HasNode (name: string): bool =
            nodes |> List.exists (fun x -> x.Name = name)

        member this.Properties
            with get () = properties
            and set v = properties <- v

        member this.Association (name: string): IDataAssociation =
            Repo.Helpers.getExactlyOne 
                (edges |> Seq.choose chooseAssociation)
                (fun a -> a.TargetName = name)
                (fun () -> Repo.ElementNotFoundException name)
                (fun () -> Repo.MultipleElementsException name)

        member this.HasAssociation (name: string) =
            edges |> Seq.choose chooseAssociation |> Seq.tryFind (fun a -> a.TargetName = name) |> Option.isSome
