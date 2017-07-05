namespace Repo

type internal ModelBuildingHelper (repo : IMutableRepo, modelName : string) =
    member this.CreateNode name potency = 
        repo.AddNode name potency 0 modelName

    member this.CreateEdge edgeType sourceId targetId = 
        repo.AddEdge edgeType sourceId targetId -1 0 "a" 0 -1 "b" 0 -1 modelName

    member this.CreateAssociation (source : NodeInfo) (target : NodeInfo) targetName targetMin targetMax = 
        repo.AddEdge EdgeType.Association source.id target.id -1 0 "a" 0 -1 targetName targetMin targetMax modelName

    member this.createConcreteNode name = 
        this.CreateNode name -1

    member this.createAbstractNode name = 
        this.CreateNode name 0

    member this.AddGeneralization (source : NodeInfo) (target  : NodeInfo) = this.CreateEdge EdgeType.Generalization source.id target.id
    member this.AddInstantiation sourceId targetId = repo.AddInstantiationRelation sourceId targetId
    member this.AddAttribute (source : NodeInfo) (target  : NodeInfo) = this.CreateEdge EdgeType.Attribute source.id target.id
    member this.AddType (source : NodeInfo) (target  : NodeInfo) = this.CreateEdge EdgeType.Type source.id target.id

    member this.CreateAttribute node name type' (value : AttributeValue) (attribute : NodeInfo) (valueAssociation : EdgeInfo) (attributeLinkType : EdgeInfo) (typeLinkType : EdgeInfo) =
        let potency = if name = "Name" then -1 else 0

        let attrValue = match value with String s -> s | _ -> ""

        let attributeNode = repo.AddAttribute name potency 0 attrValue modelName

        this.AddInstantiation attributeNode.id attribute.id

        match value with 
        | Ref v ->
            let valueLink = this.CreateEdge EdgeType.Value attributeNode.id (fst v).Id
            this.AddInstantiation valueLink.id valueAssociation.id
        | _ -> ()

        let attributeLink = this.AddAttribute node attributeNode
        this.AddInstantiation attributeLink.id attributeLinkType.id

        let typeLink = this.AddType attributeNode type'
        this.AddInstantiation typeLink.id typeLinkType.id
