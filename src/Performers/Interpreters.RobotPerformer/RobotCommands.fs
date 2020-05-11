namespace Interpreters.RobotPerformer

[<AbstractClass>]
type RobotCommand(description: string) =
    class
        member this.Description = description
    end
    
type RobotForward() =
    inherit RobotCommand("Move robot forward")
    
type RobotBackward() =
    inherit RobotCommand("Move robot backward")
    
type RobotRight() =
    inherit RobotCommand("Rotate robot right")
    
type RobotLeft() =
    inherit RobotCommand("Rotate robot left")
    
type RobotNoCommand() =
    inherit RobotCommand("No command")

