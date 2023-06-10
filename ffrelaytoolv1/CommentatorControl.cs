using ffrelaytoolv1.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public partial class RichNameControl : UserControl
    {
        private Color InactiveSpeaker = ColorTranslator.FromHtml("#606060");

        private PictureBox flagLabel;
        private Label nameLabel;
        private Label pronounsLabel;

        private string userName;

        private Color disabledColor;
        public RichNameControl()
        {
            InitializeComponent();
        }

        public void setupNameControl(LabelUtil labelUtil, UserDetails user, MetaContext context, bool smallPadding)
        {
            //setup sizing
            //assign flag
            //create label with name
            //expose methods to "enable"/"disable" name colouring
            disabledColor = ColorTranslator.FromHtml(context.layout.timerFadeColor);
            userName = user.Name;
            var userText = user.Name;
            var pronounText = $"({user.Pronouns})";
            var nameLabelSize = TextRenderer.MeasureText(userText, labelUtil.activeFontSized(context.layout.defaultTimerFontSize));
            var pronounLabelSize = TextRenderer.MeasureText(pronounText, labelUtil.activeFontSized(context.layout.defaultTimerSubFontSize));
            flagLabel = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(0, 0),
                Image = (Image)Resources.ResourceManager.GetObject(user.Flag),
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            var defaultFlagSize = flagLabel.Size;
            nameLabel = labelUtil.createBaseLabel(defaultFlagSize.Width, 0, nameLabelSize.Width, nameLabelSize.Height, userText, ContentAlignment.MiddleLeft, context.layout.defaultTimerFontSize);
            pronounsLabel = labelUtil.createBaseLabel(defaultFlagSize.Width + nameLabelSize.Width, 0, pronounLabelSize.Width, pronounLabelSize.Height, pronounText, ContentAlignment.MiddleLeft, context.layout.defaultTimerSubFontSize);
            Controls.Add(flagLabel);
            Controls.Add(nameLabel);
            Controls.Add(pronounsLabel);
            Size = new Size(nameLabelSize.Width + defaultFlagSize.Width + pronounLabelSize.Width, Math.Max(nameLabelSize.Height, defaultFlagSize.Height));
            if (smallPadding)
            {
                Padding = new Padding(0, 0, 0, 0);
                Margin = new Padding(0);
            }
        }

        public void setSpeaking(bool speaking) {
            if (!speaking)
            {
                nameLabel.ForeColor = disabledColor;
                pronounsLabel.ForeColor = disabledColor;
                nameLabel.Font = new Font(nameLabel.Font, FontStyle.Regular);
                pronounsLabel.Font = new Font(pronounsLabel.Font, FontStyle.Regular);
            } else
            {
                nameLabel.ForeColor = Color.Black;
                pronounsLabel.ForeColor = Color.Black;
                nameLabel.Font = new Font(nameLabel.Font, FontStyle.Underline);
                pronounsLabel.Font = new Font(pronounsLabel.Font, FontStyle.Underline);
            }
        }

        public string getUserName()
        {
            return userName;
        }
    }
}
