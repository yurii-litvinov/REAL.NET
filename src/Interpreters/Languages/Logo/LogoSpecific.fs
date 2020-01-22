module Languages.Logo.LogoSpecific

open System

type LogoTurtle(x: double, y: double, angle: double, isPenDown: bool, speed: double) = 
    struct 
        member this.X = x

        member this.Y = y

        member this.Position = (x, y)

        member this.Angle = angle

        member this.AngleInRadians = angle / 360.0 * 2.0 * Math.PI

        member this.IsPenDown = isPenDown

        member this.Speed = speed;

        static member NewPosition (turtle: LogoTurtle) (x, y) = LogoTurtle (x, y, turtle.Angle, turtle.IsPenDown, turtle.Speed)

        static member NewX (turtle: LogoTurtle) x = LogoTurtle.NewPosition turtle (x, turtle.Y)
        
        static member NewY (turtle: LogoTurtle) y = LogoTurtle.NewPosition turtle (turtle.X, y)

        static member PenDown (turtle: LogoTurtle) = LogoTurtle (turtle.X, turtle.Y, turtle.Angle, true, turtle.Speed)
        
        static member PenUp (turtle: LogoTurtle) = LogoTurtle (turtle.X, turtle.Y, turtle.Angle, false, turtle.Speed)

        static member NewAngle (turtle: LogoTurtle) angle = 
            let rec findCorrectAngle (angle: double) = 
                if (angle > 360.0) then findCorrectAngle (angle - 360.0)
                elif (angle < 0.0) then findCorrectAngle (angle + 360.0)
                else angle
            let newAngle = findCorrectAngle angle
            LogoTurtle (turtle.X, turtle.Y, newAngle, turtle.IsPenDown, turtle.Speed)
    end

type LogoStats(distance: double, time: double) =
    struct
        member this.Distance = distance

        member this.Time = time

        static member NewDistance (stats: LogoStats) distance = LogoStats(distance, stats.Time)

        static member NewTime (stats: LogoStats) time = LogoStats(stats.Distance, time)
    end

[<AbstractClass>]
type LogoCommand() =
    class
        abstract member Description: string with get
    end

type LogoForward(distance: double) =
    inherit LogoCommand()

    override this.Description = "forward " + distance.ToString()

    member this.Distance = distance

type LogoBackward(distance: double) =
    inherit LogoCommand()

    override this.Description = "backward " + distance.ToString()

    member this.Distance = distance

type LogoLeft(degrees: double) =
    inherit LogoCommand()

    override this.Description = "left " + degrees.ToString()

    member this.Degrees = degrees

type LogoRight(degrees: double) =
    inherit LogoCommand()

    override this.Description = "right " + degrees.ToString()

type LogoPenUp() =
    inherit LogoCommand()

    override this.Description = "pen up"

type LogoPenDown() =
    inherit LogoCommand()

    override this.Description = "pen down"

type LogoSetSpeed(speed: double) =
    inherit LogoCommand()

    override this.Description = "set speed"

    member this.Speed = speed

type LogoContext() =
    class
        member this.LogoCommands : ResizeArray<LogoCommand> = new ResizeArray<LogoCommand>()
    end

