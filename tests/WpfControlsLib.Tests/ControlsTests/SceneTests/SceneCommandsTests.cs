/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace WpfControlsLib.Tests.ControlsTests.SceneTests
{
    using NUnit.Framework;
    using WpfControlsLib.Model;
    using WpfControlsLib.Controls.Scene.Commands;
    using EditorPluginInterfaces;
    using System.Linq;

    [TestFixture]
    public class SceneCommandsTests
    {
        private Model model;
        private Repo.IModel testModel;
        private Repo.INode testNodeType;
        private Repo.IEdge testEdgeType;

        [SetUp]
        public void Init()
        {
            model = new Model();
            testModel = model.Repo.CreateModel("testModel", "RobotsMetamodel");
            model.ModelName = testModel.Name;
            testNodeType = testModel.Metamodel.FindElement("FinalNode") as Repo.INode;
            testEdgeType = testModel.Metamodel.FindElement("Link") as Repo.IEdge;
        }

        [Test]
        public void AddingNodeOnSceneShouldResultInAddingNodeInRepo()
        {
            ICommand command = new CreateNodeCommand(model, testNodeType);

            command.Execute();

            Assert.AreEqual(1, testModel.Nodes.Count());
            Assert.AreEqual("aFinalNode", testModel.Nodes.First().Name);
        }

        [Test]
        public void AddingEdgeOnSceneShouldResultInAddingEdgeInRepo()
        {
            var node1 = testModel.CreateElement(testNodeType);
            var node2 = testModel.CreateElement(testNodeType);
            ICommand command = new CreateEdgeCommand(model, testEdgeType, node1, node2);

            command.Execute();

            Assert.AreEqual(2, testModel.Nodes.Count());
            Assert.AreEqual(1, testModel.Edges.Count());
            Assert.AreEqual("aLink", testModel.Edges.First().Name);
        }

        [Test]
        public void RemovingNodeOnSceneShouldResultInRemovingFromInRepo()
        {
            var node = testModel.CreateElement(testNodeType);
            ICommand command = new RemoveNodeCommand(model, node);

            command.Execute();

            Assert.Zero(testModel.Nodes.Count());
        }

        [Test]
        public void RemovingEdgeOnSceneShouldResultInRemovingFromRepo()
        {
            var node1 = testModel.CreateElement(testNodeType);
            var node2 = testModel.CreateElement(testNodeType);
            var edge = testModel.CreateElement(testEdgeType) as Repo.IEdge;
            edge.From = node1;
            edge.To = node2;
            ICommand command = new RemoveEdgeCommand(model, edge);

            command.Execute();

            Assert.Zero(testModel.Edges.Count());
            Assert.AreEqual(2, testModel.Nodes.Count());
        }
    }
}
