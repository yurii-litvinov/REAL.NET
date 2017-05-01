namespace Repo

type internal TestModelLoader () =
    interface IModelLoader with
        member this.LoadInto repo modelName =
            let helper = ModelBuildingHelper(repo, modelName)
            let (~-) = helper.createAbstractNode
            let (~+) = helper.createConcreteNode

            let testElement1 = -"TestElement1"
            let testElement2 = +"TestElement2"

            let createAssociation source target targetName targetPotency targetLabel =
                let newAssociation = helper.CreateAssociation source target targetName targetPotency targetLabel
                newAssociation
                
            createAssociation testElement2 testElement1 "testRole" 1 1 |> ignore

            ()