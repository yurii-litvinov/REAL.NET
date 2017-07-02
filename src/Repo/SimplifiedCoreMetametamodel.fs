namespace Repo

type internal SimplifiedCoreMetametamodel () =
    interface IModelLoader with
        member this.LoadInto repo modelName =
            let helper = ModelBuildingHelper(repo, modelName)
            let (~-) = helper.createAbstractNode
            let (~+) = helper.createConcreteNode
            let (--@-->) x y = helper.AddInstantiation x y

            let modelElement = -"ModelElement"
            let node = +"Node"
            let relationship = -"Relationship"
            let generalization = +"Generalization"
            let association = +"Association"

            let stringType = -"String"
            let intType = -"Int"

            modelElement.id --@--> node.id
            node.id --@--> node.id
            relationship.id --@--> node.id
            generalization.id --@--> node.id
            association.id --@--> node.id

            let (-->) source (target, targetName, targetPotency, targetLabel) = 
                let newAssociation = helper.CreateAssociation source target targetName targetPotency targetLabel
                newAssociation.id --@--> association.id

            let (--|>) x y = 
                let generalizationLink = helper.AddGeneralization x y
                generalizationLink.id --@--> generalization.id

            // "Instantiation" relation is actually an instance of a "class" association, but we don't model 
            // "Instantiation" as first-class relation, so it can not have type.
            modelElement --> (modelElement, "class", 1, 1)
            
            relationship --> (modelElement, "source", 1, 1)
            relationship --> (modelElement, "target", 1, 1)

            stringType.id --@--> node.id
            intType.id --@--> node.id

            node --|> modelElement
            relationship --|> modelElement
            generalization --|> relationship
            association --|> relationship

            ()