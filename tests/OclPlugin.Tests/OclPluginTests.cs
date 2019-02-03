using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using OclPlugin;
using Repo;

namespace OclPlugin.Tests
{
    public class OclPluginTests
    {
        private OclInterpreter interpreter;

        [SetUp]
        public void Setup()
        {
            var repo = RepoFactory.Create();
            interpreter = new OclInterpreter(repo);
        }

        [Test]
        public void CollectionTest1()
        {
            var stream = CharStreams.fromstring(@"package RobotsTestModel
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
            Assert.IsTrue(tree.Accept(interpreter));

        }

        [Test]
        public void StringTest1()
        {
            var stream = CharStreams.fromstring(@"package RobotsTestModel
            context aMotorsForward
            inv@0:
            ""ABC""->toLower() = ""abc""
            inv@0:
            ""123""->toInteger() = 123
            endpackage");

            ITokenSource lexer = new OclLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new OclParser(tokens)
            {
                BuildParseTree = true
            };
            IParseTree tree = parser.oclFile();
            Assert.IsTrue(tree.Accept(interpreter));

        }

        [Test]
        public void NumberTest1()
        {
            var stream = CharStreams.fromstring(@"package RobotsTestModel
            context aMotorsForward
            inv@0:
            1->max(2) = 2
            inv@0:
            5->div(2) = 2
            endpackage");

            ITokenSource lexer = new OclLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new OclParser(tokens)
            {
                BuildParseTree = true
            };
            IParseTree tree = parser.oclFile();
            Assert.IsTrue(tree.Accept(interpreter));

        }
    }
}