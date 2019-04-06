/* Copyright 2019 REAL.NET group
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

namespace Repo.CSharp.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class PluginLaunchTest
    {
        [Test]
        public void RepoShallAllowToCreateAMetamodelProgrammatically()
        {
            var repo = RepoFactory.Create();
            var model = repo.CreateModel("TestModel", "InfrastructureMetamodel");

            var node1 = model.CreateElement("Node");
            node1.Name = "TestNode1";
            node1.AddAttribute("testAttribute", AttributeKind.Int, "10");

            var node2 = model.CreateElement("Node");
            node2.Name = "TestNode2";
            node2.AddAttribute("otherAttribute", AttributeKind.String, "Ololo");

            var generalization = model.CreateElement("Generalization") as IEdge;
            generalization.From = node2;
            generalization.To = node1;

            Assert.AreEqual("testAttribute", node1.Attributes.First().Name);
            Assert.AreEqual("10", node1.Attributes.First().StringValue);

            Assert.AreEqual("otherAttribute", node2.Attributes.First().Name);
            Assert.AreEqual("Ololo", node2.Attributes.First().StringValue);
        }
    }
}
