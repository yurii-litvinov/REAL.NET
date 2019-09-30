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

//namespace Repo.LanguageMetamodel.Semantics

//open Repo
//open Repo.DataLayer

///// Helper functions for element semantics.
//type ElementSemantics (metamodel: IDataModel) =
//    inherit AttributeMetamodel.Semantics.ElementSemantics(metamodel)

///// Helper functions for enum semantics.
//type EnumSemantics () =
//    static member Values (enum: IDataNode) =
//        CoreMetamodel.ElementSemantics.OutgoingAssociationsWithTargetName enum "elements"
//        |> Seq.map (fun a -> a.Target.Value :?> IDataNode)

//    /// Returns node corresponding to enum element with given name.
//    static member EnumElementNode (enum: IDataNode) name =
//        EnumSemantics.Values enum
//        |> Seq.filter (fun n -> n.Name = name)
//        |> Seq.exactlyOne

///// Helper functions for working with models.
//type ModelSemantics (metamodel: IDataModel) =
//    inherit AttributeMetamodel.Semantics.ModelSemantics(metamodel)

///// Helper class that provides semantic operations on models conforming to Language Metamodel.
//type InstantiationSemantics(metamodel: IDataModel) =
//    inherit AttributeMetamodel.Semantics.InstantiationSemantics(metamodel)

