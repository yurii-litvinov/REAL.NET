using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EditorPluginInterfaces;
using NUnit.Framework;
using OclPlugin;
using WpfControlsLib.Controls.Console;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var model = new WpfControlsLib.Model.Model();
            var repo = model.Repo;
            ICharStream stream = CharStreams.fromstring(@"package RobotsTestModel
            context aMotorsForward
            inv@0:
            Bag{ ""a"", ""bb"", ""ccc""}->select(self->size() = 2)->size() = 1
            endpackage");

            ITokenSource lexer = new OclLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new OclParser(tokens)
            {
                BuildParseTree = true
            };
            IParseTree tree = parser.oclFile();
            var interpreter = new OclInterpreter(repo);
            Assert.IsTrue(tree.Accept(interpreter));
            
        }
    }
}