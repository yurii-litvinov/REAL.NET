namespace OclPlugin.Operations
{
    using System.Collections.Generic;

    internal class IterateOperation : Operation
    {
        public IterateOperation()
            : base("iterate")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            var varStack = new Dictionary<string, Result>();
            Vars.Add(varStack);
            string key = "self";
            key = context.propertyCallParameters().declarator().NAME()[0].GetText();
            string accName = context.propertyCallParameters().declarator().NAME()[1].GetText();
            Result accumulator = Calculator.VisitExpression(context.propertyCallParameters().declarator().expression());
            varStack[accName] = accumulator;
            foreach (var val in (CollectionResult)GlobalResult)
            {
                varStack[key] = val;
                varStack[accName] = Calculator.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]);
            }

            accumulator = varStack[accName];
            Vars.RemoveAt(Vars.Count - 1);
            return accumulator;
        }
    }
}
