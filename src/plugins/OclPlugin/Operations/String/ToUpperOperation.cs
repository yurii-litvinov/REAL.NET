using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.String
{
    internal class ToUpperOperation : Operation
    {
        public ToUpperOperation()
            : base("toUpper")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new StringResult(((StringResult)GlobalResult).GetValue().ToUpper());
        }
    }
}
