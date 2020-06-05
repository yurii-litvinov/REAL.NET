module Interpreters.RobotPerformer.Resources

open System
open System.Resources
open System.Reflection
open System.Globalization

let private currentInfo = CultureInfo.CurrentCulture
let privateInvariantInfo = CultureInfo.InvariantCulture
let private russianInfo = CultureInfo.GetCultureInfo "ru-RU"
let private info = currentInfo

let private resourcesSet = 
    Lazy.CreateFromValue(
        let manager = new ResourceManager("Interpreters.RobotPerformer.LanguageResource", Assembly.GetExecutingAssembly())
        manager.GetResourceSet(info, true, true)
    )

let getResources() = 
    resourcesSet.Value
   
   
let tryGetString (set: ResourceSet) key = 
    try set.GetString key |> Some
    with :? InvalidOperationException -> None

let getString set key =
    match tryGetString set key with
    | Some x -> x
    | None -> InvalidOperationException "No such resource" |> raise
         



    


