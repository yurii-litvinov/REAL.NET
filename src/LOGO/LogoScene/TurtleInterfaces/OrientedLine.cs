namespace Logo.TurtleInterfaces
{
    public struct OrientedLine
    {
        public OrientedLine(DoublePoint start, DoublePoint end)
        {
            Start = start;
            End = end;
        }

        public DoublePoint Start { get; }

        public DoublePoint End { get; }

        public override string ToString()
        {
            return $"{Start}, {End}";
        }
    }
}