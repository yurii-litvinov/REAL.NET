namespace OclPlugin
{
    using System;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using EditorPluginInterfaces;

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

            var model = config.Model;
            var repo = model.Repo;

            var stream = CharStreams.fromPath("E:\\OUR DISK\\NIKITA\\REAL.NET\\src\\plugins\\OclPlugin\\test-ocl");
            ITokenSource lexer = new OclLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new OclParser(tokens)
            {
                BuildParseTree = true
            };
            IParseTree tree = parser.oclFile();
            var interpreter = new OclInterpreter(this.console, repo);
            Console.WriteLine(tree.ToStringTree(parser));
            tree.Accept(interpreter);
        }
    }
}
