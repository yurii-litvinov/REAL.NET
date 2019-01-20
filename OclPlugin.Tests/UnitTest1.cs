using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EditorPluginInterfaces;
using NUnit.Framework;
using WpfControlsLib.Controls.Console;

namespace Tests
{
    public class Tests
    {
        private OclPlugin.OclPlugin plugin;
        [SetUp]
        public void Setup()
        {
            plugin = new OclPlugin.OclPlugin();
        }

        [Test]
        public void Test1()
        {
            Dictionary<string, string> prop = new Dictionary<string, string>();
            prop.Add("ocl", @"package RobotsTestModel
            context aMotorsForward
            inv@0:
            Bag{ ""a"", ""bb"", ""ccc""}->select(self->size() = 2)->size() = 0
            endpackage");
            var console = new AppConsoleViewModel();
            var config = new PluginConfig(new WpfControlsLib.Model.Model(), null, null, console, null, prop);
            plugin.SetConfig(config);
            Assert.IsTrue(console.Messages.Contains("error"));
        }
    }
}