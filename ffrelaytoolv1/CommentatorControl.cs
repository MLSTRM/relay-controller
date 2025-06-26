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
using System.Xml;

namespace ffrelaytoolv1
{
    public partial class RichNameControl : UserControl
    {
        public enum ColorMode
        {
            DARK,
            LIGHT
        }
        private Color InactiveSpeakerDark = ColorTranslator.FromHtml("#464646");
        private Color InactiveSpeakerLight = ColorTranslator.FromHtml("#FFFFFF");

        private PictureBox flagLabel;
        private Label nameLabel;
        private Label pronounsLabel;
        private bool hasDropShadow;

        private string userName;
        private string discordName;

        private ColorMode colorMode;

        public RichNameControl()
        {
            InitializeComponent();
        }

        private Color getActiveColor(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.DARK:
                    return Color.Black;
                case ColorMode.LIGHT:
                    return Color.White;
            }
            return Color.Gray;
        }

        private Color getShadowColor(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.DARK:
                    return Color.White;
                case ColorMode.LIGHT:
                    return Color.Black;
            }
            return Color.Gray;
        }

        private Color getDisabledColor(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.DARK:
                    return InactiveSpeakerDark;
                case ColorMode.LIGHT:
                    return InactiveSpeakerLight;
            }
            return Color.Gray;
        }

        public void preSetup(LabelUtil labelUtil, MetaContext context, bool smallPadding, ColorMode colorMode, bool hasDropShadow = false)
        {
            this.hasDropShadow = hasDropShadow;
            var nameLabelSize = TextRenderer.MeasureText(" ", labelUtil.activeFontSized(context.features.metaControl.commentatorNameSize));
            var pronounLabelSize = TextRenderer.MeasureText(" ", labelUtil.activeFontSized(context.features.metaControl.commentatorPronounSize));
            flagLabel = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(0, 0),
                Image = (Image)Resources.ResourceManager.GetObject("us"),
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            Size defaultFlagSize = flagLabel.Size;
            nameLabel = labelUtil.createBaseLabel(defaultFlagSize.Width, 0, nameLabelSize.Width, nameLabelSize.Height, "", ContentAlignment.MiddleLeft, context.features.metaControl.commentatorNameSize);
            pronounsLabel = labelUtil.createBaseLabel(defaultFlagSize.Width + nameLabelSize.Width, 0, pronounLabelSize.Width, pronounLabelSize.Height, "", ContentAlignment.MiddleLeft, context.features.metaControl.commentatorPronounSize);
            Size = new Size(nameLabelSize.Width + defaultFlagSize.Width + pronounLabelSize.Width, Math.Max(nameLabelSize.Height, defaultFlagSize.Height));
            if (smallPadding)
            {
                Padding = new Padding(0, 0, 0, 0);
                Margin = new Padding(0, 0, 0, 5);
            }
        }

        public void setupNameControl(LabelUtil labelUtil, UserDetails user, MetaContext context, bool smallPadding, ColorMode colorMode, bool basic = false)
        {
            Controls.Clear();
            flagLabel = null;
            this.colorMode = colorMode;
            //setup sizing
            //assign flag
            //create label with name
            //expose methods to "enable"/"disable" name colouring
            userName = user.Name;
            discordName = user.DiscordName;
            var userText = user.Name;
            var pronounText = (!basic && user.Pronouns != null && user.Pronouns.Length > 0) ? $"({user.Pronouns})" : "";
            var nameLabelSize = TextRenderer.MeasureText(userText, labelUtil.activeFontSized(context.features.metaControl.commentatorNameSize));
            var pronounLabelSize = TextRenderer.MeasureText(pronounText, labelUtil.activeFontSized(context.features.metaControl.commentatorPronounSize));
            Size defaultFlagSize = new Size(0, 0);
            if (!basic && user.Flag != null && user.Flag.Length > 0)
            {
                flagLabel = new PictureBox
                {
                    BackColor = Color.Transparent,
                    Location = new Point(0, 0),
                    Image = (Image)Resources.ResourceManager.GetObject(user.Flag),
                    SizeMode = PictureBoxSizeMode.AutoSize
                };
                defaultFlagSize = flagLabel.Size;
                Controls.Add(flagLabel);
            }
            if (hasDropShadow)
            {
                nameLabel = labelUtil.createBaseDropShadowLabel(defaultFlagSize.Width, 0, nameLabelSize.Width, nameLabelSize.Height,
                    userText, ContentAlignment.MiddleLeft,
                    getActiveColor(colorMode), getShadowColor(colorMode),
                    context.features.metaControl.commentatorNameSize);
                pronounsLabel = labelUtil.createBaseDropShadowLabel(
                    defaultFlagSize.Width + nameLabelSize.Width, 0, pronounLabelSize.Width, pronounLabelSize.Height,
                    pronounText, ContentAlignment.MiddleLeft,
                    getActiveColor(colorMode), getShadowColor(colorMode),
                    context.features.metaControl.commentatorPronounSize);
            }
            else
            {
                nameLabel = labelUtil.createBaseLabel(defaultFlagSize.Width, 0, nameLabelSize.Width, nameLabelSize.Height, userText, ContentAlignment.MiddleLeft, context.features.metaControl.commentatorNameSize);
                pronounsLabel = labelUtil.createBaseLabel(defaultFlagSize.Width + nameLabelSize.Width, 0, pronounLabelSize.Width, pronounLabelSize.Height, pronounText, ContentAlignment.MiddleLeft, context.features.metaControl.commentatorPronounSize);
            }
            Controls.Add(nameLabel);
            Controls.Add(pronounsLabel);
            nameLabel.BringToFront();
            pronounsLabel.BringToFront();
            Size = new Size(nameLabelSize.Width + defaultFlagSize.Width + pronounLabelSize.Width, Math.Max(nameLabelSize.Height, defaultFlagSize.Height));
            if (smallPadding)
            {
                Padding = new Padding(0, 0, 0, 0);
                Margin = new Padding(0, 0, 0, 5);
            }
            setSpeaking(false);
        }

        public void setSpeaking(bool speaking)
        {
            if (!speaking)
            {
                nameLabel.ForeColor = getDisabledColor(colorMode);
                pronounsLabel.ForeColor = getDisabledColor(colorMode);
                nameLabel.Font = new Font(nameLabel.Font, FontStyle.Regular);
                pronounsLabel.Font = new Font(pronounsLabel.Font, FontStyle.Regular);
            }
            else
            {
                nameLabel.ForeColor = getActiveColor(colorMode);
                pronounsLabel.ForeColor = getActiveColor(colorMode);
                nameLabel.Font = new Font(nameLabel.Font, FontStyle.Italic);
                pronounsLabel.Font = new Font(pronounsLabel.Font, FontStyle.Italic);
            }
        }

        public string getUserName()
        {
            return userName;
        }

        public string getDiscordName()
        {
            return discordName;
        }
    }
}
