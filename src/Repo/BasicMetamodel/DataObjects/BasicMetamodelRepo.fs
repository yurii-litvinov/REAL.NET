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

namespace Repo.BasicMetamodel.DataObjects

open Repo.BasicMetamodel

/// Implementation of basic repository (who doesn't even know about models).
type BasicMetamodelRepo(name: string) =
    inherit BasicMetamodelElement()

    let mutable nodes: IBasicMetamodelNode list = []
    let mutable edges: IBasicMetamodelEdge list = []

    interface IBasicMetamodelRepository with
        member this.CreateNode (name: string): IBasicMetamodelNode =
            let node = BasicMetamodelNode(name) :> IBasicMetamodelNode
            nodes <- node :: nodes
            node

        member this.CreateEdge 
                (source: IBasicMetamodelElement)
                (target: IBasicMetamodelElement)
                (targetName: string)
                 =
            let edge = BasicMetamodelEdge(source, target, targetName) :> IBasicMetamodelEdge
            edges <- edge :: edges
            edge

        member this.Elements: IBasicMetamodelElement seq =
            let nodes = nodes |> Seq.cast<IBasicMetamodelElement>
            let edges = edges |> Seq.cast<IBasicMetamodelElement>
            Seq.append nodes edges

        member this.Nodes: IBasicMetamodelNode seq =
            nodes |> Seq.ofList

        member this.Edges: IBasicMetamodelEdge seq =
            edges |> Seq.ofList

        member this.DeleteElement (element: IBasicMetamodelElement) =
            match element with
            | :? IBasicMetamodelNode as n ->
                nodes <- nodes |> List.except [n]

                let deleteEdge e = (this :> IBasicMetamodelRepository).DeleteElement e

                n.OutgoingEdges |> Seq.iter deleteEdge
                edges |> List.filter (fun e -> e.Target = (n :> IBasicMetamodelElement)) |> Seq.iter deleteEdge
            
            | :? IBasicMetamodelEdge as e -> 
                edges <- edges |> List.except [e]
                (e.Source :?> BasicMetamodelElement).UnregisterOutgoingEdge e

            | _ -> failwith "Unknown descendant of IBasicMetamodelElement"


        member this.Node (name: string): IBasicMetamodelNode =
            let filtered = nodes |> List.filter (fun x -> x.Name = name)
            match filtered with
            | [] -> raise (Repo.ElementNotFoundException name)
            | _::_::_ -> raise (Repo.MultipleElementsException name)
            | [x] -> x

        member this.HasNode (name: string): bool =
            nodes |> List.exists (fun x -> x.Name = name)

        member this.Edge (name: string): IBasicMetamodelEdge =
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
