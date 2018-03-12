using System.Windows.Forms;

namespace MsAglWinFormsEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.toolsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.paletteGrid = new System.Windows.Forms.TableLayoutPanel();
            this.nodeLayout = new System.Windows.Forms.TableLayoutPanel();
            this.attributeTable = new System.Windows.Forms.DataGridView();
            this.AttributeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loadImageButton = new System.Windows.Forms.Button();
            this.imageLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.widthEditor = new System.Windows.Forms.NumericUpDown();
            this.heightEditor = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.widthLabel = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.insertingEdge = new System.Windows.Forms.CheckBox();
            this.mainLayout.SuspendLayout();
            this.toolsLayout.SuspendLayout();
            this.nodeLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).BeginInit();
            this.imageLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.widthEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightEditor)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 2;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.79958F));
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.20042F));
            this.mainLayout.Controls.Add(this.toolsLayout, 1, 0);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 1;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(958, 533);
            this.mainLayout.TabIndex = 2;
            // 
            // toolsLayout
            // 
            this.toolsLayout.ColumnCount = 1;
            this.toolsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.toolsLayout.Controls.Add(this.paletteGrid, 0, 0);
            this.toolsLayout.Controls.Add(this.nodeLayout, 0, 1);
            this.toolsLayout.Controls.Add(this.refreshButton, 0, 3);
            this.toolsLayout.Controls.Add(this.insertingEdge, 0, 2);
            this.toolsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolsLayout.Location = new System.Drawing.Point(710, 3);
            this.toolsLayout.Name = "toolsLayout";
            this.toolsLayout.RowCount = 4;
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.62574F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 41.62574F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.49165F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.256881F));
            this.toolsLayout.Size = new System.Drawing.Size(245, 527);
            this.toolsLayout.TabIndex = 2;
            // 
            // paletteGrid
            // 
            this.paletteGrid.AutoSize = true;
            this.paletteGrid.ColumnCount = 1;
            this.paletteGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.paletteGrid.Location = new System.Drawing.Point(2, 2);
            this.paletteGrid.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.paletteGrid.Name = "paletteGrid";
            this.paletteGrid.RowCount = 1;
            this.paletteGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Size = new System.Drawing.Size(241, 0);
            this.paletteGrid.TabIndex = 2;
            // 
            // nodeLayout
            // 
            this.nodeLayout.ColumnCount = 1;
            this.nodeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.nodeLayout.Controls.Add(this.attributeTable, 0, 0);
            this.nodeLayout.Controls.Add(this.loadImageButton, 0, 1);
            this.nodeLayout.Controls.Add(this.imageLayoutPanel, 0, 2);
            this.nodeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeLayout.Location = new System.Drawing.Point(2, 221);
            this.nodeLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nodeLayout.Name = "nodeLayout";
            this.nodeLayout.RowCount = 4;
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.98453F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.65728F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.00469F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.32864F));
            this.nodeLayout.Size = new System.Drawing.Size(241, 215);
            this.nodeLayout.TabIndex = 0;
            // 
            // attributeTable
            // 
            this.attributeTable.AllowUserToAddRows = false;
            this.attributeTable.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.attributeTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attributeTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AttributeName,
            this.Type,
            this.Value});
            this.attributeTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributeTable.Location = new System.Drawing.Point(3, 3);
            this.attributeTable.Name = "attributeTable";
            this.attributeTable.RowHeadersVisible = false;
            this.attributeTable.Size = new System.Drawing.Size(235, 92);
            this.attributeTable.TabIndex = 2;
            this.attributeTable.Visible = false;
            // 
            // AttributeName
            // 
            this.AttributeName.HeaderText = "Name";
            this.AttributeName.Name = "AttributeName";
            this.AttributeName.ReadOnly = true;
            this.AttributeName.Width = 70;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 70;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.ReadOnly = true;
            this.Value.Width = 70;
            // 
            // loadImageButton
            // 
            this.loadImageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadImageButton.Location = new System.Drawing.Point(2, 100);
            this.loadImageButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(237, 40);
            this.loadImageButton.TabIndex = 3;
            this.loadImageButton.Text = "Load Image";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Visible = false;
            this.loadImageButton.Click += new System.EventHandler(this.LoadImageButtonClick);
            // 
            // imageLayoutPanel
            // 
            this.imageLayoutPanel.ColumnCount = 2;
            this.imageLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.66667F));
            this.imageLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.66667F));
            this.imageLayoutPanel.Controls.Add(this.widthEditor, 0, 1);
            this.imageLayoutPanel.Controls.Add(this.heightEditor, 1, 1);
            this.imageLayoutPanel.Controls.Add(this.label1, 1, 0);
            this.imageLayoutPanel.Controls.Add(this.widthLabel, 0, 0);
            this.imageLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLayoutPanel.Location = new System.Drawing.Point(2, 144);
            this.imageLayoutPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.imageLayoutPanel.Name = "imageLayoutPanel";
            this.imageLayoutPanel.RowCount = 2;
            this.imageLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.imageLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.imageLayoutPanel.Size = new System.Drawing.Size(237, 45);
            this.imageLayoutPanel.TabIndex = 5;
            this.imageLayoutPanel.Visible = false;
            // 
            // widthEditor
            // 
            this.widthEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.widthEditor.Location = new System.Drawing.Point(2, 34);
            this.widthEditor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.widthEditor.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.widthEditor.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.widthEditor.Name = "widthEditor";
            this.widthEditor.Size = new System.Drawing.Size(114, 20);
            this.widthEditor.TabIndex = 1;
            this.widthEditor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.widthEditor.ValueChanged += new System.EventHandler(this.ImageSizeChanged);
            // 
            // heightEditor
            // 
            this.heightEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heightEditor.Location = new System.Drawing.Point(120, 34);
            this.heightEditor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.heightEditor.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.heightEditor.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.heightEditor.Name = "heightEditor";
            this.heightEditor.Size = new System.Drawing.Size(115, 20);
            this.heightEditor.TabIndex = 2;
            this.heightEditor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.heightEditor.ValueChanged += new System.EventHandler(this.ImageSizeChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(120, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(115, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "height";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.widthLabel.Location = new System.Drawing.Point(2, 0);
            this.widthLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(114, 32);
            this.widthLabel.TabIndex = 4;
            this.widthLabel.Text = "width";
            this.widthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // refreshButton
            // 
            this.refreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshButton.Location = new System.Drawing.Point(2, 484);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(241, 41);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Update Graph";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // insertingEdge
            // 
            this.insertingEdge.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.insertingEdge.AutoSize = true;
            this.insertingEdge.Location = new System.Drawing.Point(82, 451);
            this.insertingEdge.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.insertingEdge.Name = "insertingEdge";
            this.insertingEdge.Size = new System.Drawing.Size(80, 17);
            this.insertingEdge.TabIndex = 6;
            this.insertingEdge.Text = "Insert Edge";
            this.insertingEdge.UseVisualStyleBackColor = true;
            this.insertingEdge.CheckedChanged += new System.EventHandler(this.InsertingEdgeCheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 533);
            this.Controls.Add(this.mainLayout);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(605, 370);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.mainLayout.ResumeLayout(false);
            this.toolsLayout.ResumeLayout(false);
            this.toolsLayout.PerformLayout();
            this.nodeLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).EndInit();
            this.imageLayoutPanel.ResumeLayout(false);
            this.imageLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.widthEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel toolsLayout;
        private TableLayoutPanel paletteGrid;
        private Button refreshButton;
        private TableLayoutPanel nodeLayout;
        private DataGridView attributeTable;
        private DataGridViewTextBoxColumn AttributeName;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Value;
        private Button loadImageButton;
        private TableLayoutPanel imageLayoutPanel;
        private NumericUpDown widthEditor;
        private NumericUpDown heightEditor;
        private CheckBox insertingEdge;
        private Label label1;
        private Label widthLabel;
    }
}

