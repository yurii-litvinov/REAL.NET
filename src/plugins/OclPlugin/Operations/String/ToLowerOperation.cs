using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.String
{
    internal class ToLowerOperation : Operation
    {
        public ToLowerOperation()
            : base("toLower")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new StringResult(((StringResult)GlobalResult).GetValue().ToLower());
        }
    }
}
