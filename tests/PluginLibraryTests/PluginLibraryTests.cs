using System;
using NUnit.Framework;
using PluginLibrary;
using System.Reflection;
using System.IO;

namespace PluginLibraryTests
{
    [TestFixture]
    public class PluginLaunchTest
    {
        [Test]
        public void TestLaunchingSamplePlugin()
        {
            var libs = new PluginLauncher();
            libs.LaunchPlugins("src/plugins/SamplesPlugin/bin/Release");
            Assert.AreEqual(1, libs.Plugins.Count);
        }
    }
}
