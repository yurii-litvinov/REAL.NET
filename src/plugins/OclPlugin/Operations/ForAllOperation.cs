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
            return new BoolResult(this.Iterate(0, context));
        }

        private bool Iterate(int pos, OclParser.PropertyCallContext context)
        {
            if (((context.propertyCallParameters().declarator() == null) && (pos > 0)) || pos == context.propertyCallParameters().declarator().NAME().Length)
            {
                return Interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]);
            }

            var varStack = new Dictionary<string, Result>();
            Vars.Add(varStack);
            string key = "self";
            if (context.propertyCallParameters().declarator() != null)
            {
                key = context.propertyCallParameters().declarator().NAME()[pos].GetText();
            }

            foreach (var val in (CollectionResult)GlobalResult)
            {
                varStack[key] = val;
                if (!this.Iterate(pos + 1, context))
                {
                    Vars.RemoveAt(Vars.Count - 1);
                    return false;
                }
            }

            Vars.RemoveAt(Vars.Count - 1);
            return true;
        }
    }
}
