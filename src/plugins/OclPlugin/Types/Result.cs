using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
    internal abstract class Result : IComparable
    {
        public abstract int CompareTo(object obj);

        public virtual Result Add(Result res) => throw new NotImplementedException();

        public virtual Result Not() => throw new NotImplementedException();

        public virtual Result Multiply(Result res) => throw new NotImplementedException();

        public virtual Result Divide(Result res) => throw new NotImplementedException();
    }
}
