using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations
{
    class CollectOperation : Operation
    {
        public CollectOperation()
            : base("collect")
        {

        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            CollectionResult newElements = (CollectionResult)GlobalResult;

            foreach (Result val in newElements.ToList())
            {
                GlobalResult = val;
                newElements.Remove(val);
                newElements.Add(Calculator.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]));
            }

            return newElements;
        }
    }
}
