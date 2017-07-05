namespace Repo

type IModelLoader =
    interface
        abstract LoadInto : repo : IMutableRepo -> modelName : string -> unit
    end
