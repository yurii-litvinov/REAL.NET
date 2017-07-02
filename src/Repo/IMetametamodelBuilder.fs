namespace Repo.Metametamodels

open Repo.DataLayer

type IMetametamodelBuilder =
    interface
        abstract BuildInto : repo : IRepo -> IModel
    end