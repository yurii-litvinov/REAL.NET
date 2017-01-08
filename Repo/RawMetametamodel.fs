namespace Repo

type MultiplicityConstraint =
    | Concrete of int
    | Unbounded

type Multiplicity =
    { min : int;
      max : MultiplicityConstraint }

type ModelElement() =
    [<DefaultValue>]
    val mutable Class : Node

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
    val mutable Source : Node

    [<DefaultValue>]
    val mutable Target : Node

type Generalization() =
    inherit Relationship ()

type Association() =
    inherit Relationship ()

    [<DefaultValue>]
    val mutable SourceMultiplicity : Multiplicity

    [<DefaultValue>]
    val mutable TargetMultiplicity : Multiplicity
