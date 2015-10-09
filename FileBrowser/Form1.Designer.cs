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
            this.left = new System.Windows.Forms.CheckedListBox();
            this.right = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // left
            // 
            this.left.FormattingEnabled = true;
            this.left.Location = new System.Drawing.Point(5, 5);
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(490, 709);
            this.left.TabIndex = 0;
            this.left.MouseClick += new System.Windows.Forms.MouseEventHandler(this.left_MouseClick);
            this.left.DoubleClick += new System.EventHandler(this.left_DoubleClick);
            // 
            // right
            // 
            this.right.FormattingEnabled = true;
            this.right.Location = new System.Drawing.Point(685, 5);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(490, 709);
            this.right.TabIndex = 1;
            this.right.MouseClick += new System.Windows.Forms.MouseEventHandler(this.right_MouseClick);
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

        private System.Windows.Forms.CheckedListBox left;
        private System.Windows.Forms.CheckedListBox right;

    }
}

