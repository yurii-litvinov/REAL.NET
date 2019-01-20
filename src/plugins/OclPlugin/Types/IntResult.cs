namespace OclPlugin
{
    internal class IntResult : DoubleResult
    {
        private readonly int value = 0;

        public IntResult(int val)
            : base(val)
        {
            this.value = val;
        }

        public override double GetValue()
        {
            return this.value;
        }

        public override Result Not()
        {
            return new IntResult(-this.value);
        }
    }
}
