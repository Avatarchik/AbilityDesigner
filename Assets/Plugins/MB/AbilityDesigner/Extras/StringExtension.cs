using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace MB.AbilityDesigner
{
    public static class StringExtension
    {
        public static string Encapsulate(this string value, string tag)
        {
            return "<" + tag + ">" + value + "</" + tag + ">";
        }

        public static string Expose(this string input, string tag)
        {
            Match match = Regex.Match(input, "<" + tag + @">((.|\n)*?)<\/" + tag + ">", RegexOptions.Multiline);
            if (match.Success)
            {
                if (match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    return "";
                }
            }
            return "";
        }
    }
}