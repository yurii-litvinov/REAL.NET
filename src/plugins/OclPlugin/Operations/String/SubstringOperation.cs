using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.String
{
    internal class SubstringOperation : Operation
    {
        public SubstringOperation()
            : base("substring")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            var start = (IntResult)Calculator.VisitExpression(
                context.propertyCallParameters().actualParameterList().expression()[0]);
            var end = (IntResult)Calculator.VisitExpression(
                context.propertyCallParameters().actualParameterList().expression()[1]);
            return new StringResult(((StringResult)GlobalResult).GetValue().Substring((int)start.GetValue() - 1, (int)(end.GetValue() - start.GetValue() + 1)));
        }
    }
}
