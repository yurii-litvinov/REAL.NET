using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations
{
    class ForAllOperation : Operation
    {
        public ForAllOperation()
            : base("forAll")
        {

        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            var varStack = new Dictionary<string, Result>();
            Vars.Add(varStack);
            foreach (var val in (CollectionResult)GlobalResult)
            {
                varStack["self"] = val;
                if (!Interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                {
                    return new BoolResult(false);
                }
            }

            Vars.RemoveAt(Vars.Count - 1);
            return new BoolResult(true);
        }
    }
}
