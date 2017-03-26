module MetametamodelTests

open NUnit.Framework
open FsUnit

open Repo

let private repo = RepoFactory.CreateRepo () :?> RepoImpl

[<SetUp>]
let setUp () =
    let metametamodel = GraphMetametamodel () :> IModelLoader
    metametamodel.LoadInto (repo :> IMutableRepo)

[<Test>]
let ``Each model element shall have type`` () =
    let repo' = repo :> IRepo
    let nodes = repo'.ModelNodes ()
    nodes |> Seq.iter (fun node -> repo'.NodeType node.id |> ignore)

    let metamodelNodes = repo'.MetamodelNodes ()
    metamodelNodes |> Seq.iter (fun node -> repo'.NodeType node.id |> ignore)
    
    let edges = repo'.ModelEdges ()
    edges |> Seq.iter (fun edge -> repo'.EdgeType edge.id |> ignore)
    