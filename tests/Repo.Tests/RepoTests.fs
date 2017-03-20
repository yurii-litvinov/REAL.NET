module RepoTests

open NUnit.Framework
open FsUnit

open Repo

let mutable repo = RepoFactory.CreateRepo ()

[<SetUp>]
let setup () =
    repo <- RepoFactory.CreateRepo ()

//[<Test>]
//let ``Repo shall be able to load a model`` () =
//    let modelLoader = ()
//    repo.LoadModel modelLoader
//    repo.Models |> should not' (be Empty)


[<Test>]
let ``Repo shall return some metamodel nodes`` () =
    repo.MetamodelNodes () |> should not' (be Empty)

[<Test>]
let ``Repo shall return some model nodes`` () =
    repo.ModelNodes () |> should not' (be Empty)

[<Test>]
let ``Repo shall contain at least ModelElement`` () =
    repo.ModelNodes () |> Seq.filter (fun x -> x.name = "ModelElement") |> should not' (be Empty)

