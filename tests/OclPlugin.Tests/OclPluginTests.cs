﻿using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using OclPlugin;
using Repo;
using Repo.InfrastructureSemanticLayer;

namespace OclPlugin.Tests
{
    public class OclPluginTests
    {
        private OclInterpreter interpreter;

        [SetUp]
        public void Setup()
        {
            var repo = RepoFactory.Create();
            //new InfrastructureSemantic(repo.);
            IModel meta = repo.Model("CoreMetametamodel");
            IModel func = repo.CreateModel("Function", meta);
            
            IModel call = repo.CreateModel("Call", func);
            IElement funcElem = func.CreateElement(meta.FindElement("Node"));
            
            IElement callElem = call.CreateElement(funcElem);

            interpreter = new OclInterpreter(repo);
        }

        [Test]
        public void CollectionTest1()
        {
            var stream = CharStreams.fromstring(@"package RobotsTestModel
            context aMotorsForward
            inv@0:
            Bag{ ""a"", ""bb"", ""ccc""}->select(self->size() = 2)->size() = 1
            inv@0:
            Bag{1,2,3}->forAll(x, y | x <> y implies x*y > 1)
            inv@0:
            Bag{1,2,3}->iterate(x; y : T = 0 | y + x) = 6
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