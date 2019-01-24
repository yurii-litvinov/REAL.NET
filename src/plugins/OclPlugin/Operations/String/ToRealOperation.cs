using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.String
{
    internal class ToRealOperation : Operation
    {
        public ToRealOperation()
            : base("toReal")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new DoubleResult(double.Parse(((StringResult)GlobalResult).GetValue()));
        }
    }
}
