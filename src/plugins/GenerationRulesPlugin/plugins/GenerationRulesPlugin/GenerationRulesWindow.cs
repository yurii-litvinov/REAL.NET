using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using GenerationRulesPlugin.plugins.GenerationRulesPlugin;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;

namespace GenerationRulesPlugin
{
    public partial class GenerationRulesWindow
    {
        public GenerationRulesPlugin Plugin { get; set; }

        private CompletionWindow completionWindow;

        public GenerationRulesWindow()
        {
            this.InitializeComponent();
            using (var stream = new FileStream(@"..\..\..\..\Highlighter.xshd", FileMode.Open))
            {
                try
                {
                    this.CodeEditor.SyntaxHighlighting =
                        HighlightingLoader.Load(HighlightingLoader.LoadXshd(new XmlTextReader(stream)),
                            HighlightingManager.Instance);
                }
                catch (HighlightingDefinitionInvalidException e)
                {
                    this.CodeEditor.Text = $"[Invalid Syntax Highlighting Definition]\n{e.Message}";
                }
            }

            this.CodeEditor.TextArea.TextEntering += TextEntering;
            this.CodeEditor.TextArea.TextEntered += TextEntered;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            string filename = saveFileDialog.FileName;
            File.WriteAllText(filename, this.CodeEditor.Text);
            MessageBox.Show("File Saved");
        }

        private void OnSaveGeneratedCodeClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            string filename = saveFileDialog.FileName;
            File.WriteAllText(filename, this.ResultBox.Text);
            MessageBox.Show("File Saved");
        }

        private void OnGenerateCodeClick(object sender, RoutedEventArgs e)
        {
            string result = this.Plugin.CompileTemplate(this.CodeEditor.Text);
            this.ResultBox.Text = result;
        }

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "@")
            {
                completionWindow = new CompletionWindow(this.CodeEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new GenerationRulesCompletionData("Model"));
                data.Add(new GenerationRulesCompletionData("for"));
                data.Add(new GenerationRulesCompletionData("if"));
                data.Add(new GenerationRulesCompletionData("while"));
                data.Add(new GenerationRulesCompletionData("Include"));
                completionWindow.Show();
                completionWindow.Closed += (o, args) => this.completionWindow = null;
            }

            if (e.Text == ".")
            {
                completionWindow = new CompletionWindow(this.CodeEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                DocumentLine line =
                    this.CodeEditor.TextArea.Document.GetLineByNumber(this.CodeEditor.TextArea.Caret.Line);
                int lineCaretPosition = this.CodeEditor.TextArea.Caret.Offset - line.Offset;
                MatchCollection matches = new Regex(@"@Model\.").Matches(this.CodeEditor.TextArea.Document.GetText(line));
                foreach (Match match in matches)
                {
                    if (match.Index + match.Length == lineCaretPosition)
                    {
                        Plugin.GenerateCompletionList(data, true, null);
                    }
                }

                if (data.Count != 0)
                {
                    completionWindow.Show();
                    completionWindow.Closed += (o, args) => this.completionWindow = null;
                    return;
                }

                var regex = new Regex(@"@Model\.(?<element>\w+)\.");
                matches = regex.Matches(this.CodeEditor.TextArea.Document.GetText(line));
                foreach (Match match in matches)
                {
                    if (match.Index + match.Length == lineCaretPosition)
                    {
                        Plugin.GenerateCompletionList(data, false, match.Groups["element"].Captures[0].Value);
                    }
                }

                if (data.Count != 0)
                {
                    completionWindow.Show();
                    completionWindow.Closed += (o, args) => this.completionWindow = null;
                }
            }
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.Text[0]))
            {
                completionWindow?.CompletionList.RequestInsertion(e);
            }
        }
    }
}