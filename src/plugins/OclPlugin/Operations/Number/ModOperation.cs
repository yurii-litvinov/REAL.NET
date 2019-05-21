namespace OclPlugin.Operations.Number
{
    using System;

    internal class ModOperation : Operation
    {
        public ModOperation()
            : base("mod")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            Result value = Calculator.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]);

            return new DoubleResult(((int)((DoubleResult)GlobalResult).GetValue()) % ((int)((DoubleResult)value).GetValue()));
        }
    }
}
