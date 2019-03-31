using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
    class CollectionResult : Result, IEnumerable<Result>
    {
        private ICollection<Result> value = new List<Result>();

        public CollectionResult(ICollection<Result> val)
        {
            this.value = val;
        }

        public CollectionResult(ICollection<object> collection)
        {
            foreach (var val in collection)
            {
                switch (val)
                {
                    case int i:
                        this.Add(new IntResult(i));
                        break;
                    case double d:
                        this.Add(new DoubleResult(d));
                        break;
                    case string str:
                        this.Add(new StringResult(str));
                        break;
                    case ICollection<object> objects:
                        this.Add(new CollectionResult(objects));
                        break;
                }
            }
        }

        public override int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Result> GetEnumerator()
        {
            return this.value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.value.GetEnumerator();
        }

        public override Result Add(Result result)
        {
            this.value.Add(result);
            return this;
        }

        public Result Remove(Result result)
        {
            this.value.Remove(result);
            return this;
        }

        public int Count()
        {
            return this.value.Count;
        }
    }
}
