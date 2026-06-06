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
                var sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    char c = result[i];
                    if (c == ' ')
                    {
                        // 只有当空格在奇数位（参数位置），且前一位是 '='（赋值指令）时才保留
                        if (i % 2 == 1 && i > 0 && result[i - 1] == '=')
                        {
                            sb.Append(c);
                        }
                        // 否则这个空格（包括命令行、分隔用的空格）都被忽略
                    }
                    else if (!char.IsWhiteSpace(c))
                    {
                        // 非空白字符直接保留，同时丢弃换行、制表等其他空白字符
                        sb.Append(c);
                    }
                    // 其他空白字符一律不保留
                }
                result = sb.ToString();
            }
            return result;
        }
    }
}