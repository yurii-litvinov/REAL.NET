namespace Repo

type internal GraphMetametamodel () =
    interface IModelLoader with
        member this.LoadInto repo =
            let helper = ModelBuildingHelper repo
            let (~-) = helper.createAbstractNode
            let (~+) = helper.createConcreteNode
            let (--@-->) x y = helper.AddInstantiation x y

            let modelElement = -"ModelElement"
            let node = +"Node"
            let attribute = +"Attribute"
            let relationship = -"Relationship"
            let generalization = +"Generalization"
            let association = +"Association"

            let stringType = -"String"
            let intType = -"Int"

            modelElement.id --@--> node.id
            node.id --@--> node.id
            attribute.id --@--> node.id
            relationship.id --@--> node.id
            generalization.id --@--> node.id
            association.id --@--> node.id

            let createAssociation source target targetName targetPotency targetLabel =
                let newAssociation = helper.CreateAssociation source target targetName targetPotency targetLabel
                newAssociation.id --@--> association.id
                newAssociation

            let (---|>) x y = 
                let generalizationLink = helper.AddGeneralization x y
                generalizationLink.id --@--> generalization.id

            // "Instantiation" relation is actually an instance of a "class" association, but we don't model 
            // "Instantiation" as first-class relation, so it can not have type.
            createAssociation modelElement modelElement "class" 1 1 |> ignore
            
            let attributesAssociation = createAssociation modelElement attribute "attributes" 0 -1
            let (--+-->) x y = 
                let attributeLink = helper.AddAttribute x y
                attributeLink.id --@--> attributesAssociation.id

            let typesAssociation = createAssociation attribute node "type" 1 1
            let (--*-->) x y = 
                let typeLink = helper.AddType x y
                typeLink.id --@--> typesAssociation.id

            let valueAssociation = createAssociation attribute node "value" 0 1

            let createAttribute node name type' value = helper.CreateAttribute node name type' value attribute valueAssociation attributesAssociation typesAssociation

            createAssociation relationship modelElement "source" 1 1 |> ignore
            createAssociation relationship modelElement "target" 1 1 |> ignore

            createAttribute modelElement "Name" stringType (String "ModelElement")
            createAttribute modelElement "Potency" intType (Int -1)
            createAttribute modelElement "Level" intType (Int 0)

            createAttribute association "MinSource" intType None
            createAttribute association "MaxSource" intType None
            createAttribute association "SourceName" stringType None
            createAttribute association "MinTarget" intType None
            createAttribute association "MaxTarget" intType None
            createAttribute association "TargetName" stringType None

            stringType.id --@--> node.id
            intType.id --@--> node.id

            node ---|> modelElement
            attribute ---|> modelElement
            relationship ---|> modelElement
            generalization ---|> relationship
            association ---|> relationship

            ()