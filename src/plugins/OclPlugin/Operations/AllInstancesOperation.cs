using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;

namespace OclPlugin.Operations
{
    class AllInstancesOperation : Operation
    {
        public AllInstancesOperation()
            : base("allInstances")
        {

        }

        public override Result Process(OclParser.PropertyCallContext context)
        {
            IElement element = Model.FindElement(((StringResult) GlobalResult).GetValue());
            return new CollectionResult(Model.Elements.Where(x => x.Class == element).ToList<object>());
        }
    }
}
