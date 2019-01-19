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
        private readonly OclCalc calc;
        private readonly Dictionary<string, FunctionDefinition> funcs;
        private readonly IConsole console;
        private readonly IRepo repository;
        private Repo.IModel model;
        private IElement curElement;

        public OclInterpreter(IConsole console, IRepo repo)
        {
            this.vars = new ArrayList<Dictionary<string, Result>>
            {
                new Dictionary<string, Result>()
            };
            this.funcs = new Dictionary<string, FunctionDefinition>();
            this.calc = new OclCalc(this.vars, this.funcs, this, repo, console);
            this.console = console;
            this.repository = repo;
        }

        public override bool VisitPackageName([NotNull] OclParser.PackageNameContext context)
        {
            this.model = this.repository.Model(context.pathName().GetText());
            this.calc.Model = this.model;
            return this.VisitPathName(context.pathName());
        }

        public override bool VisitConstraint([NotNull] OclParser.ConstraintContext context)
        {
            this.VisitContextDeclaration(context.contextDeclaration());
            string text = context.contextDeclaration().classifierContext().GetText();
            this.curElement = this.model.FindElement(text);
            this.calc.Element = this.curElement;
            for (int i = 0; i < context.oclExpression().Length; i++)
            {
                this.calc.Depth = int.Parse(context.NUMBER()[i].GetText());
                foreach (IElement el in this.model.Elements)
                {
                    IElement par = el;
                    for (int j = 0; j < this.calc.Depth; j++)
                    {
                        par = par.Class;
                    }

                    if (par == this.curElement)
                    {
                        IElement tcur = this.curElement;
                        this.curElement = par;
                        this.VisitOclExpression(context.oclExpression()[i]);
                        this.curElement = tcur;
                    }
                }

                this.calc.Depth = 0;
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
                this.console.SendMessage("err");
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
            Result result = (Result)this.calc.VisitRelationalExpression(context.relationalExpression()[0]);

            for (int i = 1; i < context.relationalExpression().Length; i++)
            {
                var leftObj = result;
                var rightObj = this.calc.VisitRelationalExpression(context.relationalExpression()[i]);

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

        private class OclCalc : OclBaseVisitor<Result>
        {
            private readonly ArrayList<Dictionary<string, Result>> vars = null;
            private readonly Dictionary<string, FunctionDefinition> funcs = null;
            private readonly OclInterpreter hp = null;
            private readonly IRepo repository = null;
            private readonly IConsole console = null;

            public int Depth { internal get; set; } = 0;

            public Repo.IModel Model { private get; set; } = null;

            public IElement Element { private get; set; } = null;

            public Result Res { get; private set; } = null;

            public OclCalc(ArrayList<Dictionary<string, Result>> vars, Dictionary<string, FunctionDefinition> funcs, OclInterpreter hp, IRepo repo, IConsole cons)
            {
                this.vars = vars;
                this.funcs = funcs;
                this.hp = hp;
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
                    this.Res = this.VisitPrimaryExpression(context.primaryExpression());
                }
                else
                {
                    this.Res = this.VisitPrimaryExpression(context.primaryExpression());
                    for (int i = 0; i < context.propertyCall().Length; i++)
                    {
                        this.Res = this.VisitPropertyCall(context.propertyCall()[i]);
                    }
                }

                return this.Res;
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
                        switch (this.Res)
                        {
                            case CollectionResult objects:
                                return new IntResult(objects.Count());
                            case StringResult s:
                                return new IntResult(s.GetValue().Length);
                        }
                    }
                    else if (context.pathName().GetText() == "allInstances")
                    {
                        IElement elem = this.Model.FindElement(this.Res.ToString());
                        return new CollectionResult(this.Model.Elements.Where(x => x.Class == elem).ToList<object>());
                    }
                    else if (context.pathName().GetText() == "any")
                    {
                        var st = new Dictionary<string, Result>();
                        this.vars.Add(st);
                        Result ret = null;
                        foreach (var val in (CollectionResult)this.Res)
                        {
                            st["self"] = val;
                            if (this.hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                ret = val;
                                break;
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return ret;
                    }
                    else if (context.pathName().GetText() == "forAll")
                    {
                        var st = new Dictionary<string, Result>();
                        this.vars.Add(st);
                        foreach (var val in (CollectionResult)this.Res)
                        {
                            st["self"] = val;
                            if (!this.hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                return new BoolResult(false);
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return new BoolResult(true);
                    }
                    else if (context.pathName().GetText() == "collect")
                    {
                        CollectionResult ar = (CollectionResult)this.Res;

                        foreach (Result val in ar.ToList())
                        {
                            this.Res = val;
                            ar.Remove(val);
                            ar.Add(this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]));
                        }

                        return ar;
                    }
                    else if (context.pathName().GetText() == "select")
                    {
                        CollectionResult ar = (CollectionResult)this.Res;

                        Dictionary<string, Result> st = new Dictionary<string, Result>();
                        this.vars.Add(st);
                        foreach (Result val in ar.ToList())
                        {
                            st["self"] = val;
                            if (!this.hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                ar.Remove(val);
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return ar;
                    }

                    Dictionary<string, Result> stack = new Dictionary<string, Result>();
                    List<string> names = this.funcs[context.pathName().GetText()].Param;
                    OclParser.ExpressionContext contextFunc = this.funcs[context.pathName().GetText()].Context;
                    for (int i = 0; i < names.Count; i++)
                    {
                        stack[names[i]] = this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[i]);
                    }

                    this.vars.Add(stack);
                    Result ress = this.VisitExpression(contextFunc);
                    this.vars.RemoveAt(this.vars.Count - 1);
                    return ress;
                }
                else if (context.Parent is OclParser.PostfixExpressionContext expressionContext)
                {
                    string elem = expressionContext.primaryExpression().GetText();
                    if (elem != "self")
                    {
                        this.Element = this.Model.FindElement(elem);
                    }

                    if (context.NUMBER() != null)
                    {
                        IElement par = this.Element;
                        for (int i = 0; i < this.Depth - int.Parse(context.NUMBER().GetText()); i++)
                        {
                            par = par.Class;
                        }

                        this.Element = par;
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
                if (this.hp.VisitExpression(context.expression()[0]))
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
