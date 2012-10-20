using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonJobs.Utilities
{
    public static class StringUtilities
    {
        public static string AppendIfDoesNotEndWith(this string originalString, string ending)
        {
            if (string.IsNullOrEmpty(originalString)) 
                return ending;

            if (originalString.EndsWith(ending))
                return originalString;

            return originalString + ending;
        }

        //http://predicatet.blogspot.com.ar/2009/04/improved-c-slug-generator-or-how-to.html
        public static string GenerateSlug(this string phrase, int maxLength = 45)
        {
            string str = phrase.RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}
