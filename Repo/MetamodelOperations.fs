namespace Repo

open GraphMetametamodel
open QuickGraph
open System.Collections.Generic

module internal MetamodelOperations =

    let private outEdges (repo : RepoRepresentation) node = (fst repo).OutEdges node

    let private followEdge repo edgePredicate node = 
        outEdges repo node
        |> Seq.filter edgePredicate
        |> Seq.exactlyOne
        |> fun e -> e.Target

    let private attrType repo a = a |> followEdge repo (fun e -> match e.Tag with | _, Type _ -> true | _ -> false) |> fun n -> (fst n).id

    let private attrValue attribute node = 
        match attribute.name with
        | "Name" -> node.name
        | "Level" -> string node.level
        | "Potency" -> string node.potency
        | _ -> ""

    let private attributes repo node =
        outEdges repo node 
        |> Seq.filter (fun e -> match e.Tag with | _, Attribute _ -> true | _ -> false)
        |> Seq.map (fun e -> e.Target)
        |> Seq.map (fun attr -> new AttributeInfo((fst attr).name, attrType repo attr, attrValue (fst attr) (fst node)))

    let rec effectiveAttributes repo node =
        outEdges repo node
        |> Seq.filter (fun e -> match e.Tag with | _, Generalization _ -> true | _ -> false)
        |> Seq.map (fun e -> e.Target)
        |> Seq.collect (fun parent -> effectiveAttributes repo parent)
        |> Seq.append (attributes repo node)
        |> Seq.toList

    let instance (repo : RepoRepresentation) class' (attributeValues : Map<VertexLabel, AttributeValue>) =
        ()
