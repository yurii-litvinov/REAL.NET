namespace Interpreters.RobotPerformer.DataLayer

open Interpreters
open Interpreters.Parser
open Interpreters.RobotPerformer
open Repo

type RCommand =
    | RForward
    | RBackward
    | RLeft
    | RRight
    | RNoCommand

type Context =
    { Maze: Maze
      Robot: Robot
      Commands: RCommand list }

module Context =
    let createContext maze robot =
        { Maze = maze
          Robot = robot
          Commands = [] }

    let addCommand command context = { context with Commands = command :: context.Commands }

module Helper =
    let checkMovement (maze: Maze) (pos: IntPoint) (dir: Direction) =
        if pos.X < maze.Width && pos.Y < maze.Height then
            let cell = maze.GetCell(pos)
            match dir with
            | Direction.Up -> cell.HasUp |> not
            | Direction.Down -> cell.HasDown |> not
            | Direction.Left -> cell.HasLeft |> not
            | Direction.Right -> cell.HasRight |> not
            | _ -> failwith "Not correct direction"
        else
            false

    let getDirectionOfRobot (robot: Robot) = robot.Direction

    let getOppositeDirection (robot: Robot) =
        match robot.Direction with
        | Direction.Up -> Direction.Down
        | Direction.Left -> Direction.Right
        | Direction.Right -> Direction.Left
        | Direction.Down -> Direction.Up
        | _ -> failwith "Unknown direction"

    let resourceSet = Resources.getResources()

    let getResource key = Resources.getString resourceSet key

module RobotParser =
    module AvailableParsers =
        let parseForward (p: Parsing<Context> option) =
            match p with
            | None -> None
            | Some ({ Variables = vars; Context = { Maze = maze; Robot = robot } as context; Element = element;
                      Model = model } as parsing) ->
                if element.Class.Name = "Forward" then
                    if (Helper.checkMovement maze (robot.Position) (robot.Direction)) then
                        let newRobot = Robot.moveRobot 1 robot
                        let newContext = Context.addCommand RForward { context with Robot = newRobot }
                        match ElementHelper.tryNext model element with
                        | None ->
                            ParserException.raiseWithPlace "Can't determine next element"
                                (PlaceOfCreation(Some model, Some element))
                        | Some nextElement ->
                            { parsing with
                                  Context = newContext
                                  Element = nextElement }
                            |> Some
                    else
                        OperatorException(Helper.getResource "Can't move because of wall", PlaceOfCreation(Some model, Some element))
                        |> raise
                else
                    None

        let parseBackward (p: Parsing<Context> option) =
            match p with
            | None -> None
            | Some ({ Variables = vars; Context = { Maze = maze; Robot = robot } as context; Element = element;
                      Model = model } as parsing) ->
                if element.Class.Name = "Backward" then
                    if (Helper.checkMovement maze (robot.Position) (Helper.getOppositeDirection robot)) then
                        let newRobot = Robot.moveRobot -1 robot
                        let newContext = Context.addCommand RBackward { context with Robot = newRobot }
                        match ElementHelper.tryNext model element with
                        | None ->
                            ParserException.raiseWithPlace "Can't determine next element"
                                (PlaceOfCreation(Some model, Some element))
                        | Some nextElement ->
                            { parsing with
                                  Context = newContext
                                  Element = nextElement }
                            |> Some
                    else
                        OperatorException(Helper.getResource "Can't move because of wall", PlaceOfCreation(Some model, Some element))
                        |> raise
                else
                    None

        let parseLeft (p: Parsing<Context> option) =
            match p with
            | None -> None
            | Some ({ Variables = vars; Context = { Maze = maze; Robot = robot } as context; Element = element;
                      Model = model } as parsing) ->
                if element.Class.Name = "Left" then
                    let newRobot = Robot.rotateLeft robot
                    let newContext = Context.addCommand RLeft { context with Robot = newRobot }
                    match ElementHelper.tryNext model element with
                    | None ->
                        ParserException.raiseWithPlace "Can't determine next element"
                            (PlaceOfCreation(Some model, Some element))
                    | Some nextElement ->
                        { parsing with
                              Context = newContext
                              Element = nextElement }
                        |> Some
                else
                    None

        let parseRight (p: Parsing<Context> option) =
            match p with
            | None -> None
            | Some ({ Variables = vars; Context = { Maze = maze; Robot = robot } as context; Element = element;
                      Model = model } as parsing) ->
                if element.Class.Name = "Right" then
                    let newRobot = Robot.rotateRight robot
                    let newContext = Context.addCommand RRight { context with Robot = newRobot }
                    match ElementHelper.tryNext model element with
                    | None ->
                        ParserException.raiseWithPlace "Can't determine next element"
                            (PlaceOfCreation(Some model, Some element))
                    | Some nextElement ->
                        { parsing with
                              Context = newContext
                              Element = nextElement }
                        |> Some
                else
                    None

        let parseInitialNode (p: Parsing<Context> option) =
            match p with
            | None -> None
            | Some ({ Variables = vars; Context = { Maze = maze; Robot = robot } as context; Element = element;
                      Model = model } as parsing) ->
                if element.Class.Name = "InitialNode" then
                    match ElementHelper.tryNext model element with
                    | None ->
                        ParserException.raiseWithPlace "Can't determine next element"
                            (PlaceOfCreation(Some model, Some element))
                    | Some nextElement ->
                        let newContext = Context.addCommand RNoCommand context
                        { parsing with
                              Element = nextElement
                              Context = newContext }
                        |> Some
                else
                    None

        let parseMovement = parseForward >>+ parseBackward >>+ parseLeft >>+ parseRight
        
        let parseRepeat = Interpreters.Parser.AvailableParsers.parseRepeat
        
        let parseIfElse = Interpreters.Parser.AvailableParsers.parseIfElse

    let parseRobot = AvailableParsers.parseMovement >>+ AvailableParsers.parseInitialNode >>+ AvailableParsers.parseRepeat >>+ AvailableParsers.parseIfElse
