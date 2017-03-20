namespace Repo

type internal GraphMetametamodel () =
    interface IModelLoader with
        member this.LoadInto repo =
            let helper = ModelBuildingHelper repo
            let (~-) = helper.createAbstractNode
            let (~+) = helper.createConcreteNode
            let createNode = helper.CreateNode
            let createEdge = helper.CreateEdge
            let createAssociation = helper.CreateAssociation
            let (--@-->) x y = helper.AddInstantiation x y
            let (--+-->) x y = helper.AddAttribute x y
            let (--*-->) x y = helper.AddType x y
            let (---|>) x y = helper.AddGeneralization x y

            let modelElement = -"ModelElement"
            let node = +"Node"
            let attribute = +"Attribute"
            let relationship = -"Relationship"
            let generalization = +"Generalization"
            let association = +"Association"

            let stringType = -"String"
            let intType = -"Int"

            createAssociation modelElement modelElement "class" 1 1 |> ignore
            createAssociation modelElement attribute "attributes" 0 -1 |> ignore
            createAssociation attribute node "type" 1 1 |> ignore

            let valueAssociation = createAssociation attribute node "value" 0 1
            valueAssociation.id --@--> association.id

            let createAttribute node name type' value = helper.CreateAttribute node name type' value attribute valueAssociation

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