namespace OclPlugin.Operations.Number
{
    using System;

    internal class MinOperation : Operation
    {
        public MinOperation()
            : base("min")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            Result value = Calculator.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]);

            return new DoubleResult(Math.Min(((DoubleResult)GlobalResult).GetValue(), ((DoubleResult)value).GetValue()));
        }
    }
}
