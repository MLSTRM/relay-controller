
namespace ffrelaytoolv1
{
    partial class MetaControl
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
            this.metaTabGroup = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // metaTabGroup
            // 
            this.metaTabGroup.Location = new System.Drawing.Point(6, 6);
            this.metaTabGroup.Name = "metaTabGroup";
            this.metaTabGroup.SelectedIndex = 0;
            this.metaTabGroup.Size = new System.Drawing.Size(794, 294);
            this.metaTabGroup.TabIndex = 0;
            // 
            // MetaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metaTabGroup);
            this.Name = "MetaControl";
            this.Size = new System.Drawing.Size(800, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl metaTabGroup;
    }
}
