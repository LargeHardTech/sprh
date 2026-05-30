using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sprh
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
                result = result.Replace("= ", "=" + SpacePlaceholder);
                result = new string(result.Where(c => !char.IsWhiteSpace(c)).ToArray());
                result = result.Replace(SpacePlaceholder, " ");
            }
            return result;
        }
    }
}