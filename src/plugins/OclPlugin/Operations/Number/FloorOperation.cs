namespace OclPlugin.Operations.Number
{
    using System;

    internal class FloorOperation : Operation
    {
        public FloorOperation()
            : base("floor")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new DoubleResult(Math.Floor(((DoubleResult)GlobalResult).GetValue()));
        }
    }
}
