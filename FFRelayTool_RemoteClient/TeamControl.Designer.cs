namespace FFRelayTool_RemoteClient
{
    partial class TeamControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TeamLabel = new System.Windows.Forms.Label();
            this.Current = new System.Windows.Forms.Label();
            this.splitLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TeamLabel
            // 
            this.TeamLabel.AutoSize = true;
            this.TeamLabel.Location = new System.Drawing.Point(3, 0);
            this.TeamLabel.Name = "TeamLabel";
            this.TeamLabel.Size = new System.Drawing.Size(40, 13);
            this.TeamLabel.TabIndex = 0;
            this.TeamLabel.Text = "Team: ";
            // 
            // Current
            // 
            this.Current.AutoSize = true;
            this.Current.Location = new System.Drawing.Point(4, 17);
            this.Current.Name = "Current";
            this.Current.Size = new System.Drawing.Size(65, 13);
            this.Current.TabIndex = 1;
            this.Current.Text = "Current split:";
            // 
            // splitLabel
            // 
            this.splitLabel.AutoSize = true;
            this.splitLabel.Location = new System.Drawing.Point(7, 34);
            this.splitLabel.Name = "splitLabel";
            this.splitLabel.Size = new System.Drawing.Size(27, 13);
            this.splitLabel.TabIndex = 2;
            this.splitLabel.Text = "N/A";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 55);
            this.button1.TabIndex = 3;
            this.button1.Text = "Team split";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TeamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitLabel);
            this.Controls.Add(this.Current);
            this.Controls.Add(this.TeamLabel);
            this.Name = "TeamControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TeamLabel;
        private System.Windows.Forms.Label Current;
        private System.Windows.Forms.Label splitLabel;
        private System.Windows.Forms.Button button1;
    }
}
