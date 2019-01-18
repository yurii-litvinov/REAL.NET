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

    internal class OclPrinter : OclBaseVisitor<bool>
    {
        private readonly ArrayList<Dictionary<string, object>> vars;
        private readonly OclCalc calc;
        private readonly Dictionary<string, FunctionDef> funcs;
        private readonly IConsole console;
        private readonly IRepo repository;
        private Repo.IModel model;
        private IElement curElement;

        public OclPrinter(IConsole console, IRepo repo)
        {
            this.vars = new ArrayList<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
            };
            this.funcs = new Dictionary<string, FunctionDef>();
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

        public override bool VisitLetExpression([NotNull] OclParser.LetExpressionContext context)
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
            object result = this.calc.VisitRelationalExpression(context.relationalExpression()[0]);
            for (int i = 1; i < context.relationalExpression().Length; i++)
            {
                dynamic left = 0, right = 0;
                var leftObj = result;
                var rightObj = this.calc.VisitRelationalExpression(context.relationalExpression()[i]);

                if (leftObj is int j)
                {
                    left = j;
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

                switch (context.eqOperator()[i - 1].GetText())
                {
                    case "=":
                        result = left == right;
                        break;
                    default:
                        result = left != right;
                        break;
                }
            }

            return (bool)result;
        }

        private class OclCalc : OclBaseVisitor<object>
        {
            private readonly ArrayList<Dictionary<string, object>> vars = null;
            private readonly Dictionary<string, FunctionDef> funcs = null;
            private readonly OclPrinter hp = null;
            private readonly IRepo repository = null;
            private readonly IConsole console = null;

            public int Depth { internal get; set; } = 0;

            public Repo.IModel Model { private get; set; } = null;

            public IElement Element { private get; set; } = null;

            public object Res { get; private set; } = null;

            public OclCalc(ArrayList<Dictionary<string, object>> vars, Dictionary<string, FunctionDef> funcs, OclPrinter hp, IRepo repo, IConsole cons)
            {
                this.vars = vars;
                this.funcs = funcs;
                this.hp = hp;
                this.repository = repo;
                this.console = cons;
            }

            public override object VisitRelationalExpression([NotNull] OclParser.RelationalExpressionContext context)
            {
                object result = this.VisitAdditiveExpression(context.additiveExpression()[0]);

                for (int i = 1; i < context.additiveExpression().Length; i++)
                {
                    dynamic left = 0, right = 0;
                    var leftObj = result;
                    var rightObj = this.VisitAdditiveExpression(context.additiveExpression()[i]);

                    if (leftObj is int j)
                    {
                        left = j;
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
                        case "<":
                            result = left < right;
                            break;
                        case ">":
                            result = left > right;
                            break;
                    }
                }

                return result;
            }

            public override object VisitAdditiveExpression([NotNull] OclParser.AdditiveExpressionContext context)
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

            public override object VisitMultiplicativeExpression([NotNull] OclParser.MultiplicativeExpressionContext context)
            {
                if (context.multiplyOperator() == null || context.multiplyOperator().Length == 0)
                {
                    return this.VisitUnaryExpression(context.unaryExpression()[0]);
                }

                double startMul = (double)this.VisitUnaryExpression(context.unaryExpression()[0]);
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

            public override object VisitUnaryExpression([NotNull] OclParser.UnaryExpressionContext context)
            {
                object postfix = this.VisitPostfixExpression(context.postfixExpression());
                if (context.unaryOperator() != null)
                {
                    switch (context.unaryOperator().Start.Text)
                    {
                        case "-":
                            return -(double)postfix;
                        case "not":
                            return !(bool)postfix;
                    }
                }

                return postfix;
            }

            public override object VisitPostfixExpression([NotNull] OclParser.PostfixExpressionContext context)
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

            public override object VisitLiteral([NotNull] OclParser.LiteralContext context)
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

            public override object VisitLiteralCollection([NotNull] OclParser.LiteralCollectionContext context)
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

            public override object VisitPropertyCall([NotNull] OclParser.PropertyCallContext context)
            {
                if (context.propertyCallParameters() != null)
                {
                    if (context.pathName().GetText() == "size")
                    {
                        if (this.Res is ICollection<object> objects)
                        {
                            return objects.Count;
                        }
                        else if (this.Res is string s)
                        {
                            return s.Length;
                        }
                    }
                    else if (context.pathName().GetText() == "allInstances")
                    {
                        IElement elem = this.Model.FindElement(this.Res.ToString());
                        return this.Model.Elements.Where(x => x.Class == elem).ToList<object>();
                    }
                    else if (context.pathName().GetText() == "any")
                    {
                        Dictionary<string, object> st = new Dictionary<string, object>();
                        this.vars.Add(st);
                        object ret = null;
                        foreach (object val in (ICollection<object>)this.Res)
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
                        foreach (object val in (ICollection<object>)this.Res)
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
                        if (this.Res is HashSet<object>)
                        {
                            ar = new HashSet<object>();
                        }
                        else if (this.Res is SortedSet<object>)
                        {
                            ar = new SortedSet<object>();
                        }
                        else if (this.Res is LinkedList<object>)
                        {
                            ar = new LinkedList<object>();
                        }

                        foreach (object val in (ICollection<object>)this.Res)
                        {
                            this.Res = val;
                            ar.Add(this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]));
                        }

                        return ar;
                    }
                    else if (context.pathName().GetText() == "select")
                    {
                        ICollection<object> ar = null;
                        if (this.Res is HashSet<object>)
                        {
                            ar = new HashSet<object>();
                        }
                        else if (this.Res is SortedSet<object>)
                        {
                            ar = new SortedSet<object>();
                        }
                        else if (this.Res is LinkedList<object>)
                        {
                            ar = new LinkedList<object>();
                        }

                        Dictionary<string, object> st = new Dictionary<string, object>();
                        this.vars.Add(st);
                        foreach (object val in (ICollection<object>)this.Res)
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
                    OclParser.ExpressionContext contextFunc = this.funcs[context.pathName().GetText()].context;
                    for (int i = 0; i < names.Count; i++)
                    {
                        stack[names[i]] = this.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[i]);
                    }

                    this.vars.Add(stack);
                    object ress = this.VisitExpression(contextFunc);
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

                    return this.Element.Attributes.First(x => x.Name == context.pathName().GetText()).StringValue;
                }

                return this.VisitPathName(context.pathName());
            }

            public override object VisitPathName([NotNull] OclParser.PathNameContext context)
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

            public override object VisitIfExpression([NotNull] OclParser.IfExpressionContext context)
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
