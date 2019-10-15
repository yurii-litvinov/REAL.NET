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

namespace Repo.Facade

open Repo
open Repo.InfrastructureMetamodel

/// Implementation of edge wrapper.
type Edge(element: IInfrastructureElement, pool: FacadePool, repo: IInfrastructureRepository) =


    inherit Element(element, pool, repo)

//    let mutable visualInfo = info

//    new(infrastructure, element, elementRepository, attributeRepository) 
//        = Edge(infrastructure, element, elementRepository, attributeRepository, VisualEdgeInfo()) 

    interface IEdge with
        member this.From
            with get (): IElement =
                failwith "Not implemented"
                //if element.Source.IsNone then
                //    null
                //else
                //    elementRepository.GetElement element.Source.Value
            and set (v: IElement): unit =
                failwith "Not implemented"
                //let dataElement = (v :?> Element).UnderlyingElement
                //element.Source <- Some dataElement

        member this.To
            with get (): IElement =
                failwith "Not implemented"
                //if element.Target.IsNone then
                //    null
                //else
                //    elementRepository.GetElement element.Target.Value
            and set (v: IElement): unit =
                failwith "Not implemented"
                //let dataElement = (v :?> Element).UnderlyingElement
                //element.Target <- Some dataElement
        
        member this.VisualInfo
            with get () = 
                failwith "Not implemented"
                //visualInfo
            and set (info) = 
                failwith "Not implemented"
                //visualInfo <- info
