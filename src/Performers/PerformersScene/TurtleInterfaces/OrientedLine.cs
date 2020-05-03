namespace Logo.TurtleInterfaces
{
    public readonly struct OrientedLine
    {
        public OrientedLine(PerformersScene.Models.DoublePoint start, PerformersScene.Models.DoublePoint end)
        {
            Start = start;
            End = end;
        }

        public PerformersScene.Models.DoublePoint Start { get; }

        public PerformersScene.Models.DoublePoint End { get; }

        public override string ToString()
        {
            return $"{Start}, {End}";
        }
    }
}