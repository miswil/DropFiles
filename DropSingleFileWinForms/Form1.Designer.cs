namespace DropSingleFileWinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new TextBox();
            this.splitContainer1 = new SplitContainer();
            this.label1 = new Label();
            this.button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Dock = DockStyle.Top;
            this.textBox1.Location = new Point(20, 20);
            this.textBox1.Margin = new Padding(20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(760, 200);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "This text will be written to a file.";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new Point(20, 220);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = Color.Yellow;
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.MouseMove += this.splitContainer1_Panel1_MouseMove;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Size = new Size(760, 210);
            this.splitContainer1.SplitterDistance = 383;
            this.splitContainer1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Yu Gothic UI", 36F);
            this.label1.Location = new Point(34, 56);
            this.label1.Name = "label1";
            this.label1.Size = new Size(314, 96);
            this.label1.TabIndex = 0;
            this.label1.Text = "Drag me";
            this.label1.MouseMove += this.label1_MouseMove;
            // 
            // button1
            // 
            this.button1.Dock = DockStyle.Fill;
            this.button1.Font = new Font("Yu Gothic UI", 36F);
            this.button1.Location = new Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new Size(373, 210);
            this.button1.TabIndex = 0;
            this.button1.Text = "Copy file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += this.button1_Click;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(10F, 25F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Padding = new Padding(20);
            this.Text = "DropSingleFileWinForms";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private SplitContainer splitContainer1;
        private Label label1;
        private Button button1;
    }
}
