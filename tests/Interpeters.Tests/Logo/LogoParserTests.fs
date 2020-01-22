module LogoParserTests

open NUnit
open NUnit.Framework
open FsUnit

open Repo
open Languages.Logo.LogoParser

let repo = RepoFactory.Create()

let metamodel = repo.Model "LogoMetamodel"

let model = repo.CreateModel("logoParserTests", "LogoMetamodel")

let init() = 
    1

[<Test>]
let tr() = true |> should be True

