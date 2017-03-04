namespace Repo

open QuickGraph
open System.Collections.Generic
open MetamodelOperations

module internal GraphMetametamodel =

    let private createGeneralizationLabel () =
        let modelElementAttributes = { Id = newId(); Name = "Generalization"; Potency = 0; Level = 0 }
        (modelElementAttributes, Generalization)

    let private createAttributeLabel () =
        let modelElementAttributes = { Id = newId(); Name = "Attribute"; Potency = 0; Level = 0 }
        (modelElementAttributes, Attribute)

    let private createTypeLabel () =
        let modelElementAttributes = { Id = newId(); Name = "Type"; Potency = 1; Level = 0 }
        (modelElementAttributes, Type)

    let private createValueLabel () =
        let modelElementAttributes = { Id = newId(); Name = "Value"; Potency = 0; Level = 0 }
        (modelElementAttributes, Value)

    let private createAssociationLabel targetName targetMin targetMax =
        let modelElementAttributes = { Id = newId(); Name = "Association"; Potency = -1; Level = 0 }
        let associationAttributes = { SourceName = "Source"; SourceMin = 1; SourceMax = 1; TargetName = targetName; TargetMin = targetMin; TargetMax = targetMax }
        (modelElementAttributes, Association associationAttributes)

    let createM0Model () =
        let repo : RepoRepresentation = (new BidirectionalGraph<_, _> true, new Dictionary<_, _>())
        let repoGraph, classes = repo

        let createEdge label source target = 
            let edge = new TaggedEdge<_, _>(source, target, label)
            repoGraph.AddEdge edge |> ignore
            label

        let createNode name potency = 
            let vertex = { Id = newId (); Name = name; Potency = potency; Level = 0 }
            repoGraph.AddVertex (vertex, Node) |> ignore
            vertex, Node

        let (~+) name = 
            createNode name -1

        let (~-) name = 
            createNode name 0

        let (---|>) source target = createEdge (createGeneralizationLabel ()) source target |> ignore

        let (--+-->) source target = createEdge (createAttributeLabel ()) source target |> ignore

        let (--*-->) source target = createEdge (createTypeLabel ()) source target |> ignore

        let createAssociation source target targetName targetMin targetMax = createEdge (createAssociationLabel targetName targetMin targetMax) source target

        let (--@-->) source target = classes.Add(source, target)

        let modelElement = -"ModelElement"
        let node = +"Node"
        let attribute = +"Attribute"
        let relationship = -"Relationship"
        let generalization = +"Generalization"
        let association = +"Association"

        let stringType = -"String"
        let intType = -"Int"

        let classAssociation = createAssociation modelElement modelElement "class" 1 1
        createAssociation modelElement attribute "attributes" 0 -1 |> ignore
        createAssociation attribute node "type" 1 1 |> ignore

        let valueAssociation = createAssociation attribute node "value" 0 1
        ModelElementLabel.Edge valueAssociation --@--> Vertex association

        createAssociation relationship modelElement "source" 1 1 |> ignore
        createAssociation relationship modelElement "target" 1 1 |> ignore

        let createAttribute node name type' value =
            let attributeAttributes = { Value = value }
            let potency = if name = "Name" then -1 else 0
            let modelElementAttributes = { Id = newId(); Name = name; Potency = potency; Level = 0 }
            let attributeLabel = modelElementAttributes, NodeKind.Attribute attributeAttributes
            repoGraph.AddVertex attributeLabel |> ignore

            Vertex attributeLabel --@--> Vertex attribute
            match value with 
            | Ref v ->
                let valueLink = createEdge (createValueLabel ()) attributeLabel v 
                ModelElementLabel.Edge valueLink --@--> ModelElementLabel.Edge valueAssociation
            | _ -> ()

            node --+--> attributeLabel
            attributeLabel --*--> type'

        createAttribute modelElement "Name" stringType (String "ModelElement")
        createAttribute modelElement "Potency" intType (Int -1)
        createAttribute modelElement "Level" intType (Int 0)

        createAttribute association "MinSource" intType None
        createAttribute association "MaxSource" intType None
        createAttribute association "SourceName" stringType None
        createAttribute association "MinTarget" intType None
        createAttribute association "MaxTarget" intType None
        createAttribute association "TargetName" stringType None

        Vertex stringType --@--> Vertex node
        Vertex intType --@--> Vertex node

        node ---|> modelElement
        attribute ---|> modelElement
        relationship ---|> modelElement
        generalization ---|> relationship
        association ---|> relationship

        (repoGraph, classes)
