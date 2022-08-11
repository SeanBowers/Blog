using System.Globalization;
using System.Text.RegularExpressions;

namespace Blog.Extensions
{
    public static class StringExtensions
    {
        public static string Slugify(this string phrase)
        {
            //Remove all accents and make the string lower case
            string output = phrase.RemoveAccents().ToLower();

            //Remove all special characters from the string.
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            //Remove all additional spaces in favour of just one.
            output = Regex.Replace(output, @"\s+", " ").Trim();

            //Replace all spaces with hyphens.
            output = Regex.Replace(output, @"\s", "-");

            //Return the slug.
            return output;
        }
        private static string RemoveAccents(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) { return phrase; }

            //Convert to Unicode
            phrase = phrase.Normalize(System.Text.NormalizationForm.FormD);

            //Format unicode/ascii
            char[] chars = phrase.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            //Convert and return the new phrase
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
