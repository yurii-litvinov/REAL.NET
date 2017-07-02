namespace RepoExperimental.Metametamodels

open RepoExperimental.DataLayer

type IMetametamodelBuilder =
    interface
        abstract BuildInto : repo : IRepo -> IModel
    end