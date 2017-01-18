module RepoTests

open NUnit.Framework
open FsUnit

[<Test>]
let ``Repo should return some metamodel nodes`` () =
    Repo.Repo.MetamodelNodes () |> should not' (be Empty)

[<Test>]
let ``Repo should return some model nodes`` () =
    Repo.Repo.ModelNodes () |> should not' (be Empty)

[<Test>]
let ``Repo shall contain at least ModelElement`` () =
    Repo.Repo.ModelNodes () |> should contain "ModelElement"
