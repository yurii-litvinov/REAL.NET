using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.String
{
    internal class ToIntegerOperation : Operation
    {
        public ToIntegerOperation()
            : base("toInteger")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new IntResult(int.Parse(((StringResult)GlobalResult).GetValue()));
        }
    }
}
