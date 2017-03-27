namespace MsAglWinFormsEditor
{
    partial class DrawingForm
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
            this.toolLayot = new System.Windows.Forms.TableLayoutPanel();
            this.itemsListBox = new System.Windows.Forms.ListBox();
            this.shapesComboBox = new System.Windows.Forms.ComboBox();
            this.Save = new System.Windows.Forms.Button();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.mainLayout.SuspendLayout();
            this.toolLayot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 2;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.mainLayout.Controls.Add(this.toolLayot, 1, 0);
            this.mainLayout.Controls.Add(this.canvas, 0, 0);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 1;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainLayout.Size = new System.Drawing.Size(455, 282);
            this.mainLayout.TabIndex = 0;
            // 
            // toolLayot
            // 
            this.toolLayot.ColumnCount = 1;
            this.toolLayot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolLayot.Controls.Add(this.itemsListBox, 0, 1);
            this.toolLayot.Controls.Add(this.shapesComboBox, 0, 0);
            this.toolLayot.Controls.Add(this.Save, 0, 2);
            this.toolLayot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolLayot.Location = new System.Drawing.Point(325, 3);
            this.toolLayot.Name = "toolLayot";
            this.toolLayot.RowCount = 3;
            this.toolLayot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolLayot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.toolLayot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.toolLayot.Size = new System.Drawing.Size(127, 276);
            this.toolLayot.TabIndex = 0;
            // 
            // itemsListBox
            // 
            this.itemsListBox.FormattingEnabled = true;
            this.itemsListBox.Location = new System.Drawing.Point(3, 118);
            this.itemsListBox.Name = "itemsListBox";
            this.itemsListBox.Size = new System.Drawing.Size(120, 95);
            this.itemsListBox.TabIndex = 1;
            // 
            // shapesComboBox
            // 
            this.shapesComboBox.FormattingEnabled = true;
            this.shapesComboBox.Items.AddRange(new object[] {
            "Rectangle",
            "Ellipse",
            "Image"});
            this.shapesComboBox.Location = new System.Drawing.Point(3, 3);
            this.shapesComboBox.Name = "shapesComboBox";
            this.shapesComboBox.Size = new System.Drawing.Size(121, 21);
            this.shapesComboBox.TabIndex = 2;
            this.shapesComboBox.SelectedIndexChanged += new System.EventHandler(this.ShapesComboBoxSelectedIndexChanged);
            // 
            // Save
            // 
            this.Save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Save.Location = new System.Drawing.Point(3, 233);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(121, 40);
            this.Save.TabIndex = 3;
            this.Save.Text = "Save Image";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.SaveClick);
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(3, 3);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(316, 276);
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            this.canvas.Click += new System.EventHandler(this.OnCanvasClick);
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.OnCanvasPaint);
            // 
            // DrawingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 282);
            this.Controls.Add(this.mainLayout);
            this.Name = "DrawingForm";
            this.Text = "ЛУЧШЕ ИСПОЛЬЗУЙТЕ PAINT!!!!!!!!!!!!!!!!!!!!!";
            this.mainLayout.ResumeLayout(false);
            this.toolLayot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.TableLayoutPanel toolLayot;
        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.ListBox itemsListBox;
        private System.Windows.Forms.ComboBox shapesComboBox;
        private System.Windows.Forms.Button Save;
    }
}