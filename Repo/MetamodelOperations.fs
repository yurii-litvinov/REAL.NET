﻿namespace Repo

open QuickGraph

module internal MetamodelOperations =

    let private outEdges (repo : RepoRepresentation) node = (fst repo).OutEdges node

    let private followEdge repo edgePredicate node = 
        outEdges repo node
        |> Seq.filter edgePredicate
        |> Seq.exactlyOne
        |> fun e -> e.Target

    let private attrType repo a = a |> followEdge repo (fun e -> match e.Tag with | _, Type _ -> true | _ -> false) |> fun n -> (fst n).Id

    let private intrinsicAttributeValue attribute node = 
        match attribute.Name with
        | "Name" -> node.Name
        | "Level" -> string node.Level
        | "Potency" -> string node.Potency
        | _ -> ""

    let private attributeValue repo attribute node =
        let attributeAttributes, NodeKind.Attribute attributeValue = attribute
        match attributeAttributes.Name with
        | "Name" | "Level" | "Potency" -> intrinsicAttributeValue attributeAttributes node
        | _ -> 
            match attributeValue.Value with
            | AttributeValue.None -> ""
            | String s -> s
            | Int i -> string i
            | Ref r -> ""

    let private currentNodeAttributeNodes repo node =
        outEdges repo node 
        |> Seq.filter (fun e -> match e.Tag with | _, Attribute _ -> true | _ -> false)
        |> Seq.map (fun e -> e.Target)

    let rec private effectiveAttributeNodes repo node =
        outEdges repo node
        |> Seq.filter (fun e -> match e.Tag with | _, Generalization _ -> true | _ -> false)
        |> Seq.map (fun e -> e.Target)
        |> Seq.collect (fun parent -> effectiveAttributeNodes repo parent)
        |> Seq.append (currentNodeAttributeNodes repo node)

    let newId () = System.Guid.NewGuid().ToString()

    let rec effectiveAttributes repo node =
        effectiveAttributeNodes repo node 
        |> Seq.map (fun attr -> new AttributeInfo((fst attr).Name, attrType repo attr, attributeValue repo attr (fst node)))

    let instance (repo : RepoRepresentation) (class' : ModelElementLabel) (attributeValues : Map<VertexLabel, AttributeValue>) =
        let hasAttribute attributeName = 
            attributeValues 
            |> Seq.filter (fun x -> (fst x.Key).Name = attributeName)
            |> Seq.isEmpty
            |> not

        let getStringAttributeValue attributeName = 
            attributeValues 
            |> Seq.filter (fun x -> (fst x.Key).Name = attributeName) 
            |> Seq.exactlyOne 
            |> fun x -> x.Value 
            |> function 
               | String s -> s 
               | _ -> failwith "Incorrect attribute value"

        let name = if hasAttribute "Name" then 
                       getStringAttributeValue "Name" 
                   else
                       ""

        let classModelElementAttributes = 
            match class' with
            | Vertex (attrs, _) -> attrs
            | Edge (attrs, _) -> attrs

        if classModelElementAttributes.Potency = 0 then
            raise <| InstantiationOfAbstractElementException class'

        let instanceModelElementAttributes = { Id = newId (); Name = name; Potency = classModelElementAttributes.Potency - 1; Level = classModelElementAttributes.Level + 1 }
        let result = (instanceModelElementAttributes, Node)
        let graph, classes = repo

        let add v =
            if not <| graph.AddVertex v then 
                failwith "Adding instance to a graph failed"

        add result
        classes.Add(Vertex result, class')

        // Attributes-related stuff
        let (Vertex classVertex) = class'
        let classAttributes = effectiveAttributeNodes repo classVertex

        let value attr = 
            let ({Id = _; Name = name; Potency = potency; Level = _ }, _) = attr
            if (not <| attributeValues.ContainsKey attr) && potency = 1 then
                failwith <| "Value for attribute " + name + " is needed but not provided for instantiation"
            elif not <| attributeValues.ContainsKey attr then
                AttributeValue.None
            else
                attributeValues.[attr]

        let addTypeEdge source target =
            let label = { Id = newId (); Name = "Type"; Potency = 0; Level = instanceModelElementAttributes.Level }, EdgeKind.Type
            let edge = new TaggedEdge<_, _>(source, target, label)
            graph.AddEdge edge |> ignore

        let type' node = followEdge repo (fun e -> match e.Tag with | _, Type _ -> true | _ -> false) node

        let instanceAttributes = 
            classAttributes
            |> Seq.filter (fun (x, _) -> x.Potency <> 0)
            |> Seq.map (fun (({Id = _; Name = name; Potency = potency; Level = level }, _) as a) -> 
                a, ({ Id = newId (); Name = name; Potency = potency - 1; Level = level + 1}, NodeKind.Attribute { Value = value a }))
            |> Seq.map (fun ((classAttribute, instanceAttribute) as attributePair) -> add instanceAttribute; attributePair)
            |> Seq.map (fun ((classAttribute, instanceAttribute) as attributePair) -> classes.Add(Vertex instanceAttribute, Vertex classAttribute); attributePair)
            |> Seq.map (fun (classAttribute, instanceAttribute) -> addTypeEdge instanceAttribute (type' classAttribute); instanceAttribute)
            |> Seq.toList

        let attributeLabel () = { Id = newId (); Name = "Attribute"; Potency = 0; Level = instanceModelElementAttributes.Level }, EdgeKind.Attribute

        if (instanceAttributes |> Seq.map (fun v -> new TaggedEdge<_, _>(result, v, attributeLabel ())) |> Seq.map (fun x -> graph.AddEdge x) |> Seq.forall id |> not) then
            failwith "Failed to add attribute edges to a graph"

        result

