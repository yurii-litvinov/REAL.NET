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
 
module DataModelTests

open NUnit.Framework
open FsUnit

open RepoExperimental.DataLayer

[<Test>]
let ``Data model shall have name and metamodel`` () =
    let metamodel = DataModel("metamodel") :> IModel
    let model = DataModel("model", metamodel) :> IModel

    metamodel.Name |> should equal "metamodel"
    model.Name |> should equal "model"

    model.Metamodel |> should equal metamodel
    metamodel.Metamodel |> should equal metamodel

[<Test>]
let ``Data model shall allow changing name`` () =
    let metamodel = DataModel("metamodel") :> IModel
    let model = DataModel("model", metamodel) :> IModel

    metamodel.Name |> should equal "metamodel"
    model.Name |> should equal "model"

    metamodel.Name <- "new name"

    model.Metamodel.Name |> should equal "new name"
    metamodel.Name |> should equal "new name"

[<Test>]
let ``Data model shall allow creating nodes`` () =
    let model = DataModel("model") :> IModel
    let node = model.CreateNode "node1"
    model.Nodes |> should contain node

    let node2 = model.CreateNode("node2", node)
    model.Nodes |> should contain node2

    Seq.length model.Nodes |> should equal 2

[<Test>]
let ``Data model shall allow creating edges`` () =
    let model = DataModel("model") :> IModel
    let node1 = model.CreateNode "node1"
    let node2 = model.CreateNode "node2"

    let generalizationClass = model.CreateNode "generalization"
    let associationClass = model.CreateNode "association"

    let generalization = model.CreateGeneralization(generalizationClass, node1, node2)
    model.Edges |> should contain generalization

    let association = model.CreateAssociation("association1", associationClass, node1, node2, "node2End")
    model.Edges |> should contain association

    Seq.length model.Edges |> should equal 2

    Seq.append (model.Nodes |> Seq.cast<IElement>) (model.Edges |> Seq.cast<IElement>) |> should equal model.Elements

[<Test>]
let ``Data model shall allow creating unconnected associations`` () =
    let model = DataModel("model") :> IModel
    let node1 = model.CreateNode "node1" :> IElement
    let node2 = model.CreateNode "node2" :> IElement

    let associationClass = model.CreateNode "association"

    let association1 = model.CreateAssociation("association1", associationClass, Some node1, None, "association1end")
    let association2 = model.CreateAssociation("association2", associationClass, None, Some node2, "association2end")
    let association3 = model.CreateAssociation("association3", associationClass, Some node1, Some node2, "association3end")
    let association4 = model.CreateAssociation("association4", associationClass, None, None, "association4end")

    model.Edges |> should contain association1
    model.Edges |> should contain association2
    model.Edges |> should contain association3
    model.Edges |> should contain association4

[<Test>]
let ``Data model shall allow creating unconnected generalizations`` () =
    let model = DataModel("model") :> IModel
    let node1 = model.CreateNode "node1" :> IElement
    let node2 = model.CreateNode "node2" :> IElement

    let generalizationClass = model.CreateNode "generalization"

    let generalization1 = model.CreateGeneralization(generalizationClass, Some node1, None)
    let generalization2 = model.CreateGeneralization(generalizationClass, None, Some node2)
    let generalization3 = model.CreateGeneralization(generalizationClass, Some node1, Some node2)
    let generalization4 = model.CreateGeneralization(generalizationClass, None, None)

    model.Edges |> should contain generalization1
    model.Edges |> should contain generalization2
    model.Edges |> should contain generalization3
    model.Edges |> should contain generalization4

[<Test>]
let ``Data model shall allow deleting elements`` () =
    let model = DataModel("model") :> IModel
    let node1 = model.CreateNode "node1"
    model.Nodes |> should contain node1

    let node2 = model.CreateNode("node2", node1)
    model.Nodes |> should contain node2

    let generalizationClass = model.CreateNode "generalization"

    let generalization = model.CreateGeneralization(generalizationClass, node1, node2)
    model.Edges |> should contain generalization

    model.DeleteElement generalization
    model.Edges |> should not' (contain generalization)

    model.DeleteElement node1
    model.Nodes |> should not' (contain node1)
    model.Nodes |> should contain node2

    model.DeleteElement node2
    model.Nodes |> should not' (contain node2)

[<Test>]
let ``Data model shall disconnect edges on removing source or target`` () =
    let model = DataModel("model") :> IModel
    let node1 = model.CreateNode "node1"
    let node2 = model.CreateNode("node2", node1)

    let generalizationClass = model.CreateNode "generalization"
    let generalization = model.CreateGeneralization(generalizationClass, node1, node2)
    model.Edges |> should contain generalization
    generalization.Source |> should equal (Some (node1 :> IElement))
    generalization.Target |> should equal (Some (node2 :> IElement))

    model.DeleteElement node1

    generalization.Source |> should equal None
    generalization.Target |> should equal (Some (node2 :> IElement))

    model.DeleteElement node2

    generalization.Source |> should equal None
    generalization.Target |> should equal None

[<Test>]
let ``Data model shall disconnect edges on removing source or target edges`` () =
    let model = DataModel("model") :> IModel
    let generalizationClass = model.CreateNode "generalization"

    let edge1 = model.CreateGeneralization(generalizationClass, None, None)
    let edge2 = model.CreateGeneralization(generalizationClass, Some (edge1 :> IElement), None)
    let edge3 = model.CreateGeneralization(generalizationClass, edge1, edge2)
     
    model.Edges |> should contain edge1
    model.Edges |> should contain edge2
    model.Edges |> should contain edge3
    edge3.Source |> should equal (Some (edge1 :> IElement))
    edge3.Target |> should equal (Some (edge2 :> IElement))

    model.DeleteElement edge1

    edge3.Source |> should equal None
    edge3.Target |> should equal (Some (edge2 :> IElement))
    edge2.Source |> should equal None
    edge2.Target |> should equal None
