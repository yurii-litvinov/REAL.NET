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

namespace PluginLibrary.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Controls;
    using EditorPluginInterfaces;
    using NSubstitute;
    using NUnit.Framework;
    using PluginManager;

    [TestFixture]
    public class PluginLaunchTest
    {
        [Test]
        public void TestLaunchingSamplePlugin()
        {
            var libs = new PluginLauncher<PluginConfig>();
            var folder = Path.Combine(
                TestContext.CurrentContext.TestDirectory, 
                "../../../../src/plugins/SamplePlugin/bin"
                );
            var dirs = new List<string>(Directory.GetDirectories(folder));
            var console = Substitute.For<IConsole>();
            var model = Substitute.For<IModel>();
            var config = new PluginConfig(model, null, null, console, null, null);
            foreach (var dir in dirs)
            {
                libs.LaunchPlugins(dir, config);
            }
            Assert.IsTrue(libs.Plugins.Count >= 1);
            console.Received().SendMessage(Arg.Any<string>());
        }
    }
}