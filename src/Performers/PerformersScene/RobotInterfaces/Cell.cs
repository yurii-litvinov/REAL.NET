namespace PerformersScene.RobotInterfaces
{
    public readonly struct Cell
    {
        public Cell(bool hasLeftWall, bool hasRightWall, bool hasUpWall, bool hasDownWall)
        {
            HasLeftWall = hasLeftWall;
            HasRightWall = hasRightWall;
            HasUpWall = hasUpWall;
            HasDownWall = hasDownWall;
        }

        public bool HasLeftWall { get; }
        
        public bool HasRightWall { get; }
        
        public bool HasUpWall { get; }
        
        public bool HasDownWall { get; }
    }
    
    public readonly struct Side
    {
        public Side(bool isWall)
        {
            this.IsWall = isWall;
        }

        public bool IsWall { get; }
    }
}

