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

//namespace Repo.AttributeMetamodel.Semantics

//open Repo
//open Repo.DataLayer

///// Helper functions for working with models.
//type ModelSemantics (metamodel: IDataModel) =
//    inherit CoreMetamodel.ModelSemantics ()

//    let nodeSemantics = ElementSemantics(metamodel)

//    /// Prints model contents on a console.
//    member this.PrintContents (model: IDataModel) =
//        printfn "%s (ontological metamodel: %s, linguistic metamodel: %s):" 
//            model.Name
//            model.OntologicalMetamodel.Name
//            model.LinguisticMetamodel.Name
       
//        printfn "Nodes:"
//        model.Nodes
//            |> Seq.map (fun n -> nodeSemantics.ToString n)
//            |> Seq.iter (printfn "%s\n")
//        printfn ""

//        printfn "Edges:"
//        model.Edges 
//            |> Seq.map (fun e -> e.ToString())
//            |> Seq.iter (printfn "    %s")