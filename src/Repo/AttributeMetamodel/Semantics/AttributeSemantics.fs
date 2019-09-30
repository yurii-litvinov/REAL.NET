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

///// Helper methods for working with attributes.
//type AttributeSemantics () =
//    /// Returns attribute name.
//    static member Name (attribute: IDataNode) =
//        attribute.Name

//    /// Returns a node that represents type of an attribute.
//    static member Type (attribute: IDataNode) =
//        CoreMetamodel.ElementSemantics.OutgoingAssociation attribute "type"
//        |> fun a -> a.Target.Value :?> IDataNode

//    /// Returns node representing default value of an attribute.
//    static member DefaultValue (attribute: IDataNode) =
//        CoreMetamodel.ElementSemantics.OutgoingAssociation attribute "defaultValue" 
//        |> fun a -> a.Target.Value 
//        :?> IDataNode