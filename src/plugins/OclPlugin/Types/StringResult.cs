using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
    internal class StringResult : Result
    {
        private string value = string.Empty;

        public StringResult(string str)
        {
            this.value = str;
        }

        public string GetValue()
        {
            return value;
        }

        public override int CompareTo(object obj)
        {
            return value.CompareTo(((StringResult)obj).GetValue());
        }

        public override Result Add(Result res)
        {
            return new StringResult(this.value + ((StringResult) res).GetValue());
        }

    }
}
