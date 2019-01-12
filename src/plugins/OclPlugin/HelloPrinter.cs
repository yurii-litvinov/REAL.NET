using Antlr4.Runtime.Misc;
using EditorPluginInterfaces;
using Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
    class HelloPrinter : HelloBaseVisitor<bool>
    {
        ArrayList<Dictionary<string, object>> vars;
        HelloCalc calc;
        Dictionary<string, FunctionDef> funcs;
        IConsole console;
        IRepo repository;
        Repo.IModel model;
        IElement curElement;

        public HelloPrinter(IConsole console, IRepo repo)
        {
            vars = new ArrayList<Dictionary<string, object>>();
            vars.Add(new Dictionary<string, object>());
            funcs = new Dictionary<string, FunctionDef>();
            calc = new HelloCalc(vars, funcs, this, repo, console);
            this.console = console;
            this.repository = repo;
        }

        public override bool VisitPackageName([NotNull] HelloParser.PackageNameContext context)
        {
            model = repository.Model(context.pathName().GetText());
            calc.Model = model;
            return base.VisitPathName(context.pathName());
        }

        public override bool VisitConstraint([NotNull] HelloParser.ConstraintContext context)
        {
            VisitContextDeclaration(context.contextDeclaration());
            string text = context.contextDeclaration().classifierContext().GetText();
            curElement = model.FindElement(text);
            calc.Element = curElement;
            for (int i = 0; i < context.oclExpression().Length; i++)
            {
                calc.Depth = Int32.Parse(context.NUMBER()[i].GetText());
                foreach(IElement el in model.Elements)
                {
                    IElement par = el;
                    for(int j = 0; j < calc.Depth; j++)
                    {
                        par = par.Class;
                    }
                    if(par == curElement)
                    {
                        IElement tcur = curElement;
                        curElement = par;
                        VisitOclExpression(context.oclExpression()[i]);
                        curElement = tcur;
                    }
                }
                calc.Depth = 0;
            }
            return base.VisitConstraint(context);
        }

        public override bool VisitOclExpression([NotNull] HelloParser.OclExpressionContext context)
        {
            for (int i = 0; i < context.letExpression().Length; i++)
            {
                VisitLetExpression(context.letExpression()[i]);
            }
            
            if (!VisitExpression(context.expression()))
            {
                console.SendMessage("err");
            }
            else
            {
                console.SendMessage("ok");
            }
            
            return true;
        }
        public override bool VisitLetExpression([NotNull] HelloParser.LetExpressionContext context)
        {
            if (context.formalParameterList() != null)
                funcs[context.NAME().GetText()] = new FunctionDef { param = context.formalParameterList().NAME().Select(x => x.GetText()).ToList(), context = context.expression() };
            else
                vars[vars.Count - 1][context.NAME().GetText()] = context.expression().GetText();

            return true;
        }


        public override bool VisitRelationalExpression([NotNull] HelloParser.RelationalExpressionContext context)
        {
            if (context.relationalOperator() != null)
            {
                dynamic left = 0, right = 0;
                object leftObj = calc.VisitAdditiveExpression(context.additiveExpression()[0]);
                object rightObj = calc.VisitAdditiveExpression(context.additiveExpression()[1]);

                if(leftObj is int)
                {
                    left = (int)leftObj;
                }
                else if(leftObj is double)
                {
                    left = (double)leftObj;
                }
                else if(leftObj is string)
                {
                    left = (string)leftObj;
                }
                else if (leftObj is bool)
                {
                    left = (bool)leftObj;
                }

                if (rightObj is int)
                {
                    right = (int)rightObj;
                }
                else if (rightObj is double)
                {
                    right = (double)rightObj;
                }
                else if (rightObj is string)
                {
                    right = (string)rightObj;
                }
                else if (rightObj is bool)
                {
                    right = (bool)rightObj;
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
            return (bool) calc.VisitAdditiveExpression(context.additiveExpression()[0]);
        }

        class HelloCalc : HelloBaseVisitor<object>
        {
            ArrayList<Dictionary<string, object>> vars = null;
            Dictionary<string, FunctionDef> funcs = null;
            HelloPrinter hp = null;
            IRepo repository = null;
            IConsole console = null;
            public int Depth = 0;

            public Repo.IModel Model { get; set; } = null;
            public IElement Element { get; set; } = null;
            object res = null;

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
                    return VisitMultiplicativeExpression(context.multiplicativeExpression()[0]);
                }
                double startAdd = (double) VisitMultiplicativeExpression(context.multiplicativeExpression()[0]);
                for (int i = 1; i < context.multiplicativeExpression().Length; i++)
                {
                    switch (context.addOperator()[i - 1].Start.Text)
                    {
                        case "+":
                            startAdd += (double) VisitMultiplicativeExpression(context.multiplicativeExpression()[i]);
                            break;
                        case "-":
                            startAdd -= (double) VisitMultiplicativeExpression(context.multiplicativeExpression()[i]);
                            break;
                    }
                }
                return startAdd;
            }

            public override object VisitMultiplicativeExpression([NotNull] HelloParser.MultiplicativeExpressionContext context)
            {
                if(context.multiplyOperator() == null || context.multiplyOperator().Length == 0)
                {
                    return VisitUnaryExpression(context.unaryExpression()[0]);
                }
                double startMul = (double) VisitUnaryExpression(context.unaryExpression()[0]);
                for (int i = 1; i < context.unaryExpression().Length; i++)
                {
                    switch (context.multiplyOperator()[i - 1].Start.Text)
                    {
                        case "*":
                            startMul *= (double) VisitUnaryExpression(context.unaryExpression()[i]);
                            break;
                        case "/":
                            startMul /= (double) VisitUnaryExpression(context.unaryExpression()[i]);
                            break;
                    }
                }
                return startMul;
            }

            public override object VisitUnaryExpression([NotNull] HelloParser.UnaryExpressionContext context)
            {
                object postfix = VisitPostfixExpression(context.postfixExpression());
                if (context.unaryOperator() != null)
                {
                    switch (context.unaryOperator().Start.Text)
                    {
                        case "-":
                            return -(double) postfix;
                    }
                }
                return postfix;
            }

            public override object VisitPostfixExpression([NotNull] HelloParser.PostfixExpressionContext context)
            {
                if (context.propertyCall() == null || context.propertyCall().Length == 0)
                    res = VisitPrimaryExpression(context.primaryExpression());
                else {
                    res = VisitPrimaryExpression(context.primaryExpression());
                    for (int i = 0; i < context.propertyCall().Length; i++) {
                        res = VisitPropertyCall(context.propertyCall()[i]);
                    }
                }

                return res;
            }

            /**public override double VisitPrimaryExpression([NotNull] HelloParser.PrimaryExpressionContext context)
            {
                double res = 0;
                if (context.literal() != null)
                {
                    res = VisitLiteral(context.literal());
                }
                else if (context.propertyCall() != null)
                {
                    res = VisitPropertyCall(context.propertyCall());
                }
                else if (context.ifExpression() != null)
                {
                    res = VisitIfExpression(context.ifExpression());
                }

                return res;
            }**/

            public override object VisitLiteral([NotNull] HelloParser.LiteralContext context)
            {
                if (context.NUMBER() != null)
                {
                    return Double.Parse(context.NUMBER().GetText());
                }
                else if(context.stringLiteral() != null)
                {
                    return context.stringLiteral().NAME().GetText();
                }
                else if(context.booleanLiteral() != null)
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
                        return new HashSet<object>(context.collectionItem().Select(x => VisitExpression(x.expression()[0])));
                    case "OrderedSet":
                        return new SortedSet<object>(context.collectionItem().Select(x => VisitExpression(x.expression()[0])));
                    case "Bag":
                        return new LinkedList<object>(context.collectionItem().Select(x => VisitExpression(x.expression()[0])));
                    case "Sequence":
                        return new LinkedList<object>(context.collectionItem().Select(x => VisitExpression(x.expression()[0])));
                }
                return new HashSet<object>();
            }

            public override object VisitPropertyCall([NotNull] HelloParser.PropertyCallContext context)
            {
                if (context.propertyCallParameters() != null)
                {
                    if(context.pathName().GetText() == "size")
                    {
                        if (res is ICollection<object>)
                        {
                            return ((ICollection<object>) res).Count;
                        }
                        else if (res is string)
                        {
                            return ((string) res).Length;
                        }
                    }
                    else if(context.pathName().GetText() == "allInstances")
                    {
                        IElement elem = Model.FindElement(res.ToString());
                        return Model.Elements.Where(x => x.Class == elem).ToList<object>();
                    }
                    else if(context.pathName().GetText() == "any")
                    {
                        Dictionary<string, object> st = new Dictionary<string, object>();
                        vars.Add(st);
                        object ret = null;
                        foreach (object val in (ICollection<object>)res)
                        {
                            st["self"] = val;
                            if(hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                ret = val;
                                break;
                            }
                        }
                        vars.RemoveAt(vars.Count - 1);
                        return ret;
                    }
                    else if(context.pathName().GetText() == "forAll")
                    {
                        Dictionary<string, object> st = new Dictionary<string, object>();
                        vars.Add(st);
                        foreach (object val in (ICollection<object>)res)
                        {
                            st["self"] = val;
                            if (!hp.VisitExpression(context.propertyCallParameters().actualParameterList().expression()[0]))
                            {
                                return false;
                            }
                        }
                        vars.RemoveAt(vars.Count - 1);
                        return true;
                    }
                    Dictionary<string, object> stack = new Dictionary<string, object>();
                    List<string> names = funcs[context.pathName().GetText()].param;
                    HelloParser.ExpressionContext contextFunc = funcs[context.pathName().GetText()].context;
                    for (int i = 0; i < names.Count; i++)
                    {
                        stack[names[i]] = VisitExpression(context.propertyCallParameters().actualParameterList().expression()[i]);
                    }
                    vars.Add(stack);
                    object ress = VisitExpression(contextFunc);
                    vars.RemoveAt(vars.Count - 1);
                    return ress;
                }
                else if (context.Parent is HelloParser.PostfixExpressionContext)
                {
                    string elem = ((HelloParser.PostfixExpressionContext)context.Parent).primaryExpression().GetText();
                    if (elem != "self")
                    {
                        Element = Model.FindElement(elem);
                    }
                    if (context.NUMBER() != null)
                    {
                        IElement par = Element;
                        for (int i = 0; i < Depth - Int32.Parse(context.NUMBER().GetText()); i++)
                        {
                            par = par.Class;
                        }
                        Element = par;
                    }
                    return Element.Attributes.First(x => x.Name == context.pathName().GetText()).StringValue;
                }
                return VisitPathName(context.pathName());
            }
            public override object VisitPathName([NotNull] HelloParser.PathNameContext context)
            {
                for (int i = vars.Count - 1; i >= 0; i--)
                {
                    if (vars[i].ContainsKey(context.NAME()[0].GetText()))
                    {
                        return vars[i][context.NAME()[0].GetText()];
                    }
                }
                return context.NAME()[0].GetText();
            }
            public override object VisitIfExpression([NotNull] HelloParser.IfExpressionContext context)
            {
                if (hp.VisitExpression(context.expression()[0]))
                {
                    return VisitExpression(context.expression()[1]);
                }
                else
                {
                    return VisitExpression(context.expression()[2]);
                }
            }
        }
    }
}
