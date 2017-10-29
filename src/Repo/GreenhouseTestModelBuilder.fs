namespace Repo.Metametamodels

open Repo.DataLayer
open Repo.CoreSemanticLayer
open Repo.InfrastructureSemanticLayer

type GreenhouseTestModelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemantic(repo)
            let metamodel = Repo.findModel repo "GreenhouseMetamodel"

            let metamodelAbstractNode = Model.findNode metamodel "AbstractNode"
            let metamodelAirTemperature = Model.findNode metamodel "AirTemperature"
            let metamodelSoilTemperature = Model.findNode metamodel "SoilTemperature"
            let metamodelInterval = Model.findNode metamodel "Interval"
            let metamodelAndOperation = Model.findNode metamodel "AndOperation"
            let metamodelOpenWindow = Model.findNode metamodel "OpenWindow"

            let link = Model.findAssociationWithSource metamodelAbstractNode "target"

            let model = repo.CreateModel("GreenhouseTestModel", metamodel)

            let airTemperature = infrastructure.Instantiate model metamodelAirTemperature
            let soilTemperature = infrastructure.Instantiate model metamodelSoilTemperature
            let interval1 = infrastructure.Instantiate model metamodelInterval
            let interval2 = infrastructure.Instantiate model metamodelInterval
            let andOperation = infrastructure.Instantiate model metamodelAndOperation
            let openWindow = infrastructure.Instantiate model metamodelOpenWindow

            let (-->) (src: IElement) dst =
                let aLink = infrastructure.Instantiate model link :?> IAssociation
                aLink.Source <- Some src
                aLink.Target <- Some dst
                dst

            airTemperature --> interval1 --> andOperation --> openWindow |> ignore
            soilTemperature --> interval2 --> andOperation |> ignore

            ()
