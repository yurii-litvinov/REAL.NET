module DataModelTests

open NUnit.Framework
open FsUnit

open RepoExperimental.DataLayer

[<Test>]
let ``Model shall have name and metamodel`` () =
    let metamodel = DataModel("metamodel") :> IModel
    let model = DataModel("model", metamodel) :> IModel

    metamodel.Name |> should equal "metamodel"
    model.Name |> should equal "model"

    model.Metamodel |> should equal metamodel
    metamodel.Metamodel |> should equal metamodel
