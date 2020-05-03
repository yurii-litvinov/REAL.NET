﻿using System;

namespace Logo.TurtleInterfaces
{
    public class LineEventArgs : EventArgs
    {
        public PerformersScene.Models.DoublePoint StartPoint => Line.Start;

        public PerformersScene.Models.DoublePoint EndPoint => Line.End;

        public OrientedLine Line { get; }

        public LineEventArgs(PerformersScene.Models.DoublePoint oldPosition, PerformersScene.Models.DoublePoint newPosition) => Line = new OrientedLine(oldPosition, newPosition);

        public LineEventArgs(OrientedLine line) => this.Line = line;
    }
}