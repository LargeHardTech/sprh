using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sprh
{
    internal class Interpreter
    {
        public const int bufsize = 1024;
        public string op { get; set; } = "";
        public bool debug { get; set; } = false;
        public bool timer { get; set; } = false;
        public bool strict { get; set; } = false;
        public int sleeptime { get; set; } = 100;
        public string filename { get; set; } = "";
        public string inputfilename { get; set; } = "input.spri";
        public string outputfilename { get; set; } = "output.spro";


        private static byte variable = 0;
        private static byte[,] buf = new byte[bufsize, bufsize];
        
        private static int x = 0;
        private static int y = 0;
        private static Stack<byte> stack = new Stack<byte>();
        internal static FileStream? inputFileStream = null;

        /// <summary>
        /// 安全关闭并释放输入文件流
        /// </summary>
        internal static void DisposeInputStream()
        {
            if (inputFileStream != null)
            {
                inputFileStream.Close();
                inputFileStream.Dispose();
                inputFileStream = null;
            }
        }

        private static ConsoleAddon ca = new ConsoleAddon();
        private static Random rand = Random.Shared;

        // 预编译正则，避免每次循环重复解析
        private static readonly Regex HexDigitRegex = new Regex(
            "^[0-9A-Fa-f]+$", RegexOptions.Compiled);

        // 辅助方法：判断字符是否为十六进制数字
        private static bool IsHexDigit(char c)
            => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');

        // 辅助方法：将十六进制字符转为整数值
        private static int HexCharToInt(char c) => c switch
        {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'F' => c - 'A' + 10,
            >= 'a' and <= 'f' => c - 'a' + 10,
            _ => throw new ArgumentException($"非十六进制字符: {c}")
        };
        public void error(string a, int idx)
        {
            if (debug)
            {
                ca.Warn("程序发生异常,错误位于第" + idx + "," + (idx + 1) + "个指令:" + op[idx] + op[idx + 1]);
                System.Threading.Thread.Sleep(sleeptime);
            }
            ca.Error(a);
            ca.Warn("停止执行" + filename);
            Console.ResetColor();
            throw new SprhRuntimeException(a, idx);
        }
        private void ShowLogo()
        {
            Console.WriteLine("\r\n██╗      █████╗ ██████╗  ██████╗ ███████╗██╗  ██╗ █████╗ ██████╗ ██████╗\r\n" +
                              "██║     ██╔══██╗██╔══██╗██╔════╝ ██╔════╝██║  ██║██╔══██╗██╔══██╗██╔══██╗ \r\n" +
                              "██║     ███████║██████╔╝██║  ███╗█████╗  ███████║███████║██████╔╝██║  ██║\r\n" +
                              "██║     ██╔══██║██╔══██╗██║   ██║██╔══╝  ██╔══██║██╔══██║██╔══██╗██║  ██║\r\n" +
                              "███████╗██║  ██║██║  ██║╚██████╔╝███████╗██║  ██║██║  ██║██║  ██║██████╔╝\r\n" +
                              "╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ");
            Console.Write(" ________  ________  ________  ___  ___     \r\n" +
                "|\\   ____\\|\\   __  \\|\\   __  \\|\\  \\|\\  \\    Version:"+Program.version+ "\r\n" +
                "\\ \\  \\___|\\ \\  \\|\\  \\ \\  \\|\\  \\ \\  \\\\\\  \\   "+"Author:LargeHardTech"+"\r\n" +
                " \\ \\_____  \\ \\   ____\\ \\   _  _\\ \\   __  \\  "+ "GitHub:https://github.com/LargeHardTech/sprh" + "\r\n" +
                "  \\|____|\\  \\ \\  \\___|\\ \\  \\\\  \\\\ \\  \\ \\  \\ "+""+"\r\n" +
                "    ____\\_\\  \\ \\__\\    \\ \\__\\\\ _\\\\ \\__\\ \\__\\\r\n" +
                "   |\\_________\\|__|     \\|__|\\|__|\\|__|\\|__|\r\n" +
                "   \\|_________|                             \r\n                                            ");
        }
        public void r()
        {
            if (debug)
            {
                ca.Warn("开始执行文件...");
                System.Threading.Thread.Sleep(sleeptime);
            }
            try
            {
                run();
            }
            catch (Exception e)
            {
                if (debug)
                {
                    ca.Warn("程序发生异常!");
                    System.Threading.Thread.Sleep(sleeptime);
                }
                ca.Error(e.ToString());
                ca.Warn("停止执行" + filename);
                return;
            }
            if (debug)
            {
                ca.Warn(filename + "执行完成!");
            }
        }
        private void run()
        {


            for (int i = 0; i < op.Length; i += 2)
            {
                if (debug) { ca.log("执行代码:" + op[i] + op[i + 1]); Thread.Sleep(sleeptime); }
                //==========================================================
                if (char.ToLower(op[i]) == 'u')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        if (debug) ca.log("参数:" + op[i + 1]);
                        y -= HexCharToInt(op[i + 1]);
                        if (y < 0)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("上移距离超过缓冲区边界,自动移动到缓冲区边界"); y = 0; }
                        }
                    }
                    else if (char.ToLower(op[i + 1]) == 'v')
                    {
                        if (debug) ca.log("变量现在数值&向上移动距离:" + variable);
                        y -= variable;
                        if (y < 0)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("上移距离超过缓冲区边界,自动移动到缓冲区边界"); y = 0; }
                        }
                    }
                    else if (op[i + 1] == '?')
                    {
                        if (y == 0)
                        {
                            if (strict) error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else if (debug) ca.Warn("上移距离超过缓冲区边界,已取消操作");
                        }
                        else
                        {
                            var r = rand.Next(1, y + 1);
                            if (debug) ca.log("随机到的数值&向上移动距离:" + r);
                            y -= r;
                            if (y < 0)
                            {
                                if (strict)
                                    error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                                else { ca.Warn("上移距离超过缓冲区边界,自动移动到缓冲区边界"); y = 0; }
                            }
                        }
                    }
                    else error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数" + op[i + 1] + "不是一个十六进制整型或其他可能的参数", i);
                }
                else if (char.ToLower(op[i]) == 'd')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        if (debug) ca.log("参数:" + op[i + 1]);
                        y += HexCharToInt(op[i + 1]);
                        if (y >= bufsize)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("下移距离超过缓冲区边界,自动移动到缓冲区边界"); y = bufsize - 1; }
                        }
                    }
                    else if (char.ToLower(op[i + 1]) == 'v')
                    {
                        if (debug) ca.log("变量现在数值&向下移动距离:" + variable);
                        y += variable;
                        if (y >= bufsize)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("下移距离超过缓冲区边界,自动移动到缓冲区边界"); y = bufsize - 1; }
                        }
                    }
                    else if (op[i + 1] == '?')
                    {
                        int maxDown = bufsize - 1 - y;
                        if (maxDown <= 0)
                        {
                            if (strict) error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else if (debug) ca.Warn("下移距离超过缓冲区边界,已取消操作");
                        }
                        else
                        {
                            var r = rand.Next(1, maxDown + 1);
                            if (debug) ca.log("随机到的数值&向下移动距离:" + r);
                            y += r;
                            if (y >= bufsize)
                            {
                                if (strict)
                                    error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                                else { y = bufsize - 1; }
                            }
                        }
                    }
                    else error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数" + op[i + 1] + "不是一个十六进制整型或其他可能的参数", i);
                }
                else if (char.ToLower(op[i]) == 'l')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        if (debug) ca.log("参数:" + op[i + 1]);
                        x -= HexCharToInt(op[i + 1]);
                        if (x < 0)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("左移距离超过缓冲区边界,自动移动到缓冲区边界"); x = 0; }
                        }
                    }
                    else if (char.ToLower(op[i + 1]) == 'v')
                    {
                        if (debug) ca.log("变量现在数值&向左移动距离:" + variable);
                        x -= variable;
                        if (x < 0)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("左移距离超过缓冲区边界,自动移动到缓冲区边界"); x = 0; }
                        }
                    }
                    else if (op[i + 1] == '?')
                    {
                        if (x == 0)
                        {
                            if (strict) error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else if (debug) ca.Warn("左移距离超过缓冲区边界,已取消操作");
                        }
                        else
                        {
                            var r = rand.Next(1, x + 1);
                            if (debug) ca.log("随机到的数值&向左移动距离:" + r);
                            x -= r;
                            if (x < 0)
                            {
                                if (strict)
                                    error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                                else { ca.Warn("左移距离超过缓冲区边界,自动移动到缓冲区边界"); x = 0; }
                            }
                        }
                    }
                    else error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数" + op[i + 1] + "不是一个十六进制整型或其他可能的参数", i);
                }
                else if (char.ToLower(op[i]) == 'r')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        if (debug) ca.log("参数:" + op[i + 1]);
                        x += HexCharToInt(op[i + 1]);
                        if (x >= bufsize)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("右移距离超过缓冲区边界,自动移动到缓冲区边界"); x = bufsize - 1; }
                        }
                    }
                    else if (char.ToLower(op[i + 1]) == 'v')
                    {
                        if (debug) ca.log("变量现在数值&向右移动距离:" + variable);
                        x += variable;
                        if (x >= bufsize)
                        {
                            if (strict)
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else { ca.Warn("右移距离超过缓冲区边界,自动移动到缓冲区边界"); x = bufsize - 1; }
                        }
                    }
                    else if (op[i + 1] == '?')
                    {
                        int maxRight = bufsize - 1 - x;
                        if (maxRight <= 0)
                        {
                            if (strict) error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                            else if (debug) ca.Warn("右移距离超过缓冲区边界,已取消操作");
                        }
                        else
                        {
                            var r = rand.Next(1, maxRight + 1);
                            if (debug) ca.log("随机到的数值&向右移动距离:" + r);
                            x += r;
                            if (x >= bufsize)
                            {
                                if (strict)
                                    error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                                else { x = bufsize - 1; }
                            }
                        }
                    }
                    else error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数" + op[i + 1] + "不是一个十六进制整型或其他可能的参数", i);
                }
                else if (op[i] == '+')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        byte a = (byte)HexCharToInt(op[i + 1]);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((buf[x, y] + a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] += (byte)HexCharToInt(op[i + 1]);
                    }
                    else
                    {
                        if (op[i + 1] == '+')
                        {
                            if (debug)
                            {
                                ca.log("将当前位置的值设为255");
                            }
                            buf[x, y] = 255;
                        }
                        else if (op[i + 1] == '?')
                        {
                            int maxAdd = 255 - buf[x, y];
                            if (maxAdd > 1)
                            {
                                int r = rand.Next(1, maxAdd);
                                if (debug) ca.log("当前格的值:" + buf[x, y]);
                                if (debug) ca.log("随机到的值:" + r);
                                buf[x, y] += (byte)r;
                            }
                            else
                            {
                                if (debug) ca.Warn("当前单元格值已满，无法随机增加");
                            }
                        }
                        else
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                                + op[i + 1]
                                + "不是一个十六进制整型", i);
                        }

                    }
                }
                else if (op[i] == '-')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        byte a = (byte)HexCharToInt(op[i + 1]);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((buf[x, y] - a) < 0 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] -= (byte)HexCharToInt(op[i + 1]);
                    }
                    else
                    {
                        if (op[i + 1] == '-')
                        {
                            if (debug)
                            {
                                ca.log("将当前位置的值归零");
                            }
                            buf[x, y] = 0;
                        }
                        else if (op[i + 1] == '?')
                        {
                            int maxRedu = buf[x, y];
                            if (maxRedu >= 1)
                            {
                                int r = rand.Next(1, maxRedu + 1);
                                if (debug) ca.log("当前格的值:" + buf[x, y]);
                                if (debug) ca.log("随机到的值:" + r);
                                buf[x, y] -= (byte)r;
                            }
                            else
                            {
                                if (debug) ca.Warn("当前单元格值已空，无法随机减少");
                            }
                        }
                        else
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                                + op[i + 1]
                                + "不是一个十六进制整型", i);
                        }

                    }
                }
                else if (op[i].ToString().ToLower() == "p")
                {
                    if (debug)
                    {
                        ca.log("参数:" + op[i + 1]);
                        ca.Warn("结果:");
                    }
                    if (op[i + 1] == 'c' || op[i + 1] == 'C')
                    {
                        Console.Write((char)buf[x, y]);
                    }
                    else if (op[i + 1] == 'i' || op[i + 1] == 'I')
                    {
                        Console.Write((int)buf[x, y]);
                    }else if (char.ToLower(op[i+1]) == 'l')
                    {
                        ShowLogo();
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                    }
                    if (debug)
                    {
                        Console.WriteLine();
                    }
                }
                else if (op[i].ToString().ToLower() == "v")
                {
                    if (op[i + 1] == '=')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:" + variable + ",");
                        }
                        variable = buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }

                    }
                    else if (op[i + 1].ToString().ToLower() == "w")
                    {
                        buf[x, y] = variable;
                        if (debug)
                        {
                            ca.log("已将变量数值写入缓冲区" + variable + ",");
                        }
                    }
                    else if (op[i + 1] == '+')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:" + variable + ",");
                        }
                        if ((buf[x, y] + variable) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        variable += buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }
                    }
                    else if (op[i + 1] == '-')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:" + variable + ",");
                        }
                        if ((variable - buf[x, y]) < 0 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        variable -= buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }
                    }
                    else if (op[i + 1] == '*')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:" + variable + ",");
                        }
                        if ((buf[x, y] * variable) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        variable *= buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }
                    }
                    else if (op[i + 1] == '/')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:" + variable + ",");
                        }
                        if ((buf[x, y]) == 0)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不可以被用作除数", i);
                        }
                        variable /= buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }
                    }
                    else if (op[i + 1] == '?')
                    {
                        if (debug)
                            ca.log("变量原数值:" + variable);
                        variable = (byte)rand.Next(0, 256);
                        if (debug)
                            ca.log("变量现数值:" + variable);
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                    }
                }
                else if (op[i] == '=')
                {
                    if (debug)
                    {
                        ca.log("参数:" + op[i + 1]);
                    }
                    if (op[i + 1] > 255 && strict)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                    }
                    buf[x, y] = (byte)op[i + 1];


                }
                else if (op[i] == '*')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        byte a = (byte)HexCharToInt(op[i + 1]);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((buf[x, y] * a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] *= (byte)HexCharToInt(op[i + 1]);
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if (op[i] == '/')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        byte a = (byte)HexCharToInt(op[i + 1]);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((a) == 0)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不能作为除数", i);
                        }
                        buf[x, y] /= (byte)HexCharToInt(op[i + 1]);
                    }
                    else
                    {
                        if (op[i + 1] == ']')
                        {
                            if (debug)
                            {
                                ca.log("该条指令为[跳转指令的标记");
                            }

                        }
                        else
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                                + op[i + 1]
                                + "不是一个十六进制整型", i);
                        }
                    }
                }
                else if (op[i] == '[')
                {
                    char param = char.ToLower(op[i + 1]);
                    int targetX = x, targetY = y;
                    switch (param)
                    {
                        case 'u': targetY = y - 1; break;
                        case 'd': targetY = y + 1; break;
                        case 'l': targetX = x - 1; break;
                        case 'r': targetX = x + 1; break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                    // 先检查边界，再访问数组，避免 IndexOutOfRangeException
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }
                    bool cond = buf[x, y] == buf[targetX, targetY];
                    if (debug)
                    {
                        ca.log("当前位置数值:" + buf[x, y] + ",目标数值:" + buf[targetX, targetY]);
                    }
                    if (cond)
                    {
                        i = search(i,'[', ']');
                    }
                }
                else if (op[i] == '{')
                {
                    char param = char.ToLower(op[i + 1]);
                    int targetX = x, targetY = y;
                    switch (param)
                    {
                        case 'u': targetY = y - 1; break;
                        case 'd': targetY = y + 1; break;
                        case 'l': targetX = x - 1; break;
                        case 'r': targetX = x + 1; break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                    // 先检查边界，再访问数组
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }
                    bool cond = buf[x, y] > buf[targetX, targetY];
                    if (debug)
                    {
                        ca.log("当前位置数值:" + buf[x, y] + ",目标数值:" + buf[targetX, targetY]);
                    }
                    if (cond)
                    {
                        i = search(i,'{', '}');
                    }
                }
                else if (op[i] == '(')
                {
                    char param = char.ToLower(op[i + 1]);
                    int targetX = x, targetY = y;
                    switch (param)
                    {
                        case 'u': targetY = y - 1; break;
                        case 'd': targetY = y + 1; break;
                        case 'l': targetX = x - 1; break;
                        case 'r': targetX = x + 1; break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                    // 先检查边界，再访问数组
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }
                    bool cond = buf[x, y] < buf[targetX, targetY];
                    if (debug)
                    {
                        ca.log("当前位置数值:" + buf[x, y] + ",目标数值:" + buf[targetX, targetY]);
                    }
                    if (cond)
                    {
                        i = search(i,'(', ')');
                    }
                }
                else if (op[i] == '<')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        byte a = (byte)HexCharToInt(op[i + 1]);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if (i - 2 * a < 0)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:跳转时计数器溢出", i);
                        }
                        i -= a * 2 + 2;
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if (op[i] == '>')
                {
                    if (IsHexDigit(op[i + 1]))
                    {
                        byte a = (byte)HexCharToInt(op[i + 1]);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if (i + 2 * a >= op.Length)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:跳转时计数器溢出", i);
                        }
                        i += a * 2 - 2;  // -2 抵消循环的 i+=2，使跳转与 < 指令对称
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if ("&|^~,.".IndexOf(char.ToUpper(op[i])) >= 0)
                {
                    char dir = char.ToLower(op[i + 1]);
                    if (dir != 'u' && dir != 'd' && dir != 'l' && dir != 'r')
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:方向参数必须为u/d/l/r", i);
                    }

                    // 计算相邻坐标
                    int targetX = x, targetY = y;
                    switch (dir)
                    {
                        case 'u': targetY = y - 1; break;
                        case 'd': targetY = y + 1; break;
                        case 'l': targetX = x - 1; break;
                        case 'r': targetX = x + 1; break;
                    }
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }

                    byte other = buf[targetX, targetY];
                    if (debug)
                    {
                        ca.log("相邻单元格[" + targetX + "," + targetY + "]的值:" + other);
                    }

                    char cmdUpper = char.ToUpper(op[i]);
                    switch (cmdUpper)
                    {
                        case '&':
                            buf[x, y] &= other;
                            if (debug) ca.log("按位与结果:" + buf[x, y]);
                            break;
                        case '|':
                            buf[x, y] |= other;
                            if (debug) ca.log("按位或结果:" + buf[x, y]);
                            break;
                        case '^':
                            buf[x, y] ^= other;
                            if (debug) ca.log("按位异或结果:" + buf[x, y]);
                            break;
                        case '~':
                            // 对相邻单元格的值取反后赋给当前单元格
                            buf[x, y] = (byte)(~other);
                            if (debug) ca.log("按位取反结果:" + buf[x, y]);
                            break;
                        case ',': // 左移
                            {
                                int result = buf[x, y] << other;
                                if (strict && result > 255)
                                    error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:左移结果超出255", i);
                                buf[x, y] = (byte)(result & 0xFF);
                                if (debug) ca.log("左移结果:" + buf[x, y]);
                                break;
                            }
                        case '.': // 右移
                            buf[x, y] >>= other;
                            if (debug) ca.log("右移结果:" + buf[x, y]);
                            break;
                    }
                }
                else if (char.ToLower(op[i]) == 's')
                {
                    if (debug)
                    {
                        if (stack.Count != 0)
                        {
                            ca.log("当前栈顶元素:" + stack.Peek().ToString());
                        }
                        else
                        {
                            ca.log("当前栈为空");
                        }

                    }
                    switch (char.ToLower(op[i + 1]))
                    {
                        case '+':
                            stack.Push(buf[x, y]);
                            if (debug)
                                ca.log("已将" + buf[x, y] + "压入栈顶");
                            break;
                        case '-':
                            if (stack.Count == 0)
                            {
                                error("栈为空,不支持出栈", i);
                            }
                            buf[x, y] = stack.Pop();
                            if (debug)
                                ca.log("已将" + buf[x, y] + "弹出并写入缓冲区");
                            break;
                        case 'c':
                            stack.Clear();
                            if (debug)
                                ca.log("已将栈清空");
                            break;
                        case '=':
                            if (stack.Count == 0)
                            {
                                error("栈为空,不支持交换", i);
                            }
                            byte a = stack.Pop();
                            stack.Push(buf[x, y]);
                            buf[x, y] = a;
                            if (debug)
                                ca.log("已将栈顶元素与缓冲区数值交换");
                            break;
                        case 's':
                            buf[x, y] = (byte)stack.Count;
                            if (debug)
                                ca.log("已写入缓冲区,当前栈中项数:" + buf[x, y]);
                            break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                }
                else if (char.ToLower(op[i]) == 'i')
                {
                    if (debug)
                    {
                        ca.log("参数:" + op[i + 1]);
                        ca.Warn("请输入:");
                    }
                    if (op[i + 1] == '+')
                    {
                        byte a = (byte)Console.ReadKey().KeyChar;
                        if ((buf[x, y] + a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] += a;
                    }
                    else if (op[i + 1] == '-')
                    {
                        byte a = (byte)Console.ReadKey().KeyChar;
                        if ((buf[x, y] - a) < 0 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] -= a;
                    }
                    else if (op[i + 1] == '*')
                    {
                        byte a = (byte)Console.ReadKey().KeyChar;
                        if ((buf[x, y] * a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] *= a;
                    }
                    else if (op[i + 1] == '/')
                    {
                        byte a = (byte)Console.ReadKey().KeyChar;
                        if (a == 0)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不能作为除数", i);
                        }
                        buf[x, y] /= a;
                    }
                    else if (op[i + 1] == 'w' || op[i + 1] == 'W' || op[i + 1] == '=')
                    {
                        buf[x, y] = (byte)Console.ReadKey().KeyChar;
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                    }
                    if (debug)
                    {
                        Console.WriteLine("已写入[" + x.ToString() + ',' + y.ToString() + "]");
                    }
                }
                else if (char.ToLower(op[i]) == 'f')
                {
                    char subCmd = op[i + 1];
                    if (debug)
                    {
                        ca.log("参数:" + subCmd);
                        ca.Warn("执行文件操作...");
                    }
                    if(subCmd == '<')
                    {
                        inputfilename += buf[x, y];
                        if (debug)
                        {
                            ca.log("将" + buf[x,y]+"添加到输入文件名");
                            ca.Warn("当前输入文件名:"+inputfilename);
                        }
                    }
                    else if(subCmd == '>')
                    {
                        outputfilename += buf[x, y];
                        if (debug)
                        {
                            ca.log("将" + buf[x, y] + "添加到输出文件名");
                            ca.Warn("当前输出文件名:" + outputfilename);
                        }
                    }
                    else if(subCmd == '[')
                    {
                        inputfilename = "";
                        if (debug)
                        {
                            ca.log("将输入文件名清除");
                        }
                    }
                    else if(subCmd == ']')
                    {
                        outputfilename = "";
                        if (debug)
                        {
                            ca.log("将输出文件名清除");
                        }
                    }
                    else if(subCmd == '+' || subCmd == '-' || subCmd == '*' || subCmd == '/' || subCmd == '=')
                    {
                        if (string.IsNullOrEmpty(inputfilename))
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未指定输入文件名", i);
                        }
                        if (!File.Exists(inputfilename))
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:文件不存在", i);
                        }
                        try
                        {
                            if (inputFileStream == null)
                            {
                                inputFileStream = new FileStream(inputfilename, FileMode.Open, FileAccess.Read);
                            }
                            if (Path.GetFullPath(inputFileStream.Name) != Path.GetFullPath(inputfilename))
                            {
                                inputFileStream.Close();
                                inputFileStream.Dispose();
                                inputFileStream = new FileStream(inputfilename, FileMode.Open, FileAccess.Read);
                            }
                            int b = inputFileStream.ReadByte();
                            if (b == -1)
                            {
                                error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:文件已读完", i);
                            }
                            byte val = (byte)b;
                            if (debug)
                            {
                                ca.log("从文件读取的字节:" + val);
                            }
                            switch (subCmd)
                            {
                                case '+':
                                    if ((buf[x, y] + val) > 255 && strict)
                                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                                    buf[x, y] += val;
                                    break;
                                case '-':
                                    if ((buf[x, y] - val) < 0 && strict)
                                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                                    buf[x, y] -= val;
                                    break;
                                case '*':
                                    if ((buf[x, y] * val) > 255 && strict)
                                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                                    buf[x, y] *= val;
                                    break;
                                case '/':
                                    if (val == 0)
                                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不能作为除数", i);
                                    buf[x, y] /= val;
                                    break;
                                case '=':
                                    buf[x, y] = val;
                                    break;
                            }
                            if (debug)
                            {
                                ca.log("操作后当前单元格值:" + buf[x, y]);
                                ca.log("当前文件流位置:"+inputFileStream.Position.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:文件操作异常 - " + ex.Message, i);
                        }
                    }
                    else if (subCmd == 'c' || subCmd == 'C' || subCmd == 'i' || subCmd == 'I')
                    {
                        if (string.IsNullOrEmpty(outputfilename))
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未指定输出文件名", i);
                        }
                        //if (!File.Exists(outputfilename))
                        //{
                        //   error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:文件不存在", i);
                        //}
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(outputfilename, true, Encoding.Default))
                            {
                                if (subCmd == 'c' || subCmd == 'C')
                                {
                                    sw.Write((char)buf[x, y]);
                                }
                                else // i 或 I
                                {
                                    sw.Write(buf[x, y].ToString());
                                }
                            }
                            if (debug)
                            {
                                ca.log("已将值写入输出文件");
                            }
                        }
                        catch (Exception ex)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:文件操作异常 - " + ex.Message, i);
                        }
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + subCmd + '\"', i);
                    }
                }
                else
                {
                    error("未知的指令:" + op[i] + op[i + 1], i);
                }

                if (debug)
                {
                    ca.log("当前状态: x=" + x + ",y=" + y + ",num=" + buf[x, y]);
                    ca.log("当前运行指令:第" + i.ToString() + "," + (i + 1).ToString() + "个字符");
                    ca.log("下次运行指令:第" + (i + 2).ToString() + "," + (i + 3).ToString() + "个字符\n==============================");
                }

            }

        }
        
        private int search(int i, char o, char c)
        {
            // depth: 当前嵌套深度（已遇到一个开括号，初始为1）
            int depth = 1;
            // 从开括号的下一条指令开始搜索（i是开括号指令的字符索引，每条指令2字符）
            for (int j = i + 2; j < op.Length - 1; j += 2)
            {
                // 遇到同类型的开括号（命令符匹配且参数为方向字符），嵌套深度+1
                if (op[j] == o && (op[j + 1] == 'u' || op[j + 1] == 'd' ||
                                   op[j + 1] == 'l' || op[j + 1] == 'r'))
                {
                    depth++;
                }
                // 遇到对应的闭括号，嵌套深度-1
                else if (op[j] == '/' && op[j + 1] == c)
                {
                    depth--;
                    if (debug)
                    {
                        int targetInstr = j / 2;
                        if (targetInstr * 2 + 2 < op.Length)
                            ca.log("索引到指令" + targetInstr + ":/" + c +
                                   ",目前嵌套层数:" + depth);
                        else
                            ca.log("索引到代码末尾,嵌套层数:" + depth);
                    }
                    if (depth == 0)
                    {
                        // 返回闭括号指令的字符索引
                        // 循环中的 i+=2 会使下次迭代从此指令的下一条指令开始
                        return j;
                    }
                }
            }
            error("未能索引到后面的/" + c, i);
            throw new IndexOutOfRangeException();
        }
    }
}
