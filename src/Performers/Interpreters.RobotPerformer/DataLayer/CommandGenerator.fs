module Interpreters.RobotPerformer.DataLayer.CommandGenerator

open Interpreters.RobotPerformer

let generateCommand command =
    match command with
    | RForward -> RobotForward() :> RobotCommand
    | RBackward -> RobotBackward() :> RobotCommand
    | RLeft -> RobotLeft() :> RobotCommand
    | RRight -> RobotRight() :> RobotCommand
    | RNoCommand -> RobotNoCommand() :> RobotCommand