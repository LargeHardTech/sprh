using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sprh.src.Libs
{
    public static class SprhCommentRemover
    {
        private static readonly Regex CommentRegex = new Regex(
            @"/\*.*?\*/", RegexOptions.Singleline | RegexOptions.Compiled);

        private const string SpacePlaceholder = "\u0001";

        public static string RemoveComments(string code)
        {
            if (string.IsNullOrEmpty(code))
                return code;

            return CommentRegex.Replace(code, "");
        }

        public static string RemoveComments(string code, bool alsoRemoveWhitespace)
        {
            string result = RemoveComments(code);
            if (alsoRemoveWhitespace)
            {
                char[] chars = result.ToCharArray();
                for (int i = 0; i < chars.Length - 1; i++)
                {
                    if (i % 2 == 0 && chars[i] == '=' && chars[i + 1] == ' ')
                    {
                        chars[i + 1] = SpacePlaceholder[0];
                    }
                }
                string temp = new string(chars);
                temp = new string(temp.Where(c => !char.IsWhiteSpace(c) || c == SpacePlaceholder[0]).ToArray());
                result = temp.Replace(SpacePlaceholder, " ");
            }
            return result;
        }
    }
}