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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.left = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.right = new System.Windows.Forms.ListView();
            this.copyLeftToRight = new System.Windows.Forms.Button();
            this.leftBox = new System.Windows.Forms.TextBox();
            this.rightBox = new System.Windows.Forms.TextBox();
            this.jobProgress = new System.Windows.Forms.ProgressBar();
            this.commentBox = new System.Windows.Forms.TextBox();
            this.fileProgress = new System.Windows.Forms.ProgressBar();
            this.moveLeftToRight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // left
            // 
            this.left.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.left.Location = new System.Drawing.Point(13, 39);
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(500, 594);
            this.left.SmallImageList = this.imageList1;
            this.left.TabIndex = 4;
            this.left.UseCompatibleStateImageBehavior = false;
            this.left.View = System.Windows.Forms.View.List;
            this.left.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.left_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "6.png");
            this.imageList1.Images.SetKeyName(1, "Explorer.png");
            this.imageList1.Images.SetKeyName(2, "Notepad.png");
            this.imageList1.Images.SetKeyName(3, "System HD.png");
            // 
            // right
            // 
            this.right.Location = new System.Drawing.Point(672, 39);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(500, 594);
            this.right.SmallImageList = this.imageList1;
            this.right.TabIndex = 5;
            this.right.UseCompatibleStateImageBehavior = false;
            this.right.View = System.Windows.Forms.View.List;
            this.right.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.right_MouseDoubleClick);
            // 
            // copyLeftToRight
            // 
            this.copyLeftToRight.Location = new System.Drawing.Point(519, 39);
            this.copyLeftToRight.Name = "copyLeftToRight";
            this.copyLeftToRight.Size = new System.Drawing.Size(147, 51);
            this.copyLeftToRight.TabIndex = 6;
            this.copyLeftToRight.Text = "Copy >>";
            this.copyLeftToRight.UseVisualStyleBackColor = true;
            this.copyLeftToRight.Click += new System.EventHandler(this.copyLeftToRight_Click);
            // 
            // leftBox
            // 
            this.leftBox.Location = new System.Drawing.Point(13, 13);
            this.leftBox.Name = "leftBox";
            this.leftBox.ReadOnly = true;
            this.leftBox.Size = new System.Drawing.Size(500, 20);
            this.leftBox.TabIndex = 7;
            // 
            // rightBox
            // 
            this.rightBox.Location = new System.Drawing.Point(672, 13);
            this.rightBox.Name = "rightBox";
            this.rightBox.ReadOnly = true;
            this.rightBox.Size = new System.Drawing.Size(500, 20);
            this.rightBox.TabIndex = 8;
            // 
            // jobProgress
            // 
            this.jobProgress.Location = new System.Drawing.Point(13, 668);
            this.jobProgress.Name = "jobProgress";
            this.jobProgress.Size = new System.Drawing.Size(1159, 23);
            this.jobProgress.TabIndex = 9;
            // 
            // commentBox
            // 
            this.commentBox.Location = new System.Drawing.Point(13, 697);
            this.commentBox.Name = "commentBox";
            this.commentBox.ReadOnly = true;
            this.commentBox.Size = new System.Drawing.Size(1159, 20);
            this.commentBox.TabIndex = 10;
            // 
            // fileProgress
            // 
            this.fileProgress.Location = new System.Drawing.Point(13, 639);
            this.fileProgress.Name = "fileProgress";
            this.fileProgress.Size = new System.Drawing.Size(1159, 23);
            this.fileProgress.TabIndex = 11;
            // 
            // moveLeftToRight
            // 
            this.moveLeftToRight.Location = new System.Drawing.Point(519, 96);
            this.moveLeftToRight.Name = "moveLeftToRight";
            this.moveLeftToRight.Size = new System.Drawing.Size(147, 51);
            this.moveLeftToRight.TabIndex = 12;
            this.moveLeftToRight.Text = "Move >>";
            this.moveLeftToRight.UseVisualStyleBackColor = true;
            this.moveLeftToRight.Click += new System.EventHandler(this.moveLeftToRight_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 729);
            this.Controls.Add(this.moveLeftToRight);
            this.Controls.Add(this.fileProgress);
            this.Controls.Add(this.commentBox);
            this.Controls.Add(this.jobProgress);
            this.Controls.Add(this.rightBox);
            this.Controls.Add(this.leftBox);
            this.Controls.Add(this.copyLeftToRight);
            this.Controls.Add(this.right);
            this.Controls.Add(this.left);
            this.Name = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView left;
        private System.Windows.Forms.ListView right;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button copyLeftToRight;
        private System.Windows.Forms.TextBox leftBox;
        private System.Windows.Forms.TextBox rightBox;
        private System.Windows.Forms.ProgressBar jobProgress;
        private System.Windows.Forms.TextBox commentBox;
        private System.Windows.Forms.ProgressBar fileProgress;
        private System.Windows.Forms.Button moveLeftToRight;


    }
}

