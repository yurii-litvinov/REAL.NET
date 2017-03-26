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
            this.paintButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.mainLayout.SuspendLayout();
            this.toolsLayout.SuspendLayout();
            this.nodeLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).BeginInit();
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
            this.toolsLayout.Controls.Add(this.refreshButton, 0, 2);
            this.toolsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolsLayout.Location = new System.Drawing.Point(710, 3);
            this.toolsLayout.Name = "toolsLayout";
            this.toolsLayout.RowCount = 3;
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.37206F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.37205F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.255898F));
            this.toolsLayout.Size = new System.Drawing.Size(245, 527);
            this.toolsLayout.TabIndex = 2;
            // 
            // paletteGrid
            // 
            this.paletteGrid.AutoSize = true;
            this.paletteGrid.ColumnCount = 1;
            this.paletteGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.paletteGrid.Location = new System.Drawing.Point(3, 236);
            this.paletteGrid.Name = "paletteGrid";
            this.paletteGrid.RowCount = 1;
            this.paletteGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Size = new System.Drawing.Size(239, 0);
            this.paletteGrid.TabIndex = 2;
            // 
            // nodeLayout
            // 
            this.nodeLayout.ColumnCount = 1;
            this.nodeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.nodeLayout.Controls.Add(this.attributeTable, 0, 0);
            this.nodeLayout.Controls.Add(this.loadImageButton, 0, 2);
            this.nodeLayout.Controls.Add(this.paintButton, 0, 1);
            this.nodeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeLayout.Location = new System.Drawing.Point(3, 242);
            this.nodeLayout.Name = "nodeLayout";
            this.nodeLayout.RowCount = 3;
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.34539F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.3273F));
            this.nodeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.3273F));
            this.nodeLayout.Size = new System.Drawing.Size(239, 233);
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
            this.attributeTable.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.attributeTable.Location = new System.Drawing.Point(3, 3);
            this.attributeTable.Name = "attributeTable";
            this.attributeTable.RowHeadersVisible = false;
            this.attributeTable.Size = new System.Drawing.Size(233, 118);
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
            this.loadImageButton.Location = new System.Drawing.Point(3, 181);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(233, 49);
            this.loadImageButton.TabIndex = 3;
            this.loadImageButton.Text = "Load Image";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Visible = false;
            this.loadImageButton.Click += new System.EventHandler(this.LoadImageButtonClick);
            // 
            // paintButton
            // 
            this.paintButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paintButton.Location = new System.Drawing.Point(3, 127);
            this.paintButton.Name = "paintButton";
            this.paintButton.Size = new System.Drawing.Size(233, 48);
            this.paintButton.TabIndex = 4;
            this.paintButton.Text = "Open Node Paint";
            this.paintButton.UseVisualStyleBackColor = true;
            this.paintButton.Visible = false;
            this.paintButton.Click += new System.EventHandler(this.PaintButtonClick);
            // 
            // refreshButton
            // 
            this.refreshButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshButton.Location = new System.Drawing.Point(3, 481);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(239, 43);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Update Graph";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 533);
            this.Controls.Add(this.mainLayout);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.mainLayout.ResumeLayout(false);
            this.toolsLayout.ResumeLayout(false);
            this.toolsLayout.PerformLayout();
            this.nodeLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel toolsLayout;
        private TableLayoutPanel paletteGrid;
        private TableLayoutPanel nodeLayout;
        private DataGridView attributeTable;
        private DataGridViewTextBoxColumn AttributeName;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Value;
        private Button loadImageButton;
        private Button paintButton;
        private Button refreshButton;
    }
}

