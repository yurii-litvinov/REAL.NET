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
            libs.LaunchPlugins("plugins/SamplesPlugin/bin/debug");
            Assert.AreEqual(1, libs.Plugins.Count);
        }
    }
}
