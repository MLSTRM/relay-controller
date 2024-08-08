using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffrelaytoolv1
{
    public struct UserDetails
    {
        public string Name;
        public string Flag;
        public string Pronouns;
        public string DiscordName;

        public string toInlineString()
        {
            return $"{Flag}{Name} ({Pronouns})";
        }
    }

    public static class UserDetailsUtils
    {
        private static readonly Regex DETAILS_PATTERN = new Regex("(?:Runner: )?([^\\(]*)\\(([A-Za-z\\-]{2,})?\\|(\\w+/\\w+)?\\|?(.*)?\\)");
        // Parses a string of the format "someUserName(ISOCODE|pro/nouns)"
        public static UserDetails parseUserFromDetailsString(string details)
        {
            if (!DETAILS_PATTERN.IsMatch(details))
            {
                return new UserDetails
                {
                    Name = details,
                    Flag = null, //IsoCountryCodeToFlagEmoji("UN"),
                    Pronouns = null,
                    DiscordName = null
                };
            }
            var groups = DETAILS_PATTERN.Match(details).Groups;
            return new UserDetails
            {
                Name = groups[1].Value,
                Flag = groups[2].Value?.ToLower(),
                Pronouns = groups[3].Value,
                DiscordName = groups[4].Value
            };
        }
    }
}
