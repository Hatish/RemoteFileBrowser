namespace FileBrowser
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
            this.left = new System.Windows.Forms.ListBox();
            this.right = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // left
            // 
            this.left.FormattingEnabled = true;
            this.left.Location = new System.Drawing.Point(13, 13);
            this.left.Name = "left";
            this.left.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.left.Size = new System.Drawing.Size(500, 706);
            this.left.TabIndex = 2;
            this.left.DoubleClick += new System.EventHandler(this.left_DoubleClick);
            // 
            // right
            // 
            this.right.FormattingEnabled = true;
            this.right.Location = new System.Drawing.Point(672, 12);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(500, 706);
            this.right.TabIndex = 3;
            this.right.DoubleClick += new System.EventHandler(this.right_DoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 729);
            this.Controls.Add(this.right);
            this.Controls.Add(this.left);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox left;
        private System.Windows.Forms.ListBox right;


    }
}

