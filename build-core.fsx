#r "paket: groupref FakeBuild //"
#if !FAKE
  #r "Facades/netstandard"
  #r "netstandard"
#endif

#load "./.fake/build-core.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO

let solutionFile  = "Repo.Core.sln"

Target.create "Clean" (fun _ -> Shell.cleanDirs ["bin"; "temp"])
Target.create "Build" (fun _ -> solutionFile |> DotNet.build id)
Target.create "RunTests" (fun _ -> solutionFile |> DotNet.test id)
Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "RunTests"
  ==> "All"

Target.runOrDefault "All"
