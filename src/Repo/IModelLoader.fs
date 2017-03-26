namespace Repo

type IModelLoader =
    interface
        abstract LoadInto : repo : IMutableRepo -> unit
    end
