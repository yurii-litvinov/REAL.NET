(* Copyright 2019 REAL.NET group
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

namespace Repo.BasicMetamodel.Details.Elements

open Repo.BasicMetamodel

/// Implementation of basic repository (who doesn't even know about models).
type BasicRepository() =
    let mutable nodes: IBasicNode list = []
    let mutable edges: IBasicEdge list = []

    interface IBasicRepository with
        member this.CreateNode name =
            let node = BasicNode(name) :> IBasicNode
            nodes <- node :: nodes
            node

        member this.CreateEdge source target targetName =
            let edge = BasicEdge(source, target, targetName) :> IBasicEdge
            edges <- edge :: edges
            edge

        member this.Elements: IBasicElement seq =
            let nodes = nodes |> Seq.cast<IBasicElement>
            let edges = edges |> Seq.cast<IBasicElement>
            Seq.append nodes edges

        member this.Nodes = nodes |> Seq.ofList

        member this.Edges = edges |> Seq.ofList

        member this.DeleteElement element =
            match element with
            | :? IBasicNode as n ->
                nodes <- nodes |> List.except [n]

                let deleteEdge e = (this :> IBasicRepository).DeleteElement e

                n.OutgoingEdges |> Seq.iter deleteEdge
                edges |> List.filter (fun e -> e.Target = (n :> IBasicElement)) |> Seq.iter deleteEdge
            
            | :? IBasicEdge as e -> 
                edges <- edges |> List.except [e]
                (e.Source :?> BasicElement).UnregisterOutgoingEdge e

            | _ -> failwith "Unknown descendant of IBasicMetamodelElement"


        member this.Node (name: string): IBasicNode =
            let filtered = nodes |> List.filter (fun x -> x.Name = name)
            match filtered with
            | [] -> raise (Repo.ElementNotFoundException name)
            | _::_::_ -> raise (Repo.MultipleElementsException name)
            | [x] -> x

        member this.HasNode (name: string): bool =
            nodes |> List.exists (fun x -> x.Name = name)

        member this.Edge (name: string): IBasicEdge =
            Repo.Helpers.getExactlyOne 
                edges
                (fun a -> a.TargetName = name)
                (fun () -> Repo.ElementNotFoundException name)
                (fun () -> Repo.MultipleElementsException name)

        member this.HasEdge (name: string): bool =
            edges |> Seq.tryFind (fun a -> a.TargetName = name) |> Option.isSome

        member this.Clear () =
            nodes <- []
            edges <- []
