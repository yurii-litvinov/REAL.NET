using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EditorPluginInterfaces;
using NUnit.Framework;
using OclPlugin;
using Repo;

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
            var repo = RepoFactory.Create();
            ICharStream stream = CharStreams.fromstring(@"package RobotsTestModel
            context aMotorsForward
            inv@0:
            Bag{ ""a"", ""bb"", ""ccc""}->select(self->size() = 2)->size() = 0
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