namespace OclPlugin
{
    using System;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using EditorPluginInterfaces;
    using WpfControlsLib.Controls.Scene;

    public class OclPlugin : IPlugin<PluginConfig>
    {
        public string Name => "OCL";

        private IConsole console;

        public void SetConfig(PluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }

            this.console = config.Console;
            this.console.SendMessage("OCL add-on successfully launched");

            this.Execute(config);
        }

        public void Execute(PluginConfig config)
        {
            var model = config.Model;
            var repo = model.Repo;
            var textExpr = config.Properties?["ocl"];
            Scene scene = (Scene)config.Properties?["scene"];
            ICharStream stream;
            if (textExpr == null)
            {
                stream = CharStreams.fromPath("D:\\Projects\\REAL.NET\\src\\plugins\\OclPlugin\\test-ocl");
            }
            else
            {
                stream = CharStreams.fromstring(textExpr.ToString());
            }

            ITokenSource lexer = new OclLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new OclParser(tokens)
            {
                BuildParseTree = true
            };
            IParseTree tree = parser.oclFile();
            var interpreter = new OclInterpreter(repo, scene);
            //Console.WriteLine(tree.ToStringTree(parser));
            if (tree.Accept(interpreter))
            {
                this.console.SendMessage("ok");
            }
            else
            {
                this.console.SendMessage("error");
            }
        }
    }
}
