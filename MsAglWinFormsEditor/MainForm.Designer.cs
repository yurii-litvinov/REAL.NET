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
            this.attributeTable = new System.Windows.Forms.DataGridView();
            this.AttributeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.toolsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.paletteGrid = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).BeginInit();
            this.mainLayout.SuspendLayout();
            this.toolsLayout.SuspendLayout();
            this.SuspendLayout();
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
            this.attributeTable.Location = new System.Drawing.Point(3, 370);
            this.attributeTable.Name = "attributeTable";
            this.attributeTable.RowHeadersVisible = false;
            this.attributeTable.Size = new System.Drawing.Size(239, 154);
            this.attributeTable.TabIndex = 1;
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
            this.toolsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolsLayout.Controls.Add(this.attributeTable, 0, 1);
            this.toolsLayout.Controls.Add(this.paletteGrid, 0, 0);
            this.toolsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolsLayout.Location = new System.Drawing.Point(710, 3);
            this.toolsLayout.Name = "toolsLayout";
            this.toolsLayout.RowCount = 2;
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolsLayout.Size = new System.Drawing.Size(245, 527);
            this.toolsLayout.TabIndex = 2;
            // 
            // paletteGrid
            // 
            this.paletteGrid.AutoSize = true;
            this.paletteGrid.ColumnCount = 1;
            this.paletteGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.paletteGrid.Location = new System.Drawing.Point(3, 260);
            this.paletteGrid.Name = "paletteGrid";
            this.paletteGrid.RowCount = 1;
            this.paletteGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.paletteGrid.Size = new System.Drawing.Size(239, 0);
            this.paletteGrid.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 533);
            this.Controls.Add(this.mainLayout);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.attributeTable)).EndInit();
            this.mainLayout.ResumeLayout(false);
            this.toolsLayout.ResumeLayout(false);
            this.toolsLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private TableLayoutPanel mainLayout;
        private DataGridViewTextBoxColumn AttributeName;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Value;
        private TableLayoutPanel toolsLayout;
        private DataGridView attributeTable;
        private TableLayoutPanel paletteGrid;
    }
}

