using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using OclPlugin;
using Repo;
using Repo.InfrastructureSemanticLayer;
using Repo.Metametamodels;

namespace OclPlugin.Tests
{
    public class OclPluginTests
    {
        private OclInterpreter interpreter;

        [SetUp]
        public void Setup()
        {
            //var repo = RepoFactory.Create();
            //new InfrastructureSemantic(repo.);
            var data = new Repo.DataLayer.DataRepo();

            CoreMetametamodelBuilder builder5 = new CoreMetametamodelBuilder();
            ((IModelBuilder)builder5).Build(data);
            LanguageMetamodelBuilder builder6 = new LanguageMetamodelBuilder();
            ((IModelBuilder)builder6).Build(data);
            InfrastructureMetamodelBuilder builder7 = new InfrastructureMetamodelBuilder();
            ((IModelBuilder)builder7).Build(data);
            NodeMetamodelBuilder builder = new NodeMetamodelBuilder();
            ((IModelBuilder)builder).Build(data);
            ObjectMetamodelBuilder builder2 = new ObjectMetamodelBuilder();
            ((IModelBuilder)builder2).Build(data);
            ImplObjectMetamodelBuilder builder3 = new ImplObjectMetamodelBuilder();
            ((IModelBuilder)builder3).Build(data);
            DiagramObjectMetamodelBuilder builder4 = new DiagramObjectMetamodelBuilder();
            ((IModelBuilder)builder4).Build(data);
            RobotsMetamodelBuilder builder8 = new RobotsMetamodelBuilder();
            ((IModelBuilder)builder8).Build(data);
            RobotsTestModelBuilder builder9 = new RobotsTestModelBuilder();
            ((IModelBuilder)builder9).Build(data);

            var repo = new Repo.FacadeLayer.Repo(data);

            interpreter = new OclInterpreter(repo, null);
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
            inv@0:
            Set{""1"",""2"",""3""}->collect(self = ""0"")->size() = 3
            inv@0:
            Bag{""1"",""2"",""3""}->any(self.toInteger() > 2).toInteger() = 3
            inv@0:
            aMotorsForward->allInstances()->size() > -1
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
            inv@0:
            ""123""->toReal() = 123
            inv@0:
            ""abc""->toUpper() = ""ABC""
            inv@0:
            ""abc""->concat(""cde"")->toUpper() = ""ABCCDE""
            inv@0:
            ""abc""->substring(1, 1) = ""a""
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
            inv@0:
            5->min(2) = 2
            inv@0:
            5->mod(2) = 1
            inv@0:
            5->abs() = 5
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
        public void FuncTest1()
        {

            var stream = CharStreams.fromstring(@"package ObjectMetamodel
            context FunctionNode
            inv@2:
            self.params@1 <> self.callParams@2
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