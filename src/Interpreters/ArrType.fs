namespace Interpreters

open System.Collections.Immutable

type ArrType =
    | IntArray of ImmutableArray<int>
    | DoubleArray of ImmutableArray<double>
    | BoolArray of ImmutableArray<bool>
    | StringArray of ImmutableArray<string>
    
module ArrType =
    let initArray n init =
        let arrInit n v =
            let collection = Seq.replicate n v
            ImmutableArray.CreateRange(collection)
        match init with
        | Int v -> arrInit n v |> IntArray
        | Double v -> arrInit n v |> DoubleArray
        | Bool v -> arrInit n v |> BoolArray
        | String v -> arrInit n v |> StringArray
        
    let tryCreateArray list =
        let rec extractInt list out =
            match list with
            | Int x :: t -> extractInt t (x :: out)
            | _ :: _ -> None
            | [] -> List.rev out |> Some
        let rec extractDouble list out =
            match list with
            | Double x :: t -> extractDouble t (x :: out)
            | _ :: _ -> None
            | [] -> List.rev out |> Some
        let rec extractBool list out =
            match list with
            | Bool x :: t -> extractBool t (x :: out)
            | _ :: _ -> None
            | [] -> List.rev out |> Some
        let rec extractString list out =
            match list with
            | String x :: t -> extractString t (x :: out)
            | _ :: _ -> None
            | [] -> List.rev out |> Some

        match list with
        | h :: t ->
            match h with
            | Int x ->
                match extractInt t [] with
                | Some l -> ImmutableArray.CreateRange(x :: l) |> IntArray |> Some
                | None -> None
            | Double x ->
                match extractDouble t [] with
                | Some l -> ImmutableArray.CreateRange(x :: l) |> DoubleArray |> Some
                | None -> None
            | Bool x ->
                match extractBool t [] with
                | Some l -> ImmutableArray.CreateRange(x :: l) |> BoolArray |> Some
                | None -> None
            | String x ->
                match extractString t [] with
                | Some l -> ImmutableArray.CreateRange(x :: l) |> StringArray |> Some
                | None -> None
        | [] -> None
        
    let getType v =
        match v with
        | IntArray _ -> ArrayTypes.IntArray
        | DoubleArray _ -> ArrayTypes.DoubleArray
        | BoolArray _ -> ArrayTypes.BoolArray
        | StringArray _ -> ArrayTypes.StringArray
        
    let getElementType v =
        match v with
        | IntArray _ -> PrimitiveTypes.Int
        | DoubleArray _ -> PrimitiveTypes.Double
        | BoolArray _ -> PrimitiveTypes.Bool
        | StringArray _ -> PrimitiveTypes.String
    
    let isTypesEqual x y = getType x = getType y
    
    let changeValueAtIndex i newVal arr  =
            match (arr, newVal) with
            | (IntArray a, Int x) ->
                if (i < a.Length) then a.SetItem(i, x) |> IntArray
                else System.IndexOutOfRangeException() |> raise    
            | (DoubleArray a, Double x) ->
                if (i < a.Length) then a.SetItem(i, x) |> DoubleArray
                else System.IndexOutOfRangeException() |> raise
            | (BoolArray a, Bool x) ->
                if (i < a.Length) then a.SetItem(i, x) |> BoolArray
                else System.IndexOutOfRangeException() |> raise
            | (StringArray a, String x) ->
                if (i < a.Length) then a.SetItem(i, x) |> StringArray
                else System.IndexOutOfRangeException() |> raise
            | _ -> "Try to insert element of another type" |> invalidOp |> raise
        
    let getValueAtIndex i arr =
        match arr with
        | IntArray a -> a.[i] |> Int
        | DoubleArray a -> a.[i] |> Double
        | BoolArray a -> a.[i] |> Bool
        | StringArray a -> a.[i] |> String
        
    let toList arr =
        match arr with
        | IntArray a -> [ for i in a -> i |> Int]
        | DoubleArray a -> [ for i in a -> i |> Double]
        | BoolArray a -> [ for i in a -> i |> Bool]
        | StringArray a -> [ for i in a -> i |> String]
    
        

