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
        ArrayList<Dictionary<string, string>> vars;
        HelloCalc calc;
        Dictionary<string, FunctionDef> funcs;
        IConsole console;
        IRepo repository;
        Repo.IModel model;
        IElement curElement;

        public HelloPrinter(IConsole console, IRepo repo)
        {
            vars = new ArrayList<Dictionary<string, string>>();
            vars.Add(new Dictionary<string, string>());
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
                VisitOclExpression(context.oclExpression()[i]);
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
            if (calc.Depth > 0)
            {
                foreach (Repo.IElement elem in model.Elements)
                {
                    Repo.IElement el = elem;
                    for (int i = 0; i < calc.Depth; i++)
                    {
                        el = el.Class;
                    }
                    if (el == curElement)
                    {
                        curElement = elem;
                        if (!VisitExpression(context.expression()))
                        {
                            console.SendMessage("err");
                        }
                        else
                        {
                            console.SendMessage("ok");
                        }
                    }
                }
            }
            else
            {
                if (!VisitExpression(context.expression()))
                {
                    console.SendMessage("err");
                }
                else
                {
                    console.SendMessage("ok");
                }
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
                switch (context.relationalOperator().Start.Text)
                {
                    case "=":
                        return (int) calc.VisitAdditiveExpression(context.additiveExpression()[0]) == (double) calc.VisitAdditiveExpression(context.additiveExpression()[1]);
                    case "<":
                        return (double) calc.VisitAdditiveExpression(context.additiveExpression()[0]) < (double) calc.VisitAdditiveExpression(context.additiveExpression()[1]);
                    case ">":
                        return (double) calc.VisitAdditiveExpression(context.additiveExpression()[0]) > (double) calc.VisitAdditiveExpression(context.additiveExpression()[1]);

                }
            }
            return true;
        }

        class HelloCalc : HelloBaseVisitor<object>
        {
            ArrayList<Dictionary<string, string>> vars = null;
            Dictionary<string, FunctionDef> funcs = null;
            HelloPrinter hp = null;
            IRepo repository = null;
            IConsole console = null;
            public int Depth = 0;

            public Repo.IModel Model { get; set; } = null;
            public IElement Element { get; set; } = null;
            object res = null;

            public HelloCalc(ArrayList<Dictionary<string, string>> vars, Dictionary<string, FunctionDef> funcs, HelloPrinter hp, IRepo repo, IConsole cons)
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
                double num = 0;
                if (context.NUMBER() != null)
                {
                    num = Double.Parse(context.NUMBER().GetText());
                }

                return num;
            }

            public override object VisitLiteralCollection([NotNull] HelloParser.LiteralCollectionContext context)
            {
                switch (context.collectionKind().GetText())
                {
                    case "Set":
                        return new HashSet<object>(context.collectionItem());
                }
                return new HashSet<object>();
            }

            public override object VisitPropertyCall([NotNull] HelloParser.PropertyCallContext context)
            {
                if (context.propertyCallParameters() != null)
                {
                    if(context.pathName().GetText() == "size")
                    {
                        if(res != null)
                        {
                            return ((ICollection<object>)res).Count;
                        }
                    }
                    Dictionary<string, string> stack = new Dictionary<string, string>();
                    List<string> names = funcs[context.pathName().GetText()].param;
                    HelloParser.ExpressionContext contextFunc = funcs[context.pathName().GetText()].context;
                    for (int i = 0; i < names.Count; i++)
                    {
                        stack[names[i]] = VisitExpression(context.propertyCallParameters().actualParameterList().expression()[i]).ToString();
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
                    return Double.Parse(Element.Attributes.First(x => x.Name == context.pathName().GetText()).StringValue);
                }
                return VisitPathName(context.pathName());
            }
            public override object VisitPathName([NotNull] HelloParser.PathNameContext context)
            {
                for (int i = vars.Count - 1; i >= 0; i--)
                {
                    if (vars[i].ContainsKey(context.NAME()[0].GetText()))
                    {
                        return Double.Parse(vars[i][context.NAME()[0].GetText()]);
                    }
                }
                return 0;
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
