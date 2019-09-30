//(* Copyright 2019 Yurii Litvinov
//*
//* Licensed under the Apache License, Version 2.0 (the "License");
//* you may not use this file except in compliance with the License.
//* You may obtain a copy of the License at
//*
//*     http://www.apache.org/licenses/LICENSE-2.0
//*
//* Unless required by applicable law or agreed to in writing, software
//* distributed under the License is distributed on an "AS IS" BASIS,
//* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//* See the License for the specific language governing permissions and
//* limitations under the License. *)

//namespace Repo.InfrastructureMetamodel.Semantics

//open Repo
//open Repo.DataLayer

///// Helper functions for working with models.
//type ModelSemantics (metamodel: IDataModel) =
//   inherit CoreMetamodel.ModelSemantics ()

//   let infrastructureMetamodel = InfrastructureMetamodelHelper(metamodel)
//   let elementSemantics = ElementSemantics(metamodel)

//   /// Prints model contents on a console.
//   member this.PrintContents (model: IDataModel) =
//       printfn "%s (ontological metamodel: %s, linguistic metamodel: %s):" 
//           model.Name
//           model.OntologicalMetamodel.Name
//           model.LinguisticMetamodel.Name
      
//       printfn "Nodes:"
//       model.Nodes
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttribute n)
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsSlot n)
//           |> Seq.map (fun n -> elementSemantics.ToString n)
//           |> Seq.iter (printfn "%s\n")
//       printfn ""

//       printfn "Edges:"
//       model.Edges 
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttributeAssociation n)
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttributeTypeAssociation n)
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsAttributeDefaultValueAssociation n)
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsSlotAssociation n)
//           |> Seq.filter (fun n -> not <| infrastructureMetamodel.IsSlotValueAssociation n)
//           |> Seq.map (fun e -> e.ToString())
//           |> Seq.iter (printfn "    %s")