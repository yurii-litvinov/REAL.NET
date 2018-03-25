namespace WpfEditor.Controls.Scene
{
    using System.Windows;
    using System.Windows.Controls;
    using GraphX.Controls;

    [TemplatePart(Name = "PART_source_vcproot", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_target_vcproot", Type = typeof(Panel))]
    public class VertexControlForGH : VertexControl
    {
        public VertexControlForGH(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true)
            : base(vertexData, tracePositionChange, bindToDataObject)
        {
        }

        public Panel VCPSourceRoot { get; protected set; }

        public Panel VCPTargetRoot { get; protected set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.VCPSourceRoot = this.VCPSourceRoot ?? this.FindDescendant<Panel>("PART_source_vcproot");
            this.VCPTargetRoot = this.VCPTargetRoot ?? this.FindDescendant<Panel>("PART_target_vcproot");
        }
    }
}
