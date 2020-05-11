namespace Interpreters.RobotPerformer.DataLayer

open System.Collections.Immutable
open System.Net

type IntPoint =
    | IntPoint of int * int

    member this.X =
        match this with
        | IntPoint (x, y) -> x

    member this.Y =
        match this with
        | IntPoint (x, y) -> y

    static member (+)(x: IntPoint, y: IntPoint) = IntPoint(x.X + y.X, x.Y + y.Y)

    static member (*)(k: int, x: IntPoint) = IntPoint(k * x.X, k * x.Y)

type Direction =
    | Up = 0
    | Down = 1
    | Left = 2
    | Right = 3

type Robot(coordinates: IntPoint, dir: Direction) =
    struct
        member this.Position = coordinates

        member this.Direction = dir
    end

type Cell(hasLeft: bool, hasUp: bool, hasRight: bool, hasDown: bool) =
    member this.HasLeft = hasLeft

    member this.HasUp = hasUp

    member this.HasRight = hasRight

    member this.HasDown = hasDown

type Maze(horizontalLines: bool [,], verticalLines: bool [,]) =

    member this.Width = verticalLines.GetLength(0) - 1

    member this.Height = horizontalLines.GetLength(0) - 1

    member this.GetCell(position: IntPoint) =
        let hasLeft = verticalLines.[position.X, position.Y]
        let hasDown = horizontalLines.[position.Y, position.X]
        let hasRight = verticalLines.[position.X + 1, position.Y]
        let hasUp = horizontalLines.[position.Y + 1, position.X]
        Cell(hasLeft, hasUp, hasRight, hasDown)

module Robot =
    let newPosition pos (robot: Robot) = Robot(pos, robot.Direction)

    let newDirection dir (robot: Robot) = Robot(robot.Position, dir)

    let moveRobot numberOfCells (robot: Robot) =
        let moveOnePos (robot: Robot) =
            match robot.Direction with
            | Direction.Up -> IntPoint(0, 1) + robot.Position
            | Direction.Left -> IntPoint(-1, 0) + robot.Position
            | Direction.Right -> IntPoint(1, 0) + robot.Position
            | Direction.Down -> IntPoint(0, -1) + robot.Position
            | _ -> failwith "Unknown direction"
        newPosition (numberOfCells * (moveOnePos robot)) robot

    let rotateRight (robot: Robot) =
        let newDir =
            match robot.Direction with
            | Direction.Up -> Direction.Right
            | Direction.Right -> Direction.Down
            | Direction.Down -> Direction.Left
            | Direction.Left -> Direction.Up
            | _ -> failwith "Unknown direction"
        newDirection newDir robot

    let rotateLeft (robot: Robot) =
        let newDir =
            match robot.Direction with
            | Direction.Up -> Direction.Left
            | Direction.Right -> Direction.Up
            | Direction.Down -> Direction.Right
            | Direction.Left -> Direction.Down
            | _ -> failwith "Unknown direction"
        newDirection newDir robot
