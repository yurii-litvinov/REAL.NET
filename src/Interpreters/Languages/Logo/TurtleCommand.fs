module Languages.Logo.TurtleCommand

open Languages.Logo.LogoSpecific

type LCommand =
    | LForward of double
    | LBackward of double
    | LLeft of double
    | LRight of double
    | LPenDown
    | LPenUp
    | LSetSpeed of double
    | LNoCommand

let (*) x (y, z) = (x * y, x * z)

let (+.) (x, y) (z, t) = (x + z, y + t)

let evaluateTime (distance: double) speed = distance / speed 

let forward distance (turtle: LogoTurtle) (stats: LogoStats) =
    let dp = distance * (cos(turtle.AngleInRadians), sin(turtle.AngleInRadians))
    let time = evaluateTime distance turtle.Speed
    let st = LogoStats(stats.Distance + distance, stats.Time + time)
    let newTurtle = LogoTurtle.NewPosition turtle (turtle.Position +. dp)
    (newTurtle, st)


let convertToLogoCommand (command: LCommand) : LogoCommand =
    match command with 
    | LForward d -> new LogoForward(d) :> LogoCommand
    | LBackward d -> new LogoBackward(d) :> LogoCommand
    | LRight d -> new LogoRight(d) :> LogoCommand
    | LLeft d -> new LogoLeft(d) :> LogoCommand
    | LPenUp -> new LogoPenUp() :> LogoCommand
    | LPenDown -> new LogoPenDown() :> LogoCommand
    | LSetSpeed s -> new LogoSetSpeed(s) :> LogoCommand
    | _ -> failwith "unknown command"



    