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

namespace Repo.CoreMetamodel.Details.Elements

open Repo
open Repo.CoreMetamodel
open Repo.BasicMetamodel

/// Implementation of Element.
[<AbstractClass>]
type CoreElement(element: IBasicElement, pool: CorePool, repo: IBasicRepository) =

    let isInstanceOf metatype (element: IBasicElement) =
        element.Metatype = metatype

    let hasOutgoingEdgeTo (target: IBasicElement) element =
        target.OutgoingEdges 
        |> Seq.filter (fun e -> e.Target = element)
        |> Seq.isEmpty
        |> not

    /// Returns underlying BasicElement.
    member this.UnderlyingElement = element

    interface ICoreElement with
        member this.OutgoingEdges =
            element.OutgoingEdges
            |> Seq.map pool.Wrap
            |> Seq.cast<ICoreEdge>

        member this.OutgoingAssociations =
            (this :> ICoreElement).OutgoingEdges
            |> Seq.filter (fun e -> e :? ICoreAssociation)
            |> Seq.cast<ICoreAssociation>

        member this.OutgoingAssociation name =
            (this :> ICoreElement).OutgoingAssociations
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.IncomingEdges =
            repo.Edges 
            |> Seq.filter (fun e -> e.Target = element)
            |> Seq.map pool.Wrap
            |> Seq.cast<ICoreEdge>

        member this.IncomingAssociations =
            (this :> ICoreElement).IncomingEdges
            |> Seq.filter (fun e -> e :? ICoreAssociation)
            |> Seq.cast<ICoreAssociation>

        member this.IncomingAssociation name =
            (this :> ICoreElement).IncomingAssociations
            |> Seq.filter (fun e -> e.TargetName = name)
            |> Helpers.exactlyOneElement name

        member this.IsContainedInSomeModel =
            let modelMetatype = repo.Node Consts.metamodelModel

            repo.Nodes
            |> Seq.filter (isInstanceOf modelMetatype)
            |> Seq.filter (hasOutgoingEdgeTo element)
            |> Seq.isEmpty
            |> not

        member this.Generalizations =
            let outgoingEdges = (this :> ICoreElement).OutgoingEdges

            let directGeneralizations = 
                (this :> ICoreElement).OutgoingEdges
                |> Seq.filter (fun e -> e :? ICoreGeneralization)
                |> Seq.map (fun e -> e.Target)

            let parentGeneralizations = directGeneralizations |> Seq.map (fun e -> e.Generalizations)
            Seq.append directGeneralizations (Seq.concat parentGeneralizations)

        member this.Model: ICoreModel =
            let modelMetametatype = repo.Node Consts.metamodelModel
            let modelMetatype = repo.Node Consts.model

            repo.Nodes
            |> Seq.filter (fun n -> isInstanceOf modelMetametatype n || isInstanceOf modelMetatype n)
            |> Seq.filter (hasOutgoingEdgeTo element)
            |> Seq.exactlyOne
            |> pool.WrapModel

        member this.Metatypes =
            element.Metatypes
            |> Seq.map pool.Wrap

        member this.Metatype =
            pool.Wrap element.Metatype

        member this.IsInstanceOf element =
            let this = this :> ICoreElement
            if this.Metatype = element then
                true
            elif this.Metatype.Generalizations |> Seq.contains element then
                true
            elif this.Metatype = this then 
                false
            else
                this.Metatype.IsInstanceOf element
                || this.Metatype.Generalizations |> Seq.exists (fun e -> e.IsInstanceOf element)
