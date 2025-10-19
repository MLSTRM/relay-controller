namespace FFRelayTool_RemoteClient
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusHeader = new System.Windows.Forms.Label();
            this.eventStart = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusHeader
            // 
            this.statusHeader.AutoSize = true;
            this.statusHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusHeader.Location = new System.Drawing.Point(12, 21);
            this.statusHeader.Name = "statusHeader";
            this.statusHeader.Size = new System.Drawing.Size(74, 26);
            this.statusHeader.TabIndex = 3;
            this.statusHeader.Text = "Status";
            // 
            // eventStart
            // 
            this.eventStart.AutoSize = true;
            this.eventStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventStart.Location = new System.Drawing.Point(12, 57);
            this.eventStart.Name = "eventStart";
            this.eventStart.Size = new System.Drawing.Size(112, 20);
            this.eventStart.TabIndex = 4;
            this.eventStart.Text = "Event started: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(509, 136);
            this.label1.TabIndex = 5;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 396);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eventStart);
            this.Controls.Add(this.statusHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label statusHeader;
        private System.Windows.Forms.Label eventStart;
        private System.Windows.Forms.Label label1;
    }
}

