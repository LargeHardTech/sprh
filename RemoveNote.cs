using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace sprh
{

    public static class SprhCommentRemover
    {

        public static string RemoveComments(string code)
        {
            if (string.IsNullOrEmpty(code))
                return code;


            string pattern = @"/\*.*?\*/";
            return Regex.Replace(code, pattern, "", RegexOptions.Singleline);
        }

        public static string RemoveComments(string code, bool alsoRemoveWhitespace)
        {
            string result = RemoveComments(code);
            if (alsoRemoveWhitespace)
            {
                result = new string(result.Where(c => !char.IsWhiteSpace(c)).ToArray());
            }
            return result;
        }
    }
}