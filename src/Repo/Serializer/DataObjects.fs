(* Copyright 2018 Yurii Litvinov
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

namespace Repo.Serializer

open Newtonsoft.Json
open System.Collections.Generic

/// Helper class for element serialization. Allows null value, denoting the absense of value in serialized file.
/// Null as a Class is a special value denoting that this element is a class for itself 
/// (so it is CoreMetametamodel Node). All other elements must have actual classes, so can not be null.
[<JsonObject>]
[<AllowNullLiteral>]
[<AbstractClass>]
type WrappedElement() =
    member val Class: WrappedElement = null with get, set
    abstract Accept: visitor: Visitor -> unit

/// Helper class for edge serialization.
and [<JsonObject>] [<AbstractClass>] WrappedEdge() =
    inherit WrappedElement()
    member val Source: WrappedElement = null with get, set
    member val Target: WrappedElement = null with get, set

/// Helper class for association serialization.
and [<JsonObject>] WrappedAssociation() =
    inherit WrappedEdge()
    member val TargetName = "" with get, set
    override this.Accept visitor =
        visitor.Visit this

/// Helper class for generalization serialization.
and [<JsonObject>] WrappedGeneralization() =
    inherit WrappedEdge()
    override this.Accept visitor =
        visitor.Visit this

/// Helper class for node serialization.
and [<JsonObject>] WrappedNode() =
    inherit WrappedElement()
    member val Name = "" with get, set
    override this.Accept visitor =
        visitor.Visit this

/// Helper class for model serialization.
and [<JsonObject>] WrappedModel() =
    member val Name = "" with get, set
    member val MetamodelName = "" with get, set
    member val Properties: (string * string)[] = [||] with get, set
    member val Nodes: WrappedNode[] = [||] with get, set
    member val Associations: WrappedAssociation[] = [||] with get, set
    member val Generalizations: WrappedGeneralization[] = [||] with get, set
    member this.Accept (visitor: Visitor) =
        visitor.Visit this
        for node in this.Nodes do
            node.Accept visitor
        for association in this.Associations do
            association.Accept visitor
        for generalization in this.Generalizations do
            generalization.Accept visitor

/// Helper class for repository serialization.
and [<JsonObject>] WrappedRepo() =
    member val Models: WrappedModel[] = [||] with get, set
    member this.Accept (visitor: Visitor) =
        visitor.Visit this
        for model in this.Models do
            model.Accept visitor

/// Helper class that represents the entire save file.
and [<JsonObject>] Save() =
    member val Version = "0.1" with get, set
    member val Extensions = Dictionary<string, obj>() with get, set
    member val Contents = WrappedRepo() with get, set
    member this.Accept (visitor: Visitor) =
        visitor.Visit this
        this.Contents.Accept visitor

/// Interface for visitors that can do something with deserialized object tree. Objects themselves are determining 
/// traverse order.
and Visitor =
    abstract Visit: WrappedAssociation -> unit
    abstract Visit: WrappedGeneralization -> unit
    abstract Visit: WrappedNode -> unit
    abstract Visit: WrappedModel -> unit
    abstract Visit: WrappedRepo -> unit
    abstract Visit: Save -> unit
