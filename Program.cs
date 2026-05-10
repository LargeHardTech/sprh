using System.Text;
using System.Text.RegularExpressions;

namespace sprh
{
    internal class Program
    {
        
        public static string version = "2.1";
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

        public static readonly int sleeptime = 100;

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
            }else if(args.Length == 1)
            {
                if(args[0] == "-ver" || args[0] == "-version")
                {
                    Console.WriteLine("LargeHard SPRH Interpreter v"+version);
                    return;
                }
            }
            
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
                else if (args[i].ToLower() == "-debug" || args[i].ToLower() == "-d")
                {
                    debug = true;
                }
                else if (args[i].ToLower() == "-timer" || args[i].ToLower() == "-t")
                {
                    timer = true;
                }
                else if (args[i].ToLower() == "-strict" || args[i].ToLower() == "-s")
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
                Console.ResetColor();   
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
                    ca.log("自动补全输出文件到:" + inputfilename);
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
                Console.ResetColor();
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
                Interpreter i = new Interpreter
                {
                    op = op,
                    debug = debug,
                    timer = timer,
                    sleeptime = sleeptime,
                    strict = strict,
                    filename = filename,
                    inputfilename = inputfilename,
                    outputfilename = outputfilename,
                };
                i.r();
            }
            if (timer)
            {
                end = DateTime.Now;
                Console.Write('\n');
                ca.Warn("停止计时!");
                ca.Warn("时间(毫秒):"+(double)((end - start).TotalMilliseconds));//','+(double)start.Millisecond+','+end.Millisecond
            }
            if (Interpreter.inputFileStream != null)
                Interpreter.inputFileStream.Close();
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
                BufferSize = Interpreter.bufsize
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
        
        private static DateTime start;
        private static DateTime end;
        public static string GetDirectoryPath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath)??"   ";
            if (string.IsNullOrEmpty(directory))
                return string.Empty;
            return directory + Path.DirectorySeparatorChar;
        }
        private static void exit(object? sender, ConsoleCancelEventArgs e)
        {
            if (Interpreter.inputFileStream != null)
                Interpreter.inputFileStream.Close();    
            ca.Warn("已手动停止执行程序"+filename);
            Console.ResetColor();
            Environment.Exit(0);
        }
    }
}
