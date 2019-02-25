using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bubblim.Core.Helper
{
    public static class EmojiNumberConverter
    {
        private static string[] textNumbers = 
            {
                ":zero:",
                ":one:", ":two:", ":three:",
                ":four:", ":five:", ":six:",
                ":seven:", ":eight:", ":nine:"
            };

        public static List<string> ToNumberedEmojiList<T>(this ICollection<T> source, Func<T, string> func)
        {
            var i = 1;
            List<string> list = new List<string>();

            source.Select(func).ToList().ForEach(item => {
                list.Add($"{ToNumberEmoji(i)} {item ?? ""}");
                i++;
            });

            return list;
        }

        public static string ToNumberedEmojiString<T>(this ICollection<T> source, Func<T, string> func, int offset = 0)
        {
            var i = 1 + offset;
            string list = "";

            source.Select(func).ToList().ForEach(item => {
                list += $"\n{ToNumberEmoji(i)} {item ?? ""}";
                i++;
            });

            return list;
        }

        public static string ToNumberEmoji(int i)
        {
            string number = i.ToString();
            char[] numberChar = number.ToCharArray();
            int[] numbers = number.ToCharArray().Select(n => Convert.ToInt32(n.ToString())).ToArray();

            if (i < 0)
                return null;
            if (i < 10)
                return textNumbers[i];
            if (i < 100)
                return textNumbers[numbers[0]] + textNumbers[numbers[1]];

            return null;
        }
    }
}
