using NUnit.Framework;
using PluginLibrary;
using System.IO;
using System.Collections.Generic;
using PluginLibrary.MainInterfaces;

namespace PluginLibraryTests
{
    [TestFixture]
    public class PluginLaunchTest
    {
        [Test]
        public void TestLaunchingSamplePlugin()
        {
            var libs = new PluginLauncher();
            var folder = "src/plugins/SamplesPlugin/bin";
            var dirs = new List<string>(Directory.GetDirectories(folder));
            var console = Substitute.For<IConsole>();
            var config = new PluginConfig(null, null, console, null);
            foreach (var dir in dirs)
            {
                libs.LaunchPlugins(dir, config);
            }
            Assert.IsTrue(libs.Plugins.Count >= 1);
            console.Received().SendMessage(Arg.Any<string>());
        }
    }
}
