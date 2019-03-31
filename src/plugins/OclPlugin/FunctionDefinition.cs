namespace OclPlugin
{
    using System.Collections.Generic;

    internal class FunctionDefinition
    {
        public string Name { get; set; }

        public List<string> Param { get; set; }

        public OclParser.ExpressionContext Context { get; set; }
    }
}
