namespace Repo

type MultiplicityConstraint =
    | Concrete of int
    | Unbounded

type Multiplicity =
    { min : int;
      max : MultiplicityConstraint }

type ModelElement() =
    [<DefaultValue>]
    val mutable Class : ModelElement

    [<DefaultValue>]
    val mutable Attributes : Attribute list

and Node() = 
    inherit ModelElement ()

and Attribute() = 
    inherit ModelElement ()

    [<DefaultValue>]
    val mutable Type : Node

    [<DefaultValue>]
    val mutable Value : Node option

type Relationship() =
    inherit ModelElement ()
    [<DefaultValue>]
    val mutable Source : ModelElement

    [<DefaultValue>]
    val mutable Target : ModelElement

type Generalization() =
    inherit Relationship ()

type Association() =
    inherit Relationship ()

    [<DefaultValue>]
    val mutable SourceMultiplicity : Multiplicity

    [<DefaultValue>]
    val mutable SourceName : string

    [<DefaultValue>]
    val mutable TargetMultiplicity : Multiplicity

    [<DefaultValue>]
    val mutable TargetName : string
