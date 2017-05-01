module MetametamodelTests

open NUnit.Framework
open FsUnit

open Repo

let private repo = RepoFactory.CreateRepo () :?> RepoImpl
let private modelName = "mainModel"

[<SetUp>]
let setUp () =
    let metametamodel = GraphMetametamodel () :> IModelLoader
    metametamodel.LoadInto (repo :> IMutableRepo) modelName

[<Test>]
let ``Each model element shall have type`` () =
    let repo' = repo :> IRepo
    let nodes = repo'.ModelNodes modelName
    nodes |> Seq.iter (fun node -> repo'.NodeType node.id |> ignore)

    let metamodelNodes = repo'.MetamodelNodes modelName
    metamodelNodes |> Seq.iter (fun node -> repo'.NodeType node.id |> ignore)
    
    let edges = repo'.ModelEdges modelName
    edges |> Seq.iter (fun edge -> repo'.EdgeType edge.id |> ignore)
    