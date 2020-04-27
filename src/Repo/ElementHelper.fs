module Repo.ElementHelper

let outgoingEdges (model: IModel) (element: IElement) =
    model.Edges |> Seq.filter (fun (e: IEdge) -> e.From = element) 

let incomingEdges (model: IModel) (element: IElement) =
    model.Edges |> Seq.filter (fun (e: IEdge) -> e.To = element)
    
let getAllNext model element =
    outgoingEdges model element 
    |> Seq.map (fun e -> e.To)
    
let getAllPrevious model element =
    outgoingEdges model element
    |> Seq.map (fun e -> e.From)
    
let tryNext model element =
    getAllNext model element |> Seq.tryExactlyOne
    
let next model element =
    getAllNext model element |> Seq.exactlyOne

let hasAttribute name (element: IElement) =
    element.Attributes |> Seq.filter (fun x -> x.Name = name) |> Seq.isEmpty |> not
    
let findAttribute name (element: IElement) =
    element.Attributes |> Seq.find (fun x -> x.Name = name)
    
let getAttributeValue name (element: IElement) =
    let attribute = element.Attributes |> Seq.find (fun x -> x.Name = name)
    attribute.StringValue
            
let setAttributeValue name value (element: IElement) =
    let attr = findAttribute name element
    attr.StringValue <- value