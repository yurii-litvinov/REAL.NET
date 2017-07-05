namespace Repo

[<AbstractClass; Sealed>]
type RepoFactory =
    static member CreateRepo () = new RepoImpl () :> IRepo
