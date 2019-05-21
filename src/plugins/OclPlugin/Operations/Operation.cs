using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Repo;

namespace OclPlugin.Operations
{
    abstract class Operation
    {
        protected static Result GlobalResult
        {
            get => Calculator.GlobalResult;
            set => Calculator.GlobalResult = value;
        }

        protected static OclInterpreter Interpreter { get; set; }

        protected static OclInterpreter.OclCalculator Calculator { get; set; }

        protected static ArrayList<Dictionary<string, Result>> Vars { get; set; }

        public static IModel Model { get; set; }

        internal static void Init(OclInterpreter interpreter, OclInterpreter.OclCalculator calculator, ArrayList<Dictionary<string, Result>> vars, IModel model)
        {
            Interpreter = interpreter;

            Calculator = calculator;

            Vars = vars;

            Model = model;
        }

        private string name = string.Empty;

        public Operation(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public abstract Result Process(OclParser.PropertyCallContext context);
    }
}
