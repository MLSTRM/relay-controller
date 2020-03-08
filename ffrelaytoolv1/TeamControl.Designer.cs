namespace ffrelaytoolv1
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
            this.gameTimesL = new System.Windows.Forms.Label();
            this.gameTimesR = new System.Windows.Forms.Label();
            this.GameTitlesR2 = new System.Windows.Forms.Label();
            this.GameTitlesL2 = new System.Windows.Forms.Label();
            this.GameTitlesR1 = new System.Windows.Forms.Label();
            this.GameTitlesL1 = new System.Windows.Forms.Label();
            this.tabPageCategories = new System.Windows.Forms.TabPage();
            this.CommentatorsText = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.CategoryInfo2 = new System.Windows.Forms.Label();
            this.CategoryInfo1 = new System.Windows.Forms.Label();
            this.CategoryInfo3 = new System.Windows.Forms.Label();
            this.tabPageSplits = new System.Windows.Forms.TabPage();
            this.teamVsText2 = new System.Windows.Forms.Label();
            this.teamVsText1 = new System.Windows.Forms.Label();
            this.teamVsTime2 = new System.Windows.Forms.Label();
            this.teamVsTime1 = new System.Windows.Forms.Label();
            this.splitTime4 = new System.Windows.Forms.Label();
            this.splitText4 = new System.Windows.Forms.Label();
            this.splitTime3 = new System.Windows.Forms.Label();
            this.splitTime2 = new System.Windows.Forms.Label();
            this.splitTime1 = new System.Windows.Forms.Label();
            this.splitText3 = new System.Windows.Forms.Label();
            this.splitText2 = new System.Windows.Forms.Label();
            this.splitText1 = new System.Windows.Forms.Label();
            this.teamTabGroup = new System.Windows.Forms.TabControl();
            this.tabPageTimes.SuspendLayout();
            this.tabPageCategories.SuspendLayout();
            this.tabPageSplits.SuspendLayout();
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
            this.tabPageTimes.Controls.Add(this.gameTimesL);
            this.tabPageTimes.Controls.Add(this.gameTimesR);
            this.tabPageTimes.Controls.Add(this.GameTitlesR2);
            this.tabPageTimes.Controls.Add(this.GameTitlesL2);
            this.tabPageTimes.Controls.Add(this.GameTitlesR1);
            this.tabPageTimes.Controls.Add(this.GameTitlesL1);
            this.tabPageTimes.Location = new System.Drawing.Point(4, 22);
            this.tabPageTimes.Name = "tabPageTimes";
            this.tabPageTimes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTimes.Size = new System.Drawing.Size(400, 228);
            this.tabPageTimes.TabIndex = 3;
            this.tabPageTimes.Text = "M: Times";
            // 
            // gameTimesL
            // 
            this.gameTimesL.BackColor = System.Drawing.Color.Transparent;
            this.gameTimesL.Font = new System.Drawing.Font("Lucida Sans Typewriter", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameTimesL.ForeColor = System.Drawing.Color.White;
            this.gameTimesL.Location = new System.Drawing.Point(76, 11);
            this.gameTimesL.Name = "gameTimesL";
            this.gameTimesL.Size = new System.Drawing.Size(109, 147);
            this.gameTimesL.TabIndex = 32;
            this.gameTimesL.Text = "00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00";
            this.gameTimesL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gameTimesR
            // 
            this.gameTimesR.BackColor = System.Drawing.Color.Transparent;
            this.gameTimesR.Font = new System.Drawing.Font("Lucida Sans Typewriter", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameTimesR.ForeColor = System.Drawing.Color.White;
            this.gameTimesR.Location = new System.Drawing.Point(204, 11);
            this.gameTimesR.Name = "gameTimesR";
            this.gameTimesR.Size = new System.Drawing.Size(109, 147);
            this.gameTimesR.TabIndex = 31;
            this.gameTimesR.Text = "00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00\r\n00:00:00";
            this.gameTimesR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GameTitlesR2
            // 
            this.GameTitlesR2.BackColor = System.Drawing.Color.Transparent;
            this.GameTitlesR2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 13F);
            this.GameTitlesR2.ForeColor = System.Drawing.Color.White;
            this.GameTitlesR2.Location = new System.Drawing.Point(295, 93);
            this.GameTitlesR2.Name = "GameTitlesR2";
            this.GameTitlesR2.Size = new System.Drawing.Size(90, 62);
            this.GameTitlesR2.TabIndex = 10;
            this.GameTitlesR2.Text = " : 13-2\r\n : DoC\r\n : XII";
            this.GameTitlesR2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GameTitlesL2
            // 
            this.GameTitlesL2.BackColor = System.Drawing.Color.Transparent;
            this.GameTitlesL2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 13F);
            this.GameTitlesL2.ForeColor = System.Drawing.Color.White;
            this.GameTitlesL2.Location = new System.Drawing.Point(3, 93);
            this.GameTitlesL2.Name = "GameTitlesL2";
            this.GameTitlesL2.Size = new System.Drawing.Size(90, 62);
            this.GameTitlesL2.TabIndex = 9;
            this.GameTitlesL2.Text = "II : \r\nCT : \r\nDis : ";
            this.GameTitlesL2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GameTitlesR1
            // 
            this.GameTitlesR1.BackColor = System.Drawing.Color.Transparent;
            this.GameTitlesR1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 13F);
            this.GameTitlesR1.ForeColor = System.Drawing.Color.White;
            this.GameTitlesR1.Location = new System.Drawing.Point(295, 9);
            this.GameTitlesR1.Name = "GameTitlesR1";
            this.GameTitlesR1.Size = new System.Drawing.Size(90, 80);
            this.GameTitlesR1.TabIndex = 5;
            this.GameTitlesR1.Text = " : LR\r\n : IX\r\n : T-0\r\n : I";
            this.GameTitlesR1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GameTitlesL1
            // 
            this.GameTitlesL1.BackColor = System.Drawing.Color.Transparent;
            this.GameTitlesL1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 13F);
            this.GameTitlesL1.ForeColor = System.Drawing.Color.White;
            this.GameTitlesL1.Location = new System.Drawing.Point(3, 9);
            this.GameTitlesL1.Name = "GameTitlesL1";
            this.GameTitlesL1.Size = new System.Drawing.Size(90, 80);
            this.GameTitlesL1.TabIndex = 1;
            this.GameTitlesL1.Text = "CC7 : \r\nFFCC : \r\nX-2 : \r\nWoFF : ";
            this.GameTitlesL1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPageCategories
            // 
            this.tabPageCategories.BackColor = System.Drawing.Color.Black;
            this.tabPageCategories.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageCategories.BackgroundImage")));
            this.tabPageCategories.Controls.Add(this.CommentatorsText);
            this.tabPageCategories.Controls.Add(this.label25);
            this.tabPageCategories.Controls.Add(this.CategoryInfo2);
            this.tabPageCategories.Controls.Add(this.CategoryInfo1);
            this.tabPageCategories.Controls.Add(this.CategoryInfo3);
            this.tabPageCategories.Location = new System.Drawing.Point(4, 22);
            this.tabPageCategories.Name = "tabPageCategories";
            this.tabPageCategories.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCategories.Size = new System.Drawing.Size(400, 228);
            this.tabPageCategories.TabIndex = 1;
            this.tabPageCategories.Text = "M: Cats";
            // 
            // CommentatorsText
            // 
            this.CommentatorsText.BackColor = System.Drawing.Color.Transparent;
            this.CommentatorsText.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.CommentatorsText.ForeColor = System.Drawing.Color.White;
            this.CommentatorsText.Location = new System.Drawing.Point(3, 96);
            this.CommentatorsText.Name = "CommentatorsText";
            this.CommentatorsText.Size = new System.Drawing.Size(394, 68);
            this.CommentatorsText.TabIndex = 7;
            this.CommentatorsText.Text = "Commentators";
            this.CommentatorsText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.Transparent;
            this.label25.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.label25.ForeColor = System.Drawing.Color.White;
            this.label25.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label25.Location = new System.Drawing.Point(3, 108);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(391, 35);
            this.label25.TabIndex = 6;
            this.label25.Text = "Commentators:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label25.Visible = false;
            // 
            // CategoryInfo2
            // 
            this.CategoryInfo2.BackColor = System.Drawing.Color.Transparent;
            this.CategoryInfo2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.CategoryInfo2.ForeColor = System.Drawing.Color.White;
            this.CategoryInfo2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CategoryInfo2.Location = new System.Drawing.Point(3, 36);
            this.CategoryInfo2.Name = "CategoryInfo2";
            this.CategoryInfo2.Size = new System.Drawing.Size(391, 35);
            this.CategoryInfo2.TabIndex = 2;
            this.CategoryInfo2.Text = "MogCategory";
            this.CategoryInfo2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CategoryInfo1
            // 
            this.CategoryInfo1.BackColor = System.Drawing.Color.Transparent;
            this.CategoryInfo1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.CategoryInfo1.ForeColor = System.Drawing.Color.White;
            this.CategoryInfo1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CategoryInfo1.Location = new System.Drawing.Point(3, 3);
            this.CategoryInfo1.Name = "CategoryInfo1";
            this.CategoryInfo1.Size = new System.Drawing.Size(391, 35);
            this.CategoryInfo1.TabIndex = 1;
            this.CategoryInfo1.Text = "MogCategory";
            this.CategoryInfo1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CategoryInfo3
            // 
            this.CategoryInfo3.BackColor = System.Drawing.Color.Transparent;
            this.CategoryInfo3.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.CategoryInfo3.ForeColor = System.Drawing.Color.White;
            this.CategoryInfo3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CategoryInfo3.Location = new System.Drawing.Point(3, 66);
            this.CategoryInfo3.Name = "CategoryInfo3";
            this.CategoryInfo3.Size = new System.Drawing.Size(391, 35);
            this.CategoryInfo3.TabIndex = 0;
            this.CategoryInfo3.Text = "MogCategory";
            this.CategoryInfo3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPageSplits
            // 
            this.tabPageSplits.BackColor = System.Drawing.Color.Black;
            this.tabPageSplits.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageSplits.BackgroundImage")));
            this.tabPageSplits.Controls.Add(this.teamVsText2);
            this.tabPageSplits.Controls.Add(this.teamVsText1);
            this.tabPageSplits.Controls.Add(this.teamVsTime2);
            this.tabPageSplits.Controls.Add(this.teamVsTime1);
            this.tabPageSplits.Controls.Add(this.splitTime4);
            this.tabPageSplits.Controls.Add(this.splitText4);
            this.tabPageSplits.Controls.Add(this.splitTime3);
            this.tabPageSplits.Controls.Add(this.splitTime2);
            this.tabPageSplits.Controls.Add(this.splitTime1);
            this.tabPageSplits.Controls.Add(this.splitText3);
            this.tabPageSplits.Controls.Add(this.splitText2);
            this.tabPageSplits.Controls.Add(this.splitText1);
            this.tabPageSplits.Location = new System.Drawing.Point(4, 22);
            this.tabPageSplits.Name = "tabPageSplits";
            this.tabPageSplits.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSplits.Size = new System.Drawing.Size(400, 228);
            this.tabPageSplits.TabIndex = 0;
            this.tabPageSplits.Text = "M: Splits";
            // 
            // teamVsText2
            // 
            this.teamVsText2.BackColor = System.Drawing.Color.Transparent;
            this.teamVsText2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.teamVsText2.ForeColor = System.Drawing.Color.White;
            this.teamVsText2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.teamVsText2.Location = new System.Drawing.Point(3, 186);
            this.teamVsText2.Name = "teamVsText2";
            this.teamVsText2.Size = new System.Drawing.Size(228, 29);
            this.teamVsText2.TabIndex = 15;
            this.teamVsText2.Text = "Team Tonberry:";
            this.teamVsText2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // teamVsText1
            // 
            this.teamVsText1.BackColor = System.Drawing.Color.Transparent;
            this.teamVsText1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.teamVsText1.ForeColor = System.Drawing.Color.White;
            this.teamVsText1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.teamVsText1.Location = new System.Drawing.Point(3, 156);
            this.teamVsText1.Name = "teamVsText1";
            this.teamVsText1.Size = new System.Drawing.Size(230, 29);
            this.teamVsText1.TabIndex = 14;
            this.teamVsText1.Text = "Vs Team Choco:";
            this.teamVsText1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // teamVsTime2
            // 
            this.teamVsTime2.BackColor = System.Drawing.Color.Transparent;
            this.teamVsTime2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.teamVsTime2.ForeColor = System.Drawing.Color.White;
            this.teamVsTime2.Location = new System.Drawing.Point(242, 186);
            this.teamVsTime2.Name = "teamVsTime2";
            this.teamVsTime2.Size = new System.Drawing.Size(140, 29);
            this.teamVsTime2.TabIndex = 12;
            this.teamVsTime2.Text = "00:00:00";
            this.teamVsTime2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // teamVsTime1
            // 
            this.teamVsTime1.BackColor = System.Drawing.Color.Transparent;
            this.teamVsTime1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.teamVsTime1.ForeColor = System.Drawing.Color.White;
            this.teamVsTime1.Location = new System.Drawing.Point(242, 156);
            this.teamVsTime1.Name = "teamVsTime1";
            this.teamVsTime1.Size = new System.Drawing.Size(140, 29);
            this.teamVsTime1.TabIndex = 11;
            this.teamVsTime1.Text = "00:00:00";
            this.teamVsTime1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitTime4
            // 
            this.splitTime4.BackColor = System.Drawing.Color.Transparent;
            this.splitTime4.Font = new System.Drawing.Font("Lucida Sans Typewriter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitTime4.ForeColor = System.Drawing.Color.White;
            this.splitTime4.Location = new System.Drawing.Point(265, 96);
            this.splitTime4.Name = "splitTime4";
            this.splitTime4.Size = new System.Drawing.Size(117, 29);
            this.splitTime4.TabIndex = 9;
            this.splitTime4.Text = "00:00:00";
            this.splitTime4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitText4
            // 
            this.splitText4.BackColor = System.Drawing.Color.Transparent;
            this.splitText4.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.splitText4.ForeColor = System.Drawing.Color.White;
            this.splitText4.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.splitText4.Location = new System.Drawing.Point(3, 96);
            this.splitText4.Name = "splitText4";
            this.splitText4.Size = new System.Drawing.Size(256, 29);
            this.splitText4.TabIndex = 8;
            this.splitText4.Text = "Final Fantasy T-0";
            this.splitText4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitTime3
            // 
            this.splitTime3.BackColor = System.Drawing.Color.Transparent;
            this.splitTime3.Font = new System.Drawing.Font("Lucida Sans Typewriter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitTime3.ForeColor = System.Drawing.Color.White;
            this.splitTime3.Location = new System.Drawing.Point(265, 66);
            this.splitTime3.Name = "splitTime3";
            this.splitTime3.Size = new System.Drawing.Size(117, 29);
            this.splitTime3.TabIndex = 7;
            this.splitTime3.Text = "00:00:00";
            this.splitTime3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitTime2
            // 
            this.splitTime2.BackColor = System.Drawing.Color.Transparent;
            this.splitTime2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitTime2.ForeColor = System.Drawing.Color.White;
            this.splitTime2.Location = new System.Drawing.Point(265, 36);
            this.splitTime2.Name = "splitTime2";
            this.splitTime2.Size = new System.Drawing.Size(117, 29);
            this.splitTime2.TabIndex = 6;
            this.splitTime2.Text = "00:00:00";
            this.splitTime2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitTime1
            // 
            this.splitTime1.BackColor = System.Drawing.Color.Transparent;
            this.splitTime1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitTime1.ForeColor = System.Drawing.Color.White;
            this.splitTime1.Location = new System.Drawing.Point(265, 6);
            this.splitTime1.Name = "splitTime1";
            this.splitTime1.Size = new System.Drawing.Size(117, 29);
            this.splitTime1.TabIndex = 5;
            this.splitTime1.Text = "00:00:00";
            this.splitTime1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitText3
            // 
            this.splitText3.BackColor = System.Drawing.Color.Transparent;
            this.splitText3.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.splitText3.ForeColor = System.Drawing.Color.White;
            this.splitText3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.splitText3.Location = new System.Drawing.Point(3, 66);
            this.splitText3.Name = "splitText3";
            this.splitText3.Size = new System.Drawing.Size(256, 29);
            this.splitText3.TabIndex = 4;
            this.splitText3.Text = "Final Fantasy T0HD";
            this.splitText3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitText2
            // 
            this.splitText2.BackColor = System.Drawing.Color.Transparent;
            this.splitText2.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.splitText2.ForeColor = System.Drawing.Color.White;
            this.splitText2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.splitText2.Location = new System.Drawing.Point(3, 36);
            this.splitText2.Name = "splitText2";
            this.splitText2.Size = new System.Drawing.Size(256, 29);
            this.splitText2.TabIndex = 3;
            this.splitText2.Text = "FF Type-0 HD";
            this.splitText2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitText1
            // 
            this.splitText1.BackColor = System.Drawing.Color.Transparent;
            this.splitText1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 16F);
            this.splitText1.ForeColor = System.Drawing.Color.White;
            this.splitText1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.splitText1.Location = new System.Drawing.Point(3, 6);
            this.splitText1.Name = "splitText1";
            this.splitText1.Size = new System.Drawing.Size(256, 29);
            this.splitText1.TabIndex = 2;
            this.splitText1.Text = "Lightning Returns";
            this.splitText1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // TeamControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.TeamSplitButton);
            this.Controls.Add(this.teamTabGroup);
            this.Name = "TeamControl";
            this.Size = new System.Drawing.Size(426, 429);
            this.tabPageTimes.ResumeLayout(false);
            this.tabPageCategories.ResumeLayout(false);
            this.tabPageSplits.ResumeLayout(false);
            this.teamTabGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TeamSplitButton;
        private System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.TabPage tabPageTimes;
        private System.Windows.Forms.Label gameTimesL;
        private System.Windows.Forms.Label gameTimesR;
        private System.Windows.Forms.Label GameTitlesR2;
        private System.Windows.Forms.Label GameTitlesL2;
        private System.Windows.Forms.Label GameTitlesR1;
        private System.Windows.Forms.Label GameTitlesL1;
        private System.Windows.Forms.TabPage tabPageCategories;
        private System.Windows.Forms.Label CommentatorsText;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label CategoryInfo2;
        private System.Windows.Forms.Label CategoryInfo1;
        private System.Windows.Forms.Label CategoryInfo3;
        private System.Windows.Forms.TabPage tabPageSplits;
        private System.Windows.Forms.Label teamVsText2;
        private System.Windows.Forms.Label teamVsText1;
        private System.Windows.Forms.Label teamVsTime2;
        private System.Windows.Forms.Label teamVsTime1;
        private System.Windows.Forms.Label splitTime4;
        private System.Windows.Forms.Label splitText4;
        private System.Windows.Forms.Label splitTime3;
        private System.Windows.Forms.Label splitTime2;
        private System.Windows.Forms.Label splitTime1;
        private System.Windows.Forms.Label splitText3;
        private System.Windows.Forms.Label splitText2;
        private System.Windows.Forms.Label splitText1;
        private System.Windows.Forms.TabControl teamTabGroup;


    }
}
