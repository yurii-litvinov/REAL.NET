//(* Copyright 2017-2018 REAL.NET group
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

//namespace Repo.Metamodels

//open Repo.DataLayer
//open Repo.CoreMetamodel
//open Repo.InfrastructureMetamodel

//(*
///// Initializes repository with test model conforming to AirSim Metamodel, actual program that can be written by end-user.
//type AirSimModelBuilder() =
//    interface IModelBuilder with
//        member this.Build(repo: IDataRepository): unit =
//            let infrastructure = InfrastructureSemantic(repo)
//            let metamodel = repo.Model "AirSimMetamodel"
//            let infrastructureMetamodel = infrastructure.Metamodel.Model

//            let metamodelAbstractNode = metamodel.Node "AbstractNode"
//            let metamodelInitialNode = metamodel.Node "InitialNode"
//            let metamodelFinalNode = metamodel.Node "FinalNode"
//            let metamodelTakeoff = metamodel.Node "Takeoff"
//            let metamodelLand = metamodel.Node "Land"
//            let metamodelMove = metamodel.Node "Move"
//            let metamodelTimer = metamodel.Node "Timer"
//            let metamodelIf = metamodel.Node "IfNode"

//            let link = ModelSemantics.FindAssociationWithSource metamodelAbstractNode "target"
//            let ifLink = ModelSemantics.FindAssociationWithSource metamodelAbstractNode "ifTarget"

//            let model = repo.CreateModel("AirSimModel", metamodel)

//            let initialNode = infrastructure.Instantiate model metamodelInitialNode
//            let finalNode = infrastructure.Instantiate model metamodelFinalNode
//            let finalNode2 = infrastructure.Instantiate model metamodelFinalNode

//            let takeoff = infrastructure.Instantiate model metamodelTakeoff
            
//            let landing = infrastructure.Instantiate model metamodelLand
//            let move = infrastructure.Instantiate model metamodelMove

//            let timer1 = infrastructure.Instantiate model metamodelTimer
//            let timer2 = infrastructure.Instantiate model metamodelTimer
//            let timer3 = infrastructure.Instantiate model metamodelTimer
//            infrastructure.Element.SetAttributeValue timer1 "delay" "1"
//            infrastructure.Element.SetAttributeValue timer2 "delay" "1"
//            infrastructure.Element.SetAttributeValue timer3 "delay" "1"
            
//            let ifNode = infrastructure.Instantiate model metamodelIf
            
//            let find name = infrastructureMetamodel.Node name

//            // The same as in the metamodel but with functions
//            let (~+) (name, shape, isAbstract) =
//                let node = infrastructure.Instantiate metamodel (find "Node") :?> IDataNode
//                node.Name <- name
//                infrastructure.Element.SetAttributeValue node "shape" shape
//                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
//                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

//                node

//            let (-->) (src: IDataElement) dst =
//                let aLink = infrastructure.Instantiate model link :?> IDataAssociation
//                aLink.Source <- Some src
//                aLink.Target <- Some dst
//                dst
            
//            let (-->>) (src: IDataElement) dst =
//                let aLink = infrastructure.Instantiate model ifLink :?> IDataAssociation
//                infrastructure.Element.SetAttributeValue aLink "Value" "true"
//                aLink.Source <- Some src
//                aLink.Target <- Some dst
//                dst
            
//            let (-->>>) (src: IDataElement) dst =
//                let aLink = infrastructure.Instantiate model ifLink :?> IDataAssociation
//                infrastructure.Element.SetAttributeValue aLink "Value" "false"
//                aLink.Source <- Some src
//                aLink.Target <- Some dst
//                dst
            
//            // Make some function
//            let newFunc = metamodelInitialNode --> metamodelTakeoff --> metamodelFinalNode :?> IDataNode
            
//            // Get the start node of function
//            let rec getStart (el: IDataNode) = 
//                match Seq.toList el.IncomingEdges with
//                | [] -> el
//                | head::_ -> 
//                    match head.Source with
//                    | None -> el
//                    | Some s -> getStart(s :?> IDataNode)
            
//            // Add new node with function to metamodel
//            +("FuncionNode", "View/Pictures/functionBlock.png", false) |> ignore
            
//            initialNode --> ifNode-->> takeoff --> move --> timer3 --> landing --> finalNode |> ignore
//            ifNode -->>> finalNode2 |> ignore
//            ()
//*)