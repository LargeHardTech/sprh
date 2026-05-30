using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sprh
{

    public static class SprhCommentRemover
    {

        // 预编译正则，避免每次调用重新编译
        private static readonly Regex CommentRegex = new Regex(
            @"/\*.*?\*/", RegexOptions.Singleline | RegexOptions.Compiled);

        // 用于标记"= "中空格占位符的唯一标识
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
                // 保护"= "指令中的空格参数：将"= "替换为"=<占位符>"，去除空白后再还原
                result = result.Replace("= ", "=" + SpacePlaceholder);
                result = new string(result.Where(c => !char.IsWhiteSpace(c)).ToArray());
                result = result.Replace(SpacePlaceholder, " ");
            }
            return result;
        }
    }
}