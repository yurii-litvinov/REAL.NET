using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace GenerationRulesPlugin.plugins.GenerationRulesPlugin
{
    /// <summary>
    /// Describes an entry in the <see cref="T:ICSharpCode.AvalonEdit.CodeCompletion.CompletionList" /> specified for Generation Rules Plugin.
    /// </summary>
    /// <inheritdoc cref="ICompletionData"/>
    internal class GenerationRulesCompletionData : ICompletionData
    {
        public GenerationRulesCompletionData(string text)
        {
            this.Text = text;
        }


        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (completionSegment.Offset == completionSegment.EndOffset)
            {
                textArea.Document.Insert(completionSegment.Offset, this.Text);
            }
            else
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
        }

        public ImageSource Image { get; }
        public string Text { get; }
        public object Content => this.Text;

        public object Description => "Model property" + this.Text;

        public double Priority { get; }
    }
}
