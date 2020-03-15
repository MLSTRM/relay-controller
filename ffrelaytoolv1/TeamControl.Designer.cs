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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamControl));
            this.TeamSplitButton = new System.Windows.Forms.Button();
            this.TimerLabel = new System.Windows.Forms.Label();
            this.tabPageTimes = new System.Windows.Forms.TabPage();
            this.tabPageCategories = new System.Windows.Forms.TabPage();
            this.tabPageSplits = new System.Windows.Forms.TabPage();
            this.teamTabGroup = new System.Windows.Forms.TabControl();
            this.cycleIconButton = new System.Windows.Forms.Button();
            this.teamTabGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // TeamSplitButton
            // 
            this.TeamSplitButton.BackColor = System.Drawing.Color.CornflowerBlue;
            this.TeamSplitButton.Location = new System.Drawing.Point(12, 86);
            this.TeamSplitButton.Name = "TeamSplitButton";
            this.TeamSplitButton.Size = new System.Drawing.Size(400, 54);
            this.TeamSplitButton.TabIndex = 17;
            this.TeamSplitButton.Text = "Team Mog Split";
            this.TeamSplitButton.UseVisualStyleBackColor = false;
            this.TeamSplitButton.Click += new System.EventHandler(this.TeamSplitButton_Click);
            // 
            // TimerLabel
            // 
            this.TimerLabel.BackColor = System.Drawing.Color.White;
            this.TimerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F);
            this.TimerLabel.Location = new System.Drawing.Point(85, 7);
            this.TimerLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(254, 64);
            this.TimerLabel.TabIndex = 18;
            this.TimerLabel.Text = "00:00:00";
            this.TimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPageTimes
            // 
            this.tabPageTimes.BackColor = System.Drawing.Color.Black;
            this.tabPageTimes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageTimes.BackgroundImage")));
            this.tabPageTimes.Location = new System.Drawing.Point(4, 22);
            this.tabPageTimes.Name = "tabPageTimes";
            this.tabPageTimes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimes.Size = new System.Drawing.Size(400, 228);
            this.tabPageTimes.TabIndex = 3;
            this.tabPageTimes.Text = "M: Times";
            // 
            // tabPageCategories
            // 
            this.tabPageCategories.BackColor = System.Drawing.Color.Black;
            this.tabPageCategories.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageCategories.BackgroundImage")));
            this.tabPageCategories.Location = new System.Drawing.Point(4, 22);
            this.tabPageCategories.Name = "tabPageCategories";
            this.tabPageCategories.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCategories.Size = new System.Drawing.Size(400, 228);
            this.tabPageCategories.TabIndex = 1;
            this.tabPageCategories.Text = "M: Cats";
            // 
            // tabPageSplits
            // 
            this.tabPageSplits.BackColor = System.Drawing.Color.Black;
            this.tabPageSplits.Location = new System.Drawing.Point(4, 22);
            this.tabPageSplits.Name = "tabPageSplits";
            this.tabPageSplits.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSplits.Size = new System.Drawing.Size(400, 228);
            this.tabPageSplits.TabIndex = 0;
            this.tabPageSplits.Text = "M: Splits";
            // 
            // teamTabGroup
            // 
            this.teamTabGroup.Controls.Add(this.tabPageSplits);
            this.teamTabGroup.Controls.Add(this.tabPageCategories);
            this.teamTabGroup.Controls.Add(this.tabPageTimes);
            this.teamTabGroup.Location = new System.Drawing.Point(8, 170);
            this.teamTabGroup.Name = "teamTabGroup";
            this.teamTabGroup.SelectedIndex = 0;
            this.teamTabGroup.Size = new System.Drawing.Size(408, 254);
            this.teamTabGroup.TabIndex = 16;
            // 
            // cycleIconButton
            // 
            this.cycleIconButton.Location = new System.Drawing.Point(348, 7);
            this.cycleIconButton.Name = "cycleIconButton";
            this.cycleIconButton.Size = new System.Drawing.Size(64, 64);
            this.cycleIconButton.TabIndex = 19;
            this.cycleIconButton.Text = "button1";
            this.cycleIconButton.UseVisualStyleBackColor = true;
            this.cycleIconButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // TeamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cycleIconButton);
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.TeamSplitButton);
            this.Controls.Add(this.teamTabGroup);
            this.Name = "TeamControl";
            this.Size = new System.Drawing.Size(430, 430);
            this.teamTabGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TeamSplitButton;
        private System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.TabPage tabPageTimes;
        private System.Windows.Forms.TabPage tabPageCategories;
        private System.Windows.Forms.TabPage tabPageSplits;
        private System.Windows.Forms.TabControl teamTabGroup;
        private System.Windows.Forms.Button cycleIconButton;


    }
}