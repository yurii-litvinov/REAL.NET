using System;

namespace Logo.TurtleInterfaces
{
    public class LineEventArgs : EventArgs
    {
        public DoublePoint StartPoint => Line.Start;

        public DoublePoint EndPoint => Line.End;

        public OrientedLine Line { get; }

        public LineEventArgs(DoublePoint oldPosition, DoublePoint newPosition) => Line = new OrientedLine(oldPosition, newPosition);

        public LineEventArgs(OrientedLine line) => this.Line = line;
    }
}