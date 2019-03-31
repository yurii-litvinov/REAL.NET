namespace OclPlugin
{
    internal class DoubleResult : Result
    {
        private readonly double value = 0.0;

        public DoubleResult(double value)
        {
            this.value = value;
        }

        public virtual double GetValue()
        {
            return this.value;
        }

        public override int CompareTo(object obj)
        {
            return this.value.CompareTo(((DoubleResult)obj).GetValue());
        }

        public override Result Add(Result res)
        {
            return new DoubleResult(this.value + ((DoubleResult)res).GetValue());
        }

        public override Result Not()
        {
            return new DoubleResult(-this.GetValue());
        }

        public override Result Multiply(Result res)
        {
            return new DoubleResult(((DoubleResult)res).GetValue() * this.GetValue());
        }

        public override Result Divide(Result res)
        {
            return new DoubleResult(this.GetValue() / ((DoubleResult)res).GetValue());
        }
    }
}
