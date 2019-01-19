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

        public abstract Result Add(Result res);

        public abstract Result Not();

        public abstract Result Multiply(Result res);

        public abstract Result Divide(Result res);
    }
}
