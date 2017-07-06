namespace RepoExperimental.DataLayer

type DataModel private (name: string, metamodel: IModel option) =

    new(name: string) = DataModel(name, None)
    new(name: string, metamodel: IModel) = DataModel(name, Some metamodel)

    interface IModel with
        member this.CreateAssociation name ``class`` source target targetName = 
            raise (System.NotImplementedException())

        member this.CreateGeneralization(name: string) (``class``: string) (source: IElement) (target: IElement): unit = 
            raise (System.NotImplementedException())

        member this.CreateNode(name: string) (``class``: IElement): INode = 
            raise (System.NotImplementedException())

        member this.DeleteElement(element: IElement): unit = 
            raise (System.NotImplementedException())

        member this.Elements: IElement list = 
            raise (System.NotImplementedException())

        member this.GetElement(name: string): IElement = 
            raise (System.NotImplementedException())

        member this.GetNode(name: string): INode = 
            raise (System.NotImplementedException())

        member this.Metamodel
            with get (): IModel = 
                match metamodel with 
                | Some v -> v
                | None -> this :> IModel

        member val Name = name with get, set

        member this.Nodes: seq<INode> = 
            raise (System.NotImplementedException()) 
