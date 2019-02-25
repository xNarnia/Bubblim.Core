using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bubblim.Core.Helper
{
    public static class StringUtilities
    {
        private static Regex numberRegex = new Regex(@"[^\d]");
        public static string GetNumbersFromString(string input)
        {
            return numberRegex.Replace(input, "");
        }
    }
}
