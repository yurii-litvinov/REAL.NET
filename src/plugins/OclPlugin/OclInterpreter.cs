using System.ComponentModel.Design;

namespace OclPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Antlr4.Runtime.Misc;
    using EditorPluginInterfaces;
    using Repo;

    internal class OclInterpreter : OclBaseVisitor<bool>
    {
        private readonly ArrayList<Dictionary<string, Result>> vars;
        private readonly OclCalculator calculator;
        private readonly Dictionary<string, FunctionDefinition> funcs;
        private readonly IConsole console;
        private readonly IRepo repository;
        private Repo.IModel model;
        private IElement currentElement;

        public OclInterpreter(IConsole console, IRepo repo)
        {
            this.vars = new ArrayList<Dictionary<string, Result>>
            {
                new Dictionary<string, Result>()
            };
            this.funcs = new Dictionary<string, FunctionDefinition>();
            this.calculator = new OclCalculator(this.vars, this.funcs, this, repo, console);
            this.console = console;
            this.repository = repo;
        }

        public override bool VisitPackageName([NotNull] OclParser.PackageNameContext context)
        {
            this.model = this.repository.Model(context.pathName().GetText());
            this.calculator.Model = this.model;
            return this.VisitPathName(context.pathName());
        }

        public override bool VisitConstraint([NotNull] OclParser.ConstraintContext context)
        {
            this.VisitContextDeclaration(context.contextDeclaration());
            string text = context.contextDeclaration().classifierContext().GetText();
            this.currentElement = this.model.FindElement(text);
            this.calculator.Element = this.currentElement;
            for (int i = 0; i < context.oclExpression().Length; i++)
            {
                this.calculator.Depth = int.Parse(context.NUMBER()[i].GetText());
                foreach (IElement element in this.model.Elements)
                {
                    IElement parent = element;
                    for (int j = 0; j < this.calculator.Depth; j++)
                    {
                        parent = parent.Class;
                    }

                    if (parent == this.currentElement)
                    {
                        IElement cur = this.currentElement;
                        this.currentElement = parent;
                        this.VisitOclExpression(context.oclExpression()[i]);
                        this.currentElement = cur;
                    }
                }

                this.calculator.Depth = 0;
            }

            return base.VisitConstraint(context);
        }

        public override bool VisitOclExpression([NotNull] OclParser.OclExpressionContext context)
        {
            foreach (var expression in context.letExpression())
            {
                this.VisitLetExpression(expression);
            }

            if (!this.VisitExpression(context.expression()))
            {
                this.console.SendMessage("error");
            }
            else
            {
                this.console.SendMessage("ok");
            }

            return true;
        }

        public override bool VisitLetExpression([NotNull] OclParser.LetExpressionContext context)
        {
            if (context.formalParameterList() != null)
            {
                this.funcs[context.NAME().GetText()] = new FunctionDefinition
                {
                    Param = context.formalParameterList().NAME()
                    .Select(x => x.GetText()).ToList(), Context = context.expression()
                };
            }
            else
            {
                this.vars[this.vars.Count - 1][context.NAME().GetText()] = new StringResult(context.expression().GetText());
            }

            return true;
        }

        public override bool VisitImplExpression([NotNull] OclParser.ImplExpressionContext context)
        {
            bool result = this.VisitOrExpression(context.orExpression()[0]);
            for (int i = 1; i < context.orExpression().Length; i++)
            {
                result = !result || this.VisitOrExpression(context.orExpression()[i]);
            }

            return result;
        }

        public override bool VisitOrExpression([NotNull] OclParser.OrExpressionContext context)
        {
            bool result = this.VisitAndExpression(context.andExpression()[0]);
            for (int i = 1; i < context.andExpression().Length; i++)
            {
                switch (context.orOperator()[i - 1].GetText())
                {
                    case "or":
                        result = result || this.VisitAndExpression(context.andExpression()[i]);
                        break;
                    default:
                        result = result ^ this.VisitAndExpression(context.andExpression()[i]);
                        break;
                }
            }

            return result;
        }

        public override bool VisitAndExpression([NotNull] OclParser.AndExpressionContext context)
        {
            bool result = this.VisitEqExpression(context.eqExpression()[0]);
            for (int i = 1; i < context.eqExpression().Length; i++)
            {
                result = result && this.VisitEqExpression(context.eqExpression()[i]);
            }

            return result;
        }

        public override bool VisitEqExpression([NotNull] OclParser.EqExpressionContext context)
        {
            Result result = (Result)this.calculator.VisitRelationalExpression(context.relationalExpression()[0]);

            for (int i = 1; i < context.relationalExpression().Length; i++)
            {
                var leftObj = result;
                var rightObj = this.calculator.VisitRelationalExpression(context.relationalExpression()[i]);

                switch (context.eqOperator()[i - 1].GetText())
                {
                    case "=":
                        result = new BoolResult(leftObj.CompareTo(rightObj) == 0);
                        break;
                    default:
                        result = new BoolResult(leftObj.CompareTo(rightObj) != 0);
                        break;
                }
            }

            return ((BoolResult)result).GetValue();
        }

        private class OclCalculator : OclBaseVisitor<Result>
        {
            private readonly ArrayList<Dictionary<string, Result>> vars = null;
            private readonly Dictionary<string, FunctionDefinition> funcs = null;
            private readonly OclInterpreter interpreter = null;
            private readonly IRepo repository = null;
            private readonly IConsole console = null;

            public int Depth { internal get; set; } = 0;

            public Repo.IModel Model { private get; set; } = null;

            public IElement Element { private get; set; } = null;

            public Result GlobalResult { get; private set; } = null;

            public OclCalculator(ArrayList<Dictionary<string, Result>> vars, Dictionary<string, FunctionDefinition> funcs, OclInterpreter interpreter, IRepo repo, IConsole cons)
            {
                this.vars = vars;
                this.funcs = funcs;
                this.interpreter = interpreter;
                this.repository = repo;
                this.console = cons;
            }

            public override Result VisitRelationalExpression([NotNull] OclParser.RelationalExpressionContext context)
            {
                Result result = (Result)this.VisitAdditiveExpression(context.additiveExpression()[0]);

                for (int i = 1; i < context.additiveExpression().Length; i++)
                {
                    var leftObj = result;
                    var rightObj = this.VisitAdditiveExpression(context.additiveExpression()[i]);

                    switch (context.relationalOperator().Start.Text)
                    {
                        case "<":
                            result = new BoolResult(leftObj.CompareTo(rightObj) < 0);
                            break;
                        case ">":
                            result = new BoolResult(leftObj.CompareTo(rightObj) > 0);
                            break;
                    }
                }

                return result;
            }

            public override Result VisitAdditiveExpression([NotNull] OclParser.AdditiveExpressionContext context)
            {
                if (context.addOperator() == null || context.addOperator().Length == 0)
                {
                    return this.VisitMultiplicativeExpression(context.multiplicativeExpression()[0]);
                }

                var startAdd = this.VisitMultiplicativeExpression(context.multiplicativeExpression()[0]);
                for (int i = 1; i < context.multiplicativeExpression().Length; i++)
                {
                    switch (context.addOperator()[i - 1].Start.Text)
                    {
                        case "+":
                            startAdd = startAdd.Add(this.VisitMultiplicativeExpression(context.multiplicativeExpression()[i]));
                            break;
                        case "-":
                            startAdd = startAdd.Add(this.VisitMultiplicativeExpression(context.multiplicativeExpression()[i]).Not());
                            break;
                    }
                }

                return startAdd;
            }

            public override Result VisitMultiplicativeExpression([NotNull] OclParser.MultiplicativeExpressionContext context)
            {
                if (context.multiplyOperator() == null || context.multiplyOperator().Length == 0)
                {
                    return this.VisitUnaryExpression(context.unaryExpression()[0]);
                }

                Result startMul = this.VisitUnaryExpression(context.unaryExpression()[0]);
                for (int i = 1; i < context.unaryExpression().Length; i++)
                {
                    switch (context.multiplyOperator()[i - 1].Start.Text)
                    {
                        case "*":
                            startMul = startMul.Multiply(this.VisitUnaryExpression(context.unaryExpression()[i]));
                            break;
                        case "/":
                            startMul = startMul.Divide(this.VisitUnaryExpression(context.unaryExpression()[i]));
                            break;
                    }
                }

                return startMul;
            }

            public override Result VisitUnaryExpression([NotNull] OclParser.UnaryExpressionContext context)
            {
                Result postfix = this.VisitPostfixExpression(context.postfixExpression());
                if (context.unaryOperator() != null)
                {
                    return postfix.Not();
                }

                return postfix;
            }

            public override Result VisitPostfixExpression([NotNull] OclParser.PostfixExpressionContext context)
            {
                if (context.propertyCall() == null || context.propertyCall().Length == 0)
                {
                    this.GlobalResult = this.VisitPrimaryExpression(context.primaryExpression());
                }
                else
                {
                    this.GlobalResult = this.VisitPrimaryExpression(context.primaryExpression());
                    for (int i = 0; i < context.propertyCall().Length; i++)
                    {
                        this.GlobalResult = this.VisitPropertyCall(context.propertyCall()[i]);
                    }
                }

                return this.GlobalResult;
            }

            public override Result VisitLiteral([NotNull] OclParser.LiteralContext context)
            {
                if (context.NUMBER() != null)
                {
                    return new DoubleResult(double.Parse(context.NUMBER().GetText()));
                }
                else if (context.stringLiteral() != null)
                {
                    return new StringResult(context.stringLiteral().NAME()?.GetText() ?? string.Empty);
                }
                else if (context.booleanLiteral() != null)
                {
                    return new BoolResult(context.booleanLiteral().GetText() == "true");
                }

                return null;
            }

            public override Result VisitLiteralCollection([NotNull] OclParser.LiteralCollectionContext context)
            {
                switch (context.collectionKind().GetText())
                {
                    case "Set":
                        return new CollectionResult(new HashSet<Result>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0]))));
                    case "OrderedSet":
                        return new CollectionResult(new SortedSet<Result>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0]))));
                    case "Bag":
                        return new CollectionResult(new LinkedList<Result>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0]))));
                    default:
                        return new CollectionResult(new LinkedList<Result>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0]))));
                }
            }

            public override Result VisitPropertyCall([NotNull] OclParser.PropertyCallContext context)
            {
                if (context.propertyCallParameters() != null)
                {
                    if (context.pathName().GetText() == "size")
                    {
                        switch (this.GlobalResult)
                        {
                            case CollectionResult objects:
                                return new IntResult(objects.Count());
                            case StringResult s:
                                return new IntResult(s.GetValue().Length);
                        }
                    }
                    else if (context.pathName().GetText() == "allInstances")
                    {
                        IElement element = this.Model.FindElement(this.GlobalResult.ToString());
                        return new CollectionResult(this.Model.Elements.Where(x => x.Class == element).ToList<object>());
                    }
                    else if (context.pathName().GetText() == "any")
                    {
                        var varStack = new Dictionary<string, Result>();
                        this.vars.Add(varStack);
                        Result returnValue = null;
                        foreach (var val in (CollectionResult)this.GlobalResult)
                        {
                            varStack["self"] = val;
                            if (this.interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                returnValue = val;
                                break;
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return returnValue;
                    }
                    else if (context.pathName().GetText() == "forAll")
                    {
                        var varStack = new Dictionary<string, Result>();
                        this.vars.Add(varStack);
                        foreach (var val in (CollectionResult)this.GlobalResult)
                        {
                            varStack["self"] = val;
                            if (!this.interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                return new BoolResult(false);
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return new BoolResult(true);
                    }
                    else if (context.pathName().GetText() == "collect")
                    {
                        CollectionResult newElements = (CollectionResult)this.GlobalResult;

                        foreach (Result val in newElements.ToList())
                        {
                            this.GlobalResult = val;
                            newElements.Remove(val);
                            newElements.Add(this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]));
                        }

                        return newElements;
                    }
                    else if (context.pathName().GetText() == "select")
                    {
                        CollectionResult filteredElements = (CollectionResult)this.GlobalResult;

                        Dictionary<string, Result> varsStack = new Dictionary<string, Result>();
                        this.vars.Add(varsStack);
                        foreach (Result val in filteredElements.ToList())
                        {
                            varsStack["self"] = val;
                            if (!this.interpreter.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                filteredElements.Remove(val);
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return filteredElements;
                    }

                    Dictionary<string, Result> stack = new Dictionary<string, Result>();
                    List<string> names = this.funcs[context.pathName().GetText()].Param;
                    OclParser.ExpressionContext contextFunc = this.funcs[context.pathName().GetText()].Context;
                    for (int i = 0; i < names.Count; i++)
                    {
                        stack[names[i]] = this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[i]);
                    }

                    this.vars.Add(stack);
                    Result result = this.VisitExpression(contextFunc);
                    this.vars.RemoveAt(this.vars.Count - 1);
                    return result;
                }
                else if (context.Parent is OclParser.PostfixExpressionContext expressionContext)
                {
                    string element = expressionContext.primaryExpression().GetText();
                    if (element != "self")
                    {
                        this.Element = this.Model.FindElement(element);
                    }

                    if (context.NUMBER() != null)
                    {
                        IElement parent = this.Element;
                        for (int i = 0; i < this.Depth - int.Parse(context.NUMBER().GetText()); i++)
                        {
                            parent = parent.Class;
                        }

                        this.Element = parent;
                    }

                    return new StringResult(this.Element.Attributes.First(x => x.Name == context.pathName().GetText()).StringValue);
                }

                return this.VisitPathName(context.pathName());
            }

            public override Result VisitPathName([NotNull] OclParser.PathNameContext context)
            {
                for (int i = this.vars.Count - 1; i >= 0; i--)
                {
                    if (this.vars[i].ContainsKey(context.NAME()[0].GetText()))
                    {
                        return this.vars[i][context.NAME()[0].GetText()];
                    }
                }

                return new StringResult(context.NAME()[0].GetText());
            }

            public override Result VisitIfExpression([NotNull] OclParser.IfExpressionContext context)
            {
                if (this.interpreter.VisitExpression(context.expression()[0]))
                {
                    return this.VisitExpression(context.expression()[1]);
                }
                else
                {
                    return this.VisitExpression(context.expression()[2]);
                }
            }
        }
    }
}
