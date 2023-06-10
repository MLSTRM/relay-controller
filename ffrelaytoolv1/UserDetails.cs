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

        public string toInlineString()
        {
            return $"{Flag}{Name} ({Pronouns})";
        }
    }

    public static class UserDetailsUtils
    {
        private static readonly Regex DETAILS_PATTERN = new Regex("([^\\(]*)\\(([A-Za-z]{2})\\|(\\w+/\\w+)\\)");
        // Parses a string of the format "someUserName(ISOCODE|pro/nouns)"
        public static UserDetails parseUserFromDetailsString(string details)
        {
            if (!DETAILS_PATTERN.IsMatch(details))
            {
                return new UserDetails
                {
                    Name = details,
                    Flag = "gb", //IsoCountryCodeToFlagEmoji("UN"),
                    Pronouns = "-/-"
                };
            }
            var groups = DETAILS_PATTERN.Match(details).Groups;
            return new UserDetails
            {
                Name = groups[1].Value,
                Flag = groups[2].Value.ToLower(),
                Pronouns = groups[3].Value
            };
        }

        private static string IsoCountryCodeToFlagEmoji(string country)
        {
            return string.Concat(country.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
        }
    }
}
