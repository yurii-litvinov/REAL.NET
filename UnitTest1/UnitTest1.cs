using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginLibrary;

namespace UnitTest1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var libs = new PluginLauncher();
            libs.LaunchPlugins("../../../PluginLibrary/bin/debug");
            libs.PrintAvailiblePlugins();
        }
    }
}
