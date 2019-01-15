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

    internal class HelloPrinter : HelloBaseVisitor<bool>
    {
        private readonly ArrayList<Dictionary<string, object>> vars;
        private readonly HelloCalc calc;
        private readonly Dictionary<string, FunctionDef> funcs;
        private readonly IConsole console;
        private readonly IRepo repository;
        private Repo.IModel model;
        private IElement curElement;

        public HelloPrinter(IConsole console, IRepo repo)
        {
            this.vars = new ArrayList<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
            };
            this.funcs = new Dictionary<string, FunctionDef>();
            this.calc = new HelloCalc(this.vars, this.funcs, this, repo, console);
            this.console = console;
            this.repository = repo;
        }

        public override bool VisitPackageName([NotNull] HelloParser.PackageNameContext context)
        {
            this.model = this.repository.Model(context.pathName().GetText());
            this.calc.Model = this.model;
            return this.VisitPathName(context.pathName());
        }

        public override bool VisitConstraint([NotNull] HelloParser.ConstraintContext context)
        {
            VisitContextDeclaration(context.contextDeclaration());
            string text = context.contextDeclaration().classifierContext().GetText();
            this.curElement = this.model.FindElement(text);
            this.calc.Element = this.curElement;
            for (int i = 0; i < context.oclExpression().Length; i++)
            {
                this.calc.depth = int.Parse(context.NUMBER()[i].GetText());
                foreach (IElement el in this.model.Elements)
                {
                    IElement par = el;
                    for (int j = 0; j < this.calc.depth; j++)
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

                this.calc.depth = 0;
            }
            return base.VisitConstraint(context);
        }

        public override bool VisitOclExpression([NotNull] HelloParser.OclExpressionContext context)
        {
            for (int i = 0; i < context.letExpression().Length; i++)
            {
                this.VisitLetExpression(context.letExpression()[i]);
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

        public override bool VisitLetExpression([NotNull] HelloParser.LetExpressionContext context)
        {
            if (context.formalParameterList() != null)
            {
                this.funcs[context.NAME().GetText()] = new FunctionDef { param = context.formalParameterList().NAME().Select(x => x.GetText()).ToList(), context = context.expression() };
            }
            else
            {
                this.vars[this.vars.Count - 1][context.NAME().GetText()] = context.expression().GetText();
            }

            return true;
        }


        public override bool VisitRelationalExpression([NotNull] HelloParser.RelationalExpressionContext context)
        {
            if (context.relationalOperator() != null)
            {
                dynamic left = 0, right = 0;
                var leftObj = this.calc.VisitAdditiveExpression(context.additiveExpression()[0]);
                var rightObj = this.calc.VisitAdditiveExpression(context.additiveExpression()[1]);

                if (leftObj is int i)
                {
                    left = i;
                }
                else if (leftObj is double d)
                {
                    left = d;
                }
                else if (leftObj is string s)
                {
                    left = s;
                }
                else if (leftObj is bool obj)
                {
                    left = obj;
                }

                if (rightObj is int obj1)
                {
                    right = obj1;
                }
                else if (rightObj is double obj)
                {
                    right = obj;
                }
                else if (rightObj is string s)
                {
                    right = s;
                }
                else if (rightObj is bool b)
                {
                    right = b;
                }

                switch (context.relationalOperator().Start.Text)
                {
                    case "=":
                        return left == right;
                    case "<":
                        return left < right;
                    case ">":
                        return left > right;
                }
            }

            return (bool) this.calc.VisitAdditiveExpression(context.additiveExpression()[0]);
        }

        private class HelloCalc : HelloBaseVisitor<object>
        {
            private readonly ArrayList<Dictionary<string, object>> vars = null;
            private readonly Dictionary<string, FunctionDef> funcs = null;
            private readonly HelloPrinter hp = null;
            private readonly IRepo repository = null;
            private readonly IConsole console = null;
            private readonly int depth = 0;

            public Repo.IModel Model { private get; set; } = null;
            public IElement Element { private get; set; } = null;
            private object res = null;

            public HelloCalc(ArrayList<Dictionary<string, object>> vars, Dictionary<string, FunctionDef> funcs, HelloPrinter hp, IRepo repo, IConsole cons)
            {
                this.vars = vars;
                this.funcs = funcs;
                this.hp = hp;
                this.repository = repo;
                this.console = cons;
            }

            public override object VisitAdditiveExpression([NotNull] HelloParser.AdditiveExpressionContext context)
            {
                if (context.addOperator() == null || context.addOperator().Length == 0)
                {
                    return this.VisitMultiplicativeExpression(context.multiplicativeExpression()[0]);
                }

                var startAdd = (double)this.VisitMultiplicativeExpression(context.multiplicativeExpression()[0]);
                for (int i = 1; i < context.multiplicativeExpression().Length; i++)
                {
                    switch (context.addOperator()[i - 1].Start.Text)
                    {
                        case "+":
                            startAdd += (double)this.VisitMultiplicativeExpression(context.multiplicativeExpression()[i]);
                            break;
                        case "-":
                            startAdd -= (double)this.VisitMultiplicativeExpression(context.multiplicativeExpression()[i]);
                            break;
                    }
                }

                return startAdd;
            }

            public override object VisitMultiplicativeExpression([NotNull] HelloParser.MultiplicativeExpressionContext context)
            {
                if (context.multiplyOperator() == null || context.multiplyOperator().Length == 0)
                {
                    return this.VisitUnaryExpression(context.unaryExpression()[0]);
                }

                double startMul = (double) this.VisitUnaryExpression(context.unaryExpression()[0]);
                for (int i = 1; i < context.unaryExpression().Length; i++)
                {
                    switch (context.multiplyOperator()[i - 1].Start.Text)
                    {
                        case "*":
                            startMul *= (double)this.VisitUnaryExpression(context.unaryExpression()[i]);
                            break;
                        case "/":
                            startMul /= (double)this.VisitUnaryExpression(context.unaryExpression()[i]);
                            break;
                    }
                }
                return startMul;
            }

            public override object VisitUnaryExpression([NotNull] HelloParser.UnaryExpressionContext context)
            {
                object postfix = this.VisitPostfixExpression(context.postfixExpression());
                if (context.unaryOperator() != null)
                {
                    switch (context.unaryOperator().Start.Text)
                    {
                        case "-":
                            return -(double)postfix;
                    }
                }

                return postfix;
            }

            public override object VisitPostfixExpression([NotNull] HelloParser.PostfixExpressionContext context)
            {
                if (context.propertyCall() == null || context.propertyCall().Length == 0)
                {
                    this.res = this.VisitPrimaryExpression(context.primaryExpression());
                }
                else
                {
                    this.res = this.VisitPrimaryExpression(context.primaryExpression());
                    for (int i = 0; i < context.propertyCall().Length; i++)
                    {
                        this.res = this.VisitPropertyCall(context.propertyCall()[i]);
                    }
                }

                return this.res;
            }

            public override object VisitLiteral([NotNull] HelloParser.LiteralContext context)
            {
                if (context.NUMBER() != null)
                {
                    return double.Parse(context.NUMBER().GetText());
                }
                else if (context.stringLiteral() != null)
                {
                    return context.stringLiteral().NAME() != null ? context.stringLiteral().NAME().GetText() : string.Empty;
                }
                else if (context.booleanLiteral() != null)
                {
                    return context.booleanLiteral().GetText() == "true";
                }

                return null;
            }

            public override object VisitLiteralCollection([NotNull] HelloParser.LiteralCollectionContext context)
            {
                switch (context.collectionKind().GetText())
                {
                    case "Set":
                        return new HashSet<object>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0])));
                    case "OrderedSet":
                        return new SortedSet<object>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0])));
                    case "Bag":
                        return new LinkedList<object>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0])));
                    case "Sequence":
                        return new LinkedList<object>(context.collectionItem().Select(x => this.VisitExpression(x.expression()[0])));
                }

                return new HashSet<object>();
            }

            public override object VisitPropertyCall([NotNull] HelloParser.PropertyCallContext context)
            {
                if (context.propertyCallParameters() != null)
                {
                    if (context.pathName().GetText() == "size")
                    {
                        if (this.res is ICollection<object> objects)
                        {
                            return objects.Count;
                        }
                        else if (this.res is string s)
                        {
                            return s.Length;
                        }
                    }
                    else if (context.pathName().GetText() == "allInstances")
                    {
                        IElement elem = this.Model.FindElement(this.res.ToString());
                        return this.Model.Elements.Where(x => x.Class == elem).ToList<object>();
                    }
                    else if (context.pathName().GetText() == "any")
                    {
                        Dictionary<string, object> st = new Dictionary<string, object>();
                        this.vars.Add(st);
                        object ret = null;
                        foreach (object val in (ICollection<object>)this.res)
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
                        Dictionary<string, object> st = new Dictionary<string, object>();
                        this.vars.Add(st);
                        foreach (object val in (ICollection<object>)this.res)
                        {
                            st["self"] = val;
                            if (!this.hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                return false;
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return true;
                    }
                    else if (context.pathName().GetText() == "collect")
                    {
                        ICollection<object> ar = null;
                        if (this.res is HashSet<object>)
                        {
                            ar = new HashSet<object>();
                        }
                        else if (this.res is SortedSet<object>)
                        {
                            ar = new SortedSet<object>();
                        }
                        else if (this.res is LinkedList<object>)
                        {
                            ar = new LinkedList<object>();
                        }

                        foreach (object val in (ICollection<object>)this.res)
                        {
                            this.res = val;
                            ar.Add(this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]));
                        }

                        return ar;
                    }
                    else if (context.pathName().GetText() == "select")
                    {
                        ICollection<object> ar = null;
                        if (this.res is HashSet<object>)
                        {
                            ar = new HashSet<object>();
                        }
                        else if (this.res is SortedSet<object>)
                        {
                            ar = new SortedSet<object>();
                        }
                        else if (this.res is LinkedList<object>)
                        {
                            ar = new LinkedList<object>();
                        }

                        Dictionary<string, object> st = new Dictionary<string, object>();
                        this.vars.Add(st);
                        foreach (object val in (ICollection<object>)this.res)
                        {
                            st["self"] = val;
                            if (this.hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                ar.Add(val);
                            }
                        }

                        this.vars.RemoveAt(this.vars.Count - 1);
                        return ar;
                    }

                    Dictionary<string, object> stack = new Dictionary<string, object>();
                    List<string> names = this.funcs[context.pathName().GetText()].param;
                    HelloParser.ExpressionContext contextFunc = this.funcs[context.pathName().GetText()].context;
                    for (int i = 0; i < names.Count; i++)
                    {
                        stack[names[i]] = this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[i]);
                    }

                    this.vars.Add(stack);
                    object ress = this.VisitExpression(contextFunc);
                    this.vars.RemoveAt(this.vars.Count - 1);
                    return ress;
                }
                else if (context.Parent is HelloParser.PostfixExpressionContext expressionContext)
                {
                    string elem = expressionContext.primaryExpression().GetText();
                    if (elem != "self")
                    {
                        this.Element = this.Model.FindElement(elem);
                    }

                    if (context.NUMBER() != null)
                    {
                        IElement par = this.Element;
                        for (int i = 0; i < this.depth - int.Parse(context.NUMBER().GetText()); i++)
                        {
                            par = par.Class;
                        }

                        this.Element = par;
                    }

                    return this.Element.Attributes.First(x => x.Name == context.pathName().GetText()).StringValue;
                }

                return this.VisitPathName(context.pathName());
            }

            public override object VisitPathName([NotNull] HelloParser.PathNameContext context)
            {
                for (int i = this.vars.Count - 1; i >= 0; i--)
                {
                    if (this.vars[i].ContainsKey(context.NAME()[0].GetText()))
                    {
                        return this.vars[i][context.NAME()[0].GetText()];
                    }
                }

                return context.NAME()[0].GetText();
            }

            public override object VisitIfExpression([NotNull] HelloParser.IfExpressionContext context)
            {
                if (hp.VisitExpression(context.expression()[0]))
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
