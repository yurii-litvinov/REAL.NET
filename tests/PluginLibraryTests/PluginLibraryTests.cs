using System;
using NUnit.Framework;
using PluginLibrary;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

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
            foreach (var dir in dirs)
            {
                libs.LaunchPlugins(dir);
            }
            Assert.IsTrue(libs.Plugins.Count >= 1);
        }
    }
}
