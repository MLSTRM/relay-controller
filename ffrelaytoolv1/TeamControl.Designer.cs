﻿namespace ffrelaytoolv1
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
            this.TeamSplitButton = new System.Windows.Forms.Button();
            this.TimerLabel = new System.Windows.Forms.Label();
            this.teamTabGroup = new System.Windows.Forms.TabControl();
            this.cycleIconButton = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TeamSplitButton
            // 
            this.TeamSplitButton.BackColor = System.Drawing.Color.CornflowerBlue;
            this.TeamSplitButton.Location = new System.Drawing.Point(384, 8);
            this.TeamSplitButton.Name = "TeamSplitButton";
            this.TeamSplitButton.Size = new System.Drawing.Size(357, 64);
            this.TeamSplitButton.TabIndex = 17;
            this.TeamSplitButton.Text = "Team Mog Split";
            this.TeamSplitButton.UseVisualStyleBackColor = false;
            this.TeamSplitButton.Click += new System.EventHandler(this.TeamSplitButton_Click);
            // 
            // TimerLabel
            // 
            this.TimerLabel.BackColor = System.Drawing.Color.White;
            this.TimerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F);
            this.TimerLabel.Location = new System.Drawing.Point(12, 8);
            this.TimerLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(254, 64);
            this.TimerLabel.TabIndex = 18;
            this.TimerLabel.Text = "00:00:00";
            this.TimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // teamTabGroup
            // 
            this.teamTabGroup.Location = new System.Drawing.Point(8, 100);
            this.teamTabGroup.Name = "teamTabGroup";
            this.teamTabGroup.SelectedIndex = 0;
            this.teamTabGroup.Size = new System.Drawing.Size(813, 192);
            this.teamTabGroup.TabIndex = 16;
            // 
            // cycleIconButton
            // 
            this.cycleIconButton.Location = new System.Drawing.Point(296, 8);
            this.cycleIconButton.Name = "cycleIconButton";
            this.cycleIconButton.Size = new System.Drawing.Size(64, 64);
            this.cycleIconButton.TabIndex = 19;
            this.cycleIconButton.Text = "button1";
            this.cycleIconButton.UseVisualStyleBackColor = true;
            this.cycleIconButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(758, 8);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(59, 63);
            this.undoButton.TabIndex = 20;
            this.undoButton.Text = "Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // TeamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.cycleIconButton);
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.TeamSplitButton);
            this.Controls.Add(this.teamTabGroup);
            this.Name = "TeamControl";
            this.Size = new System.Drawing.Size(827, 302);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TeamSplitButton;
        private System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.TabControl teamTabGroup;
        private System.Windows.Forms.Button cycleIconButton;
        private System.Windows.Forms.Button undoButton;
    }
}
