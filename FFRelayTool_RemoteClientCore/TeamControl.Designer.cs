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
            TeamLabel = new System.Windows.Forms.Label();
            Current = new System.Windows.Forms.Label();
            splitLabel = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // TeamLabel
            // 
            TeamLabel.AutoSize = true;
            TeamLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TeamLabel.Location = new System.Drawing.Point(4, 0);
            TeamLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            TeamLabel.Name = "TeamLabel";
            TeamLabel.Size = new System.Drawing.Size(79, 26);
            TeamLabel.TabIndex = 0;
            TeamLabel.Text = "Team: ";
            // 
            // Current
            // 
            Current.AutoSize = true;
            Current.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            Current.Location = new System.Drawing.Point(4, 47);
            Current.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Current.Name = "Current";
            Current.Size = new System.Drawing.Size(98, 20);
            Current.TabIndex = 1;
            Current.Text = "Current split:";
            // 
            // splitLabel
            // 
            splitLabel.AutoSize = true;
            splitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            splitLabel.Location = new System.Drawing.Point(5, 77);
            splitLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            splitLabel.Name = "splitLabel";
            splitLabel.Size = new System.Drawing.Size(35, 20);
            splitLabel.TabIndex = 2;
            splitLabel.Text = "N/A";
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(7, 106);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(164, 63);
            button1.TabIndex = 3;
            button1.Text = "Team split";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // TeamControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(button1);
            Controls.Add(splitLabel);
            Controls.Add(Current);
            Controls.Add(TeamLabel);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TeamControl";
            Size = new System.Drawing.Size(178, 173);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label TeamLabel;
        private System.Windows.Forms.Label Current;
        private System.Windows.Forms.Label splitLabel;
        private System.Windows.Forms.Button button1;
    }
}
