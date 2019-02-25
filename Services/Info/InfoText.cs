using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bubblim.Core.Services
{
    public class InfoText
    {
        public string Summary { get; set; }
        public string Content { get; set; }

        public InfoText()
        {
        }

        public InfoText(FileInfo fileInfo)
        {
            var reader = fileInfo.OpenText();

            if (reader.EndOfStream)
                return;

            var firstLine = reader.ReadLine();

            if (firstLine.StartsWith("//summary:"))
                Summary = firstLine.Replace("//summary:", "");
            else
                Content = firstLine;

            while (!reader.EndOfStream)
            {
                Content += "\n" + EmojiReplacer(reader.ReadLine());
            }

            reader.Close();
        }

        public bool Empty()
        {
            if (Summary == null && Content == null)
                return true;
            return false;
        }

        public override string ToString()
        {
            return Summary;
        }

        public string EmojiReplacer(string input)
        {
            if (input.Contains(":blank:"))
            {
                input = input.Replace(":blank:", "<:blank:455742558035902474>");
            }

            return input;
        }
    }
}
