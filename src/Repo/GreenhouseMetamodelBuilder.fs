namespace Repo.Metametamodels

open Repo
open Repo.DataLayer
open Repo.InfrastructureSemanticLayer
open System.Security.Policy

type GreenhouseMetamodelBuilder() =
    interface IModelBuilder with
        member this.Build(repo: IRepo): unit =
            let infrastructure = InfrastructureSemanticLayer.InfrastructureSemantic(repo)
            let metamodel = infrastructure.Metamodel.Model

            let find name = CoreSemanticLayer.Model.findNode metamodel name

            let metamodelElement = find "Element"
            let metamodelNode = find "Node"
            let metamodelGeneralization = find "Generalization"
            let metamodelAssociation = find "Association"

            let model = repo.CreateModel("GreenhouseMetamodel", metamodel)

            let (~+) (name, shape, isAbstract) =
                let node = infrastructure.Instantiate model metamodelNode :?> INode
                node.Name <- name
                infrastructure.Element.SetAttributeValue node "shape" shape
                infrastructure.Element.SetAttributeValue node "isAbstract" (if isAbstract then "true" else "false")
                infrastructure.Element.SetAttributeValue node "instanceMetatype" "Metatype.Node"

                node

            let (--|>) (source: IElement) target =
                model.CreateGeneralization(metamodelGeneralization, source, target) |> ignore

            let (--->) (source: IElement) (target, targetName, linkName) =
                let edge = infrastructure.Instantiate model metamodelAssociation :?> IAssociation
                edge.Source <- Some source
                edge.Target <- Some target
                edge.TargetName <- targetName

                infrastructure.Element.SetAttributeValue edge "shape" "Pictures/Edge.png"
                infrastructure.Element.SetAttributeValue edge "isAbstract" "false"
                infrastructure.Element.SetAttributeValue edge "instanceMetatype" "Metatype.Edge"
                infrastructure.Element.SetAttributeValue edge "name" linkName

                edge

            let abstractNode = +("AbstractNode", "", true)

            let abstractOperation = +("AbstractOperation", "", true)
            let andOperation = +("AndOperation", "Pictures/andOperation.png", false)
            let orOperation = +("OrOperation", "Pictures/orOperation.png", false)

            let abstractActuator = +("AbstractActuator", "", true)
            let openWindow = +("OpenWindow", "Pictures/openWindow.png", false)
            let closeWindow = +("CloseWindow", "Pictures/closeWindow.png", false)
            let pourSoil = +("PourSoil", "Pictures/pourSoil.png", false)

            let abstractSensor = +("AbstractSensor", "", true)
            let airTemperature = +("AirTemperature", "Pictures/airTemperature.png", false)
            let soilTemperature = +("SoilTemperature", "Pictures/soilTemperature.png", false)

            let interval = +("Interval", "Pictures/interval.png", false)

            infrastructure.Element.AddAttribute interval "min" "AttributeKind.Int" "0"
            infrastructure.Element.AddAttribute interval "max" "AttributeKind.Int" "0"
            
            infrastructure.Element.AddAttribute airTemperature "value" "AttributeKind.Int" "0"

            infrastructure.Element.AddAttribute soilTemperature "value" "AttributeKind.Int" "0"

            let link = abstractNode ---> (abstractNode, "target", "Link")
            infrastructure.Element.AddAttribute link "guard" "AttributeKind.String" ""

            abstractOperation --|> abstractNode
            abstractActuator --|> abstractNode
            abstractSensor --|> abstractNode
            
            openWindow --|> abstractActuator
            closeWindow --|> abstractActuator
            pourSoil --|> abstractActuator

            airTemperature --|> abstractSensor
            soilTemperature --|> abstractSensor

            interval --|> abstractNode

            andOperation --|> abstractOperation
            orOperation --|> abstractOperation

            ()
            