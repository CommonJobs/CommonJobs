using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonJobs.Utilities
{
    public class CleanTextBuilder
    {
        private readonly HashSet<UnicodeCategory> ForbiddenCategories = new HashSet<UnicodeCategory>() {
            UnicodeCategory.Format,
            UnicodeCategory.PrivateUse 
        };

        private readonly HashSet<UnicodeCategory> SpaceCategories = new HashSet<UnicodeCategory>() {
            UnicodeCategory.NonSpacingMark,
            UnicodeCategory.SpaceSeparator,
            UnicodeCategory.SpacingCombiningMark
        };

        private readonly HashSet<UnicodeCategory> LineSeparatorsCategories = new HashSet<UnicodeCategory>() {
            UnicodeCategory.LineSeparator,
            UnicodeCategory.Control
        };

        bool space = true;
        bool line = true;
        bool forbidden = false;
        StringBuilder stringBuilder = new StringBuilder();

        public void Add(char character)
        {
            var category = Char.GetUnicodeCategory(character);
            if (ForbiddenCategories.Contains(category))
            {
                forbidden = true;
            }
            else if (SpaceCategories.Contains(category))
            {
                if (!space && !line)
                {
                    stringBuilder.Append(" ");
                    forbidden = false;
                    space = true;
                }
            }
            else if (LineSeparatorsCategories.Contains(category))
            {
                if (!line)
                {
                    stringBuilder.AppendLine();
                    forbidden = false;
                    line = true;
                }
            }
            else
            {
                if (forbidden && !line && !space)
                {
                    stringBuilder.Append(" ");
                }
                forbidden = false;
                space = false;
                line = false;
                stringBuilder.Append(character);
            }
        }

        public void Add(IEnumerable<char> characters)
        {
            foreach (var character in characters)
                Add(character);
        }

        public void Add(TextReader reader, int bufferSize = 255)
        {
            var buffer = new char[bufferSize];
            int readed;
            while ((readed = reader.Read(buffer, 0, bufferSize)) > 0)
                Add(buffer.Take(readed));
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
