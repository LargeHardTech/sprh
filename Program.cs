using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace sprh
{
    internal class Program
    {
        public const int bufsize = 1024;
        public static string version = "2.0";
        private static string filename = "";
        private static string filenametoconvert = "";
        private static string inputfilename = "";
        private static string outputfilename = "";
        private static bool debug = false;
        private static bool timer = false;
        private static bool strict = false;
        private static bool toCpp = false;
        private static bool filenameflag = false;
        private static bool filenametoconvertflag = false;
        private static bool inputfilenameflag = false;
        private static bool outputfilenameflag = false;
        private static bool argsError = false;
        private static string op = "";
        private static byte variable = 0;
        public static readonly int sleeptime = 100;
        private static byte[,] buf = new byte[bufsize,bufsize];
        private static int x = 0;
        private static int y = 0;
        private static Stack<byte> stack = new Stack<byte>();
        private static FileStream? inputFileStream = null;
        private static ConsoleAddon ca = new ConsoleAddon();
        static void Main(string[] args)
        {
            Console.Title = "LargeHard SPRH Interpreter";
            Console.CancelKeyPress += exit;
            
            
            if (args.Length == 0)
            {
                Help.introduce();
                //Console.ReadKey();
                return;
            }
            else {
                for (int i = 0; i < args.Length; i++)
                {
                    //Console.WriteLine(args[i].Substring(args[i].Length - 5, 5));
                    if (args[i].ToLower().EndsWith(".sprh"))
                    {
                        if (!filenameflag)
                        {
                            filename = args[i];
                            filenameflag = true;
                        }
                        else
                        {
                            ca.Error("提供了多个被读取的文件,sprh解释器无法确定准确位置");
                            argsError = true;
                        }
                        
                    }
                    else if (args[i].ToLower().EndsWith(".cpp"))
                    {
                        if (!filenametoconvertflag)
                        {
                            filenametoconvert = args[i];
                            filenametoconvertflag = true;
                        }
                        else
                        {
                            ca.Error("提供了多个编译后的文件名,sprh解释器无法确定准确位置");
                            argsError = true;
                        }
                    }
                    else if (args[i].ToLower().EndsWith(".spri"))
                    {
                        if (!inputfilenameflag)
                        {
                            inputfilename = args[i];
                            inputfilenameflag = true;
                        }
                        else
                        {
                            ca.Error("提供了多个需要读取的文件名,sprh解释器无法确定准确位置");
                            argsError = true;
                        }
                    }
                    else if (args[i].ToLower().EndsWith(".spro"))
                    {
                        if (!outputfilenameflag)
                        {
                            outputfilename = args[i];
                            outputfilenameflag = true;
                        }
                        else
                        {
                            ca.Error("提供了多个需要写入的文件名,sprh解释器无法确定准确位置");
                            argsError = true;
                        }
                    }
                    else if (args[i].ToLower() == "-debug"|| args[i].ToLower() == "-d")
                    {
                        debug = true;
                    }
                    else if (args[i].ToLower() == "-timer" || args[i].ToLower() == "-t")
                    {
                        timer = true;
                    }
                    else if (args[i].ToLower() == "-strict"|| args[i].ToLower() == "-s")
                    {
                        strict = true;
                    }
                    else if (args[i].ToLower() == "-c" || args[i].ToLower() == "-tocpp")
                    {
                        toCpp = true;
                    }
                    else if (args[i].ToLower() == "/?")
                    {
                        Help.help();
                        return;
                    }
                    else
                    {
                        ca.Error("无效的程序参数:" + args[i]);
                        return;
                    }
                }
            }
            if (filename == "")
            {
                ca.Error("请输入源文件位置");
                Environment.Exit(0);
            }
            
            //-----------------------------
            if (toCpp)
            {
                if (filenametoconvert == "")
                {
                    ca.Error("未提供转换后.cpp的位置");
                    argsError = true;
                }
            }
            else if (filenametoconvert != "")
            {
                ca.Warn("提供了转换后.cpp的位置,但没有要求转换为.cpp");
            }
            bool i1=false,o1 = false;
            if (!inputfilenameflag)
            {
                
                {
                    inputfilename = GetDirectoryPath(filename) + "input.spri";
                }
                i1 = true;
            }
            if (!outputfilenameflag)
            {
                 
                {
                    outputfilename = GetDirectoryPath(filename) + "output.spro";
                }
                o1 = true;
            }
            /*try
            {
                inputFileStream = new FileStream(inputfilename, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                ca.Error("文件操作异常 : " + e.Message);
                argsError = true;
            }*/

            if (argsError)
            {
                Environment.Exit(0);
            }
            
            
            if (debug)
            {
                ca.log("调试模式已开启");
                System.Threading.Thread.Sleep(sleeptime);
                ca.log("开始检查文件");
                System.Threading.Thread.Sleep(sleeptime);
                if (i1)
                {
                    ca.log("自动补全输入文件到:" + inputfilename);
                    System.Threading.Thread.Sleep(sleeptime);
                }
                if (o1)
                {
                    ca.log("自动补全输入文件到:" + inputfilename);
                    System.Threading.Thread.Sleep(sleeptime);
                }

            }
            if (!File.Exists(filename))
            {
                ca.Error("无法找到文件:"+filename);
                return;
            }
            else
            {
                if (debug)
                {
                    ca.log("已找到文件:\n"+filename);
                    System.Threading.Thread.Sleep(sleeptime);
                }
            }
            if (debug)
            {
                ca.log("正在读取文件:\n" + filename);
                System.Threading.Thread.Sleep(sleeptime);
            }
            op = File.ReadAllText(filename);
            if (debug)
            {
                ca.log("文件读取成功!");
                System.Threading.Thread.Sleep(sleeptime);
                ca.log("文件数据:\n" + op);
                System.Threading.Thread.Sleep(sleeptime);
                ca.log("正在格式化文件...");
                System.Threading.Thread.Sleep(sleeptime);
            }
            op = SprhCommentRemover.RemoveComments(op,true);
            if (debug)
            {
                ca.log("文件格式化完成!");
                System.Threading.Thread.Sleep(sleeptime);
                ca.log("文件数据:\n" + op);
                System.Threading.Thread.Sleep(sleeptime);
                ca.log("正在校验文件执行情况...");
                System.Threading.Thread.Sleep(sleeptime);
            }
            if(op.Length%2 == 0)
            {
                if (debug)
                {
                    ca.log("文件无误!");
                    System.Threading.Thread.Sleep(sleeptime);
                }
            }
            else
            {
                ca.Error("文件数据不匹配!");
                return;
            }
            if (timer)
            {
                start = DateTime.Now;
                ca.Warn("开始计时!");
            }
            if (toCpp)
            {
                c();
            }
            else
            {
                r();
            }
            if (timer)
            {
                end = DateTime.Now;
                Console.Write('\n');
                ca.Warn("停止计时!");
                ca.Warn("时间(毫秒):"+(double)((end - start).TotalMilliseconds));//','+(double)start.Millisecond+','+end.Millisecond
            }
            if (inputFileStream != null)
                inputFileStream.Close();
            return ;
        }
        private static void c()
        {
            SprhToCppCompiler converter = new SprhToCppCompiler();
            var options = new SprhToCppCompiler.Options
            {
                Strict = strict,   // 启用严格模式（检查溢出）
                Debug = debug,
                SleepTimeMs = sleeptime,
                InputFile = inputfilename,
                OutputFile = outputfilename,
                BufferSize = bufsize
                // 不生成调试输出（设为 true 则开启）
            };
            try
            {
                ca.Warn("开始编译...");
                // 执行转换
                converter.Compile(filename, filenametoconvert, options);
                ca.Warn("编译成功！");
            }
            catch (Exception ex)
            {
                ca.Error("转换失败：" + ex.Message);
            }
        }
        private static void r()
        {
            if (debug)
            {
                ca.Warn("开始执行文件...");
                System.Threading.Thread.Sleep(sleeptime);
            }
            try
            {
                run(op);
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
        private static DateTime start;
        private static DateTime end;
        private static void run(string op)
        {
            
            
            for (int i = 0; i < op.Length; i+=2) {
                if (debug) { ca.log("执行代码:" + op[i] + op[i+1]);Thread.Sleep(sleeptime); }
                //==========================================================
                if (op[i].ToString().ToLower() == "u")
                {
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        y -= Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (y < 0)
                        {

                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出",i);
                        }
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if (op[i].ToString().ToLower() == "d")
                {
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        y += Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (y >= bufsize)
                        {

                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                        }
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if (op[i].ToString().ToLower() == "l")
                {
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        x -= Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (x < 0)
                        {

                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                        }
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if (op[i].ToString().ToLower() == "r")
                {
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        x += Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (x >= bufsize)
                        {

                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                        }
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:参数"
                            + op[i + 1]
                            + "不是一个十六进制整型", i);
                    }
                }
                else if (op[i] == '+')
                {
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        byte a = (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((buf[x, y] + a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] += (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
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
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        byte a = (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((buf[x, y] - a) < 0 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] -= (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
                    }
                    else
                    {
                        if (op[i+1] == '-')
                        {
                            if (debug)
                            {
                                ca.log("将当前位置的值归零");
                            }
                            buf[x, y] = 0;
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
                    if(op[i+1] == '=')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:"+variable+",");
                        }
                        variable = buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }

                    }
                    else if (op[i+1].ToString().ToLower() == "w")
                    {
                        buf[x,y] = variable;
                        if (debug)
                        {
                            ca.log("已将变量数值写入缓冲区" + variable + ",");
                        }
                    }
                    else if (op[i+1] == '+')
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
                    else if (op[i+1] == '-')
                    {
                        if (debug)
                        {
                            ca.log("变量原数值:" + variable + ",");
                        }
                        if ((variable-buf[x, y] ) < 0 && strict)
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
                        if ((variable) == 0)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不可以被用作除数", i);
                        }
                        variable /= buf[x, y];
                        if (debug)
                        {
                            ca.log("变量现数值:" + variable);
                        }
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
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        byte a = (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((buf[x, y] * a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] *= (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
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
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        byte a = (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if ((a) == 0)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不能作为除数", i);
                        }
                        buf[x, y] /= (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
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
                    bool cond = false;
                    switch (param)
                    {
                        case 'u': targetY = y - 1; cond = buf[x, y] == buf[x, y - 1]; break;
                        case 'd': targetY = y + 1; cond = buf[x, y] == buf[x, y + 1]; break;
                        case 'l': targetX = x - 1; cond = buf[x, y] == buf[x - 1, y]; break;
                        case 'r': targetX = x + 1; cond = buf[x, y] == buf[x + 1, y]; break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }
                    if (debug)
                    {
                        ca.log("当前位置数值:" + buf[x, y] + ",目标数值:" + buf[targetX, targetY]);
                    }
                    if (cond)
                    {
                        i = search(i,']');
                    }
                }
                else if (op[i] == '{')
                {
                    char param = char.ToLower(op[i + 1]);
                    int targetX = x, targetY = y;
                    bool cond = false;
                    switch (param)
                    {
                        case 'u': targetY = y - 1; cond = buf[x, y] > buf[x, y - 1]; break;
                        case 'd': targetY = y + 1; cond = buf[x, y] > buf[x, y + 1]; break;
                        case 'l': targetX = x - 1; cond = buf[x, y] > buf[x - 1, y]; break;
                        case 'r': targetX = x + 1; cond = buf[x, y] > buf[x + 1, y]; break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }
                    if (debug)
                    {
                        ca.log("当前位置数值:" + buf[x, y] + ",目标数值:" + buf[targetX, targetY]);
                    }
                    if (cond)
                    {
                        i = search(i,'}');
                    }
                }
                else if (op[i] == '(')
                {
                    char param = char.ToLower(op[i + 1]);
                    int targetX = x, targetY = y;
                    bool cond = false;
                    switch (param)
                    {
                        case 'u': targetY = y - 1; cond = buf[x, y] < buf[x, y - 1]; break;
                        case 'd': targetY = y + 1; cond = buf[x, y] < buf[x, y + 1]; break;
                        case 'l': targetX = x - 1; cond = buf[x, y] < buf[x - 1, y]; break;
                        case 'r': targetX = x + 1; cond = buf[x, y] < buf[x + 1, y]; break;
                        default:
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                            break;
                    }
                    if (targetX < 0 || targetX >= bufsize || targetY < 0 || targetY >= bufsize)
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:缓冲区溢出", i);
                    }
                    if (debug)
                    {
                        ca.log("当前位置数值:" + buf[x, y] + ",目标数值:" + buf[targetX, targetY]);
                    }
                    if (cond)
                    {
                        i = search(i, ')');
                    }
                }
                else if (op[i] == '<')
                {
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        byte a = (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
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
                    if (Regex.IsMatch(op[i + 1].ToString(), "^[0-9A-Fa-f]+$"))
                    {
                        byte a = (byte)Convert.ToInt32(op[i + 1].ToString(), 16);
                        if (debug)
                        {
                            ca.log("参数:" + op[i + 1]);
                        }
                        if (i + 2 * a >= op.Length)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:跳转时计数器溢出", i);
                        }
                        i += a * 2;
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
                            stack.Push(buf[x,y]);
                            if(debug)
                                ca.log("已将" + buf[x, y]+"压入栈顶");
                            break;
                        case '-':
                            if(stack.Count == 0)
                            {
                                error("栈为空,不支持出栈",i);
                            }
                            buf[x,y]=stack.Pop();
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
                            stack.Push(buf[x,y]);
                            buf[x,y] = a;
                            if (debug)
                                ca.log("已将栈顶元素与缓冲区数值交换");
                            break;
                        case 's':
                            buf[x,y] = (byte)stack.Count;
                            if (debug)
                                ca.log("已写入缓冲区,当前栈中项数:"+buf[x,y]);
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
                        buf[x,y] += a;
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
                    else if(op[i + 1] == '*')
                    {
                        byte a = (byte)Console.ReadKey().KeyChar;
                        if ((buf[x, y] * a) > 255 && strict)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:Char类型溢出", i);
                        }
                        buf[x, y] *= a;
                    }
                    else if(op[i + 1] == '/')
                    {
                        byte a = (byte)Console.ReadKey().KeyChar;
                        if (a == 0 )
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:0不能作为除数", i);
                        }
                        buf[x, y] /= a;
                    }
                    else if (op[i + 1] == 'w' || op[i + 1] == 'W' || op[i+1] == '=')
                    {
                        buf[x, y] = (byte)Console.ReadKey().KeyChar;
                    }
                    else
                    {
                        error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未知的参数\"" + op[i + 1] + '\"', i);
                    }
                    if (debug)
                    {
                        Console.WriteLine("已写入["+x.ToString()+','+y.ToString()+"]");
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
                    if (subCmd == '+' || subCmd == '-' || subCmd == '*' || subCmd == '/' || subCmd == '=')
                    {
                        if (string.IsNullOrEmpty(inputfilename))
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未指定输入文件名", i);
                        }
                        try
                        {
                            if (inputFileStream == null)
                            {
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
                            }
                        }
                        catch (Exception ex)
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:文件操作异常 - " + ex.Message, i);
                        }
                    }
                    // 文件输出：c（字符）或 i（整数）
                    else if (subCmd == 'c' || subCmd == 'C' || subCmd == 'i' || subCmd == 'I')
                    {
                        if (string.IsNullOrEmpty(outputfilename))
                        {
                            error("在执行\"" + op[i] + op[i + 1] + "\"时出错,原因:未指定输出文件名", i);
                        }
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
                    error("未知的指令:" + op[i] + op[i + 1],i);
                }

                if (debug)
                {
                    ca.log("当前状态: x=" + x + ",y=" + y + ",num=" + buf[x, y] );
                    ca.log("当前运行指令:第" +i.ToString()+","+(i+1).ToString()+"个字符");
                    ca.log("下次运行指令:第" + (i+2).ToString() + "," + (i + 3).ToString() + "个字符\n==============================");
                }
                
            }
            
        }
        private static int search(int i,char c)
        {
            int q = 0;
            int q1 = 0;
            for (int j = i; j < op.Length - 1; j += 1)
            {
                if (op[j] == '/' && op[j + 1] == c)
                {
                    if (debug)
                    {
                        if((((i + j) / 2) + 2) < op.Length)
                        {
                            ca.log("索引到" +
                            (int)(((i + j) / 2) + 2) + ":" + op[(((i + j) / 2) + 2)] + op[(((i + j) / 2) + 3)] +
                            ",目前读取到的嵌套层数:" + q1.ToString() + "/" + q.ToString());
                        }
                        else
                        {
                            ca.log("索引到代码末尾");
                        }
                        
                        //ca.log("索引到" + (j + 2) + ":" + op[(j + 2)] + op[(j + 2)]);
                    }
                    if(q-q1 == 0)
                    {
                        return (((i + j) / 2)+2);
                    }
                    else
                    {
                        q1++;
                    }
                    //i = j + 2;
                }
                else if (op[j] == c && (op[j + 1] == 'l'|| op[j + 1] == 'r'|| op[j + 1] == 'u' || op[j + 1] == 'd'))
                {
                    q++;
                }
             }
            error("未能索引到后面的/"+c, i); 
            throw new IndexOutOfRangeException();
        }
        private static void error(string a,int idx)
        {
            if (debug)
            {
                ca.Warn("程序发生异常,错误位于第" + idx + "," + (idx + 1) + "个指令:" + op[idx] + op[idx+1]);
                System.Threading.Thread.Sleep(sleeptime);
            }
            ca.Error(a);
            ca.Warn("停止执行" + filename);
            Environment.Exit(0);
        }
        private static void error(string a)
        {
            if (debug)
            {
                ca.Warn("程序发生异常!");
                System.Threading.Thread.Sleep(sleeptime);
            }
            ca.Error(a);
            ca.Warn("停止执行" + filename);
            Environment.Exit(0);
        }
        public static string GetDirectoryPath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath)??"   ";
            if (string.IsNullOrEmpty(directory))
                return string.Empty;
            return directory + Path.DirectorySeparatorChar;
        }
        private static void exit(object? sender, ConsoleCancelEventArgs e)
        {
            if (inputFileStream != null)
                inputFileStream.Close();    
            ca.Warn("已手动停止执行程序"+filename);
            Environment.Exit(0);
        }
    }
}
