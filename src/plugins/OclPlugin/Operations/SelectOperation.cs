using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations
{
    class SelectOperation : Operation
    {
        public SelectOperation()
            : base("select")
        {

        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            CollectionResult filteredElements = (CollectionResult) GlobalResult;

            Dictionary<string, Result> varsStack = new Dictionary<string, Result>();
            Vars.Add(varsStack);
            foreach (Result val in filteredElements.ToList())
            {
                varsStack["self"] = val;
                if (!Interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                {
                    filteredElements.Remove(val);
                }
            }

            Vars.RemoveAt(Vars.Count - 1);
            return filteredElements;
        }
    }
}
