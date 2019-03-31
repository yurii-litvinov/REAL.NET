using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.String
{
    internal class ConcatOperation : Operation
    {
        public ConcatOperation()
            : base("concat")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            var sourceString = (StringResult)GlobalResult;
            var concatString =
                (StringResult)Calculator.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]);
            return new StringResult(sourceString.GetValue() + concatString.GetValue());
        }
    }
}
