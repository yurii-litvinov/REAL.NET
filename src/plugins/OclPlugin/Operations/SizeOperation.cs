using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin.Operations
{
    class SizeOperation : Operation
    {
        public SizeOperation()
            : base("size")
        {

        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            switch (GlobalResult)
            {
                case CollectionResult objects:
                    return new IntResult(objects.Count());
                case StringResult s:
                    return new IntResult(s.GetValue().Length);
            }

            return null;
        }
    }
}
