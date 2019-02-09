using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using OclPlugin.Operations;
using OclPlugin.Operations.String;
using Repo;
using OclPlugin.Operations.Number;

namespace OclPlugin
{
    internal class OperationLauncher
    {
        private readonly List<Operation> operations = new List<Operation>()
        {
            new AllInstancesOperation(),
            new AnyOperation(),
            new CollectOperation(),
            new ForAllOperation(),
            new SelectOperation(),
            new SizeOperation(),
            new IterateOperation(),

            new ConcatOperation(),
            new SubstringOperation(),
            new ToIntegerOperation(),
            new ToLowerOperation(),
            new ToRealOperation(),
            new ToUpperOperation(),

            new AbsOperation(),
            new DivOperation(),
            new FloorOperation(),
            new MaxOperation(),
            new MinOperation(),
            new ModOperation(),
            new RoundOperation()
        };

        public OperationLauncher(OclInterpreter interpreter, OclInterpreter.OclCalculator calculator, ArrayList<Dictionary<string, Result>> vars, IModel model)
        {
            Operation.Init(interpreter, calculator, vars, model);
        }

        public Result LaunchOperation(string name, OclParser.PropertyCallContext context)
        {
            foreach (var operation in this.operations)
            {
                if (operation.GetName() == name)
                {
                    return operation.Process(context);
                }
            }

            return null;
        }
    }
}
