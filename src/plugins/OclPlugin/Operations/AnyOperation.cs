using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations
{
    internal class AnyOperation : Operation
    {
        public AnyOperation()
            : base("any")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            var varStack = new Dictionary<string, Result>();
            Vars.Add(varStack);
            Result returnValue = null;
            foreach (var val in (CollectionResult)GlobalResult)
            {
                varStack["self"] = val;
                if (Interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                {
                    returnValue = val;
                    break;
                }
            }

            Vars.RemoveAt(Vars.Count - 1);
            return returnValue;
        }
    }
}
