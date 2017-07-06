namespace RepoExperimental.DataLayer

type DataRepo() =
    let mutable models = []

    interface IRepo with
        member this.CreateModel(name: string): IModel = 
            let model = DataModel(name)
            models <- model :: models
            model :> IModel
        
        member this.DeleteModel(model: IModel): unit = 
            models <- models |> List.filter (fun m -> not ((m :> IModel).Equals(model)))
        
        member this.Models: seq<IModel> = 
            Seq.ofList models |> Seq.cast