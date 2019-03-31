using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.Number
{
    class AbsOperation : Operation
    {
        public AbsOperation()
            : base("abs")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new DoubleResult(Math.Abs(((DoubleResult)GlobalResult).GetValue()));
        }
    }
}
