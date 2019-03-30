using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EditorPluginInterfaces.Toolbar;
using GenerationRulesPlugin.plugins.GenerationRulesPlugin;
using ICSharpCode.AvalonEdit.CodeCompletion;
using RazorEngine;
using RazorEngine.Templating;
using Repo;

namespace GenerationRulesPlugin
{
    using System;
    using EditorPluginInterfaces;
    using WpfControlsLib.Controls.Toolbar;

    public class GenerationRulesPlugin : IPlugin<PluginConfig>
    {
        public string Name => "Generation Rules Plugin";

        private GenerationRulesWindow window;

        private IConsole console;
        private IModel model;

        public void SetConfig(PluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }

            this.console = config.Console;
            this.console.SendMessage($"{this.Name} successfully launched");

            this.model = config.Model;
            IToolbar toolbar = config.Toolbar;

            var command = new Command(() =>
            {
                this.window = new GenerationRulesWindow { Plugin = this };
                this.window.Show();
            });
            var button = new Button(command, "Opens code generation rules editor", "redo.png");
            toolbar.AddButton(button);
        }

        public string CompileTemplate(string input)
        {
            input = "@using GenerationRulesPlugin\n" + input;
            IRepo repo = this.model.Repo;
            Repo.IModel currentModel = repo.Model("RobotsTestModel");
            var regex = new Regex(@"@Model\.(?<element>\w+)\.(?<attribute>\w+)(?<lastCharacter>\W?)");
            MatchCollection matches = regex.Matches(input);
            string template = input;
            foreach (Match match in matches)
            {
                string attribute = match.Groups["attribute"].Captures[0].Value;
                string element = match.Groups["element"].Captures[0].Value;
                Group lastCharacterMatchGroup = match.Groups["lastCharacter"];
                string lastCharacter = lastCharacterMatchGroup.Success
                    ? lastCharacterMatchGroup.Captures[0].Value
                    : string.Empty;
                template = Regex.Replace(template, match.Value, $"@Model.FindElement(\"{element}\").FindAttribute(\"{attribute}\").StringValue{lastCharacter}");
            }

            try
            {

                return Engine.Razor.RunCompile(template, $"key{new Random().Next()}", typeof(Repo.IModel),
                    currentModel).Trim();
            }
            catch (Exception e)
            {
                return $"[Compiling error]\n{e.Message}";
            }
        }

        public void GenerateCompletionList(IList<ICompletionData> data, bool isElement, string element)
        {
            if (isElement)
            {
                foreach (IElement el in this.model.Repo.Model("RobotsTestModel").Elements)
                {
                    data.Add(new GenerationRulesCompletionData(el.Name));
                }
            }
            else
            {
                try
                {
                    foreach (IAttribute attribute in this.model.Repo.Model("RobotsTestModel").FindElement(element).Attributes)
                    {
                        data.Add(new GenerationRulesCompletionData(attribute.Name));
                    }
                }
                catch (MultipleElementsException)
                {
                    data.Clear();
                }
            }

            return;
        }
    }

    public static class ElementExtension
    {
        public static IAttribute FindAttribute(this IElement element, string name)
        {
            foreach (IAttribute attribute in element.Attributes.Where(a => a.Name == name))
            {
                return attribute;
            }

            throw new AttributeNotFoundException(name);
        }
    }

    
}
