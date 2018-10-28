using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EditorPluginInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OclPlugin
{
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

      ICharStream stream = CharStreams.fromPath("D:\\Projects\\REAL.NET\\src\\plugins\\OclPlugin\\test-ocl");
      ITokenSource lexer = new HelloLexer(stream);
      ITokenStream tokens = new CommonTokenStream(lexer);
      HelloParser parser = new HelloParser(tokens);
      parser.BuildParseTree = true;
      IParseTree tree = parser.oclFile();
      HelloPrinter printer = new HelloPrinter(this.console, repo);
      //ParseTreeWalker.Default.Walk(printer, tree);
      tree.Accept(printer);
    }
  }
}
