module TurtleCommand

open System
open Languages.Logo.LogoSpecific

type LCommand =
    | LForward of double
    | LBackward of double
    | LLeft of double
    | LRight of double
    | PenDown
    | PenUp
    | SetSpeed of double

let (*) x (y, z) = (x * y, x * z)

let (+.) (x, y) (z, t) = (x + z, y + t)

let evaluateTime (distance: double) speed = distance / speed 

let forward distance (turtle: LogoTurtle) (stats: LogoStats) =
    let dp = distance * (cos(turtle.AngleInRadians), sin(turtle.AngleInRadians))
    let time = evaluateTime distance turtle.Speed
    let st = LogoStats(stats.Distance + distance, stats.Time + time)
    let newTurtle = LogoTurtle.NewPosition turtle (turtle.Position +. dp)
    (newTurtle, st)


    