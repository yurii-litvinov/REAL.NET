namespace OclPlugin.Operations.Number
{
    using System;

    internal class MaxOperation : Operation
    {
        public MaxOperation()
            : base("max")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            Result value = Calculator.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]);

            return new DoubleResult(Math.Max(((DoubleResult)GlobalResult).GetValue(), ((DoubleResult)value).GetValue()));
        }
    }
}
