using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations.Number
{
    internal class RoundOperation : Operation
    {
        public RoundOperation()
            : base("round")
        {
        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            return new DoubleResult(Math.Round(((DoubleResult)GlobalResult).GetValue()));
        }
    }
}
