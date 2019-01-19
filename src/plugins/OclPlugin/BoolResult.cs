using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
    internal class BoolResult : Result
    {
        private readonly bool value = false;

        public BoolResult(bool value)
        {
            this.value = value;
        }

        public bool GetValue()
        {
            return this.value;
        }

        public override int CompareTo(object obj)
        {
            return this.value.CompareTo(((BoolResult)obj).GetValue());
        }

        public override Result Add(Result res)
        {
            throw new NotImplementedException();
        }

        public override Result Not()
        {
            return new BoolResult(!this.GetValue());
        }

        public override Result Multiply(Result res)
        {
            throw new NotImplementedException();
        }

        public override Result Divide(Result res)
        {
            throw new NotImplementedException();
        }
    }
}
