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
            this.insertingEdge = new System.Windows.Forms.CheckBox();
            this.refreshButton = new System.Windows.Forms.Button();
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
            this.mainLayout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 1;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(1437, 820);
            this.mainLayout.TabIndex = 2;
            // 
            // toolsLayout
            // 
            this.toolsLayout.ColumnCount = 1;
            this.toolsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.toolsLayout.Controls.Add(this.paletteGrid, 0, 0);
            this.toolsLayout.Controls.Add(this.nodeLayout, 0, 1);
            this.toolsLayout.Controls.Add(this.refreshButton, 0, 2);
            this.toolsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolsLayout.Location = new System.Drawing.Point(1064, 5);
            this.toolsLayout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolsLayout.Name = "toolsLayout";
            this.toolsLayout.RowCount = 4;
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.37205F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.37205F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.255898F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.toolsLayout.Size = new System.Drawing.Size(369, 810);
            this.toolsLayout.TabIndex = 2;
            // 
            // paletteGrid
            // 
            this.paletteGrid.AutoSize = true;
            this.paletteGrid.ColumnCount = 1;
            this.paletteGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.paletteGrid.Location = new System.Drawing.Point(4, 348);
            this.paletteGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.paletteGrid.Name = "paletteGrid";
            this.paletteGrid.RowCount = 1;
            this.paletteGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Size = new System.Drawing.Size(361, 0);
            this.paletteGrid.TabIndex = 2;
            // 
            // nodeLayout
            // 
            this.nodeLayout.ColumnCount = 1;
            this.nodeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.nodeLayout.Controls.Add(this.attributeTable, 0, 0);
            this.nodeLayout.Controls.Add(this.loadImageButton, 0, 1);
            this.nodeLayout.Controls.Add(this.imageLayoutPanel, 0, 2);
            this.nodeLayout.Controls.Add(this.insertingEdge, 0, 3);
            this.nodeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeLayout.Location = new System.Drawing.Point(4, 358);
            this.nodeLayout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nodeLayout.Name = "nodeLayout";
            this.nodeLayout.RowCount = 4;
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.18144F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.13019F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.68837F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.nodeLayout.Size = new System.Drawing.Size(361, 343);
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
            this.attributeTable.Location = new System.Drawing.Point(4, 5);
            this.attributeTable.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.attributeTable.Name = "attributeTable";
            this.attributeTable.RowHeadersVisible = false;
            this.attributeTable.Size = new System.Drawing.Size(353, 147);
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
            this.loadImageButton.Location = new System.Drawing.Point(4, 162);
            this.loadImageButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(353, 59);
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
            this.imageLayoutPanel.Controls.Add(this.widthEditor, 0, 0);
            this.imageLayoutPanel.Controls.Add(this.heightEditor, 1, 0);
            this.imageLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLayoutPanel.Location = new System.Drawing.Point(4, 231);
            this.imageLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.imageLayoutPanel.Name = "imageLayoutPanel";
            this.imageLayoutPanel.RowCount = 1;
            this.imageLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.imageLayoutPanel.Size = new System.Drawing.Size(353, 49);
            this.imageLayoutPanel.TabIndex = 5;
            this.imageLayoutPanel.Visible = false;
            // 
            // widthEditor
            // 
            this.widthEditor.Location = new System.Drawing.Point(4, 5);
            this.widthEditor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.widthEditor.Size = new System.Drawing.Size(136, 26);
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
            this.heightEditor.Location = new System.Drawing.Point(180, 5);
            this.heightEditor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.heightEditor.Size = new System.Drawing.Size(169, 26);
            this.heightEditor.TabIndex = 2;
            this.heightEditor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.heightEditor.ValueChanged += new System.EventHandler(this.ImageSizeChanged);
            // 
            // insertingEdge
            // 
            this.insertingEdge.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.insertingEdge.AutoSize = true;
            this.insertingEdge.Location = new System.Drawing.Point(121, 302);
            this.insertingEdge.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.insertingEdge.Name = "insertingEdge";
            this.insertingEdge.Size = new System.Drawing.Size(118, 24);
            this.insertingEdge.TabIndex = 6;
            this.insertingEdge.Text = "Insert Edge";
            this.insertingEdge.UseVisualStyleBackColor = true;
            this.insertingEdge.CheckedChanged += new System.EventHandler(this.InsertingEdgeCheckedChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshButton.Location = new System.Drawing.Point(4, 711);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(361, 62);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Update Graph";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1437, 820);
            this.Controls.Add(this.mainLayout);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.mainLayout.ResumeLayout(false);
            this.toolsLayout.ResumeLayout(false);
            this.toolsLayout.PerformLayout();
            this.nodeLayout.ResumeLayout(false);
            this.nodeLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).EndInit();
            this.imageLayoutPanel.ResumeLayout(false);
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
    }
}

