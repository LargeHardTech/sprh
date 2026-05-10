using System;

namespace sprh
{
    internal static class Help
    {
        private static void NextPage()
        {
            Console.WriteLine("\n--- 按任意键继续下一页 ---");
            Console.ReadKey(true);
            Console.Clear();
        }

        public static void help()
        {
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("               SPRH 语言帮助                ");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("【简介】");
            Console.WriteLine("SPRH 是一种基于二维图灵机模型的极简编程语言，名称源于其设计哲学：");
            Console.WriteLine("  • Slow         – 执行速度慢（解释执行）");
            Console.WriteLine("  • Poor Readability – 代码可读性差（类似 Brainfuck）");
            Console.WriteLine("  • Hard         – 编程难度高（锻炼思维）");
            Console.WriteLine();
            Console.WriteLine($"当前版本：{Program.version}　　缓冲区大小：{Interpreter.bufsize} × {Interpreter.bufsize}");
            Console.WriteLine("作者：巨硬科技LHT");
            Console.WriteLine();

            Console.WriteLine("【使用方法】");
            Console.WriteLine("  sprh <文件名.sprh> [选项]");
            Console.WriteLine();
            Console.WriteLine("选项：");
            Console.WriteLine("  -d, -debug     调试模式，每执行一条指令打印状态信息");
            Console.WriteLine("  -s, -strict    严格模式，检查算术溢出（char 超出 0~255 时报错）");
            Console.WriteLine("  -t, -timer     计时模式，输出程序运行时间");
            Console.WriteLine("  -c, -tocpp     编译为 C++ 文件（需同时指定输出 .cpp 文件名）");
            Console.WriteLine("  <文件名.spri/o> 从选择的文件里输入/输出字符，默认input.spri，output.spro");
            Console.WriteLine("  /?              显示本帮助");
            Console.WriteLine();
            Console.WriteLine("示例：");
            Console.WriteLine("  sprh test.sprh -d -s");
            Console.WriteLine("  sprh test.sprh -c output.cpp 1.spri 2.spro");
            Console.WriteLine();

            Console.WriteLine("【语法格式】");
            Console.WriteLine("每条指令由两个 ASCII 字符组成：命令符 + 参数。");
            Console.WriteLine("参数可以是十六进制数字（0-9 A-F a-f）或特定符号（见下表）。");
            Console.WriteLine("注释使用 /* ... */，不支持嵌套，编译器会忽略注释内容。");
            Console.WriteLine();

            NextPage();
            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("1. 移动指针（修改 X,Y 坐标）");
            Console.WriteLine("   U*    Y 减少 *（向上）");
            Console.WriteLine("   D*    Y 增加 *（向下）");
            Console.WriteLine("   L*    X 减少 *（向左）");
            Console.WriteLine("   R*    X 增加 *（向右）");
            Console.WriteLine("   * 可为十六进制数（0~F）、'v'（按变量值移动）或 '?'（随机移动）。");
            Console.WriteLine("   使用 'v' 时移动距离等于变量 variable 当前值；");
            Console.WriteLine("   使用 '?' 时向上/左可移动 1~当前坐标，向下/右可移动 1~剩余空间。");
            Console.WriteLine("   非严格模式下越界会自动移动到边界，严格模式则会报错。");
            Console.WriteLine();

            Console.WriteLine("2. 算术运算（修改当前单元格的值）");
            Console.WriteLine("   +*    当前值加 *（十六进制，0~F）");
            Console.WriteLine("   -*    当前值减 *（十六进制，0~F）");
            Console.WriteLine("   **    当前值乘 *（十六进制）");
            Console.WriteLine("   /*    当前值除以 *（十六进制，*≠0）");
            Console.WriteLine("   ++    将当前单元格设为 255");
            Console.WriteLine("   --    将当前单元格设为 0");
            Console.WriteLine("   +?    随机增加（至少加1，不超过255，满则警告）");
            Console.WriteLine("   -?    随机减少（至少减1，不小于0，空则警告）");
            Console.WriteLine();

            NextPage();

            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("3. 赋值与读写");
            Console.WriteLine("   =?    将 ASCII 字符 ? 写入当前单元格");
            Console.WriteLine("   Pi    以整数形式输出当前单元格的值");
            Console.WriteLine("   Pc    以字符形式输出当前单元格的值");
            Console.WriteLine();

            Console.WriteLine("4. 变量操作（单字节寄存器 variable）");
            Console.WriteLine("   V=    将当前单元格的值复制到 variable");
            Console.WriteLine("   Vw    将 variable 的值写入当前单元格");
            Console.WriteLine("   V+    variable += 当前单元格的值（严格模式检查溢出）");
            Console.WriteLine("   V-    variable -= 当前单元格的值");
            Console.WriteLine("   V*    variable *= 当前单元格的值");
            Console.WriteLine("   V/    variable /= 当前单元格的值（除数不能为0）");
            Console.WriteLine("   V?    将 variable 设为随机值（0~255）");
            Console.WriteLine();

            NextPage();

            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("5. 栈操作（Stack<byte>）");
            Console.WriteLine("   S+    将当前单元格的值压入栈");
            Console.WriteLine("   S-    弹出栈顶值并写入当前单元格（栈空则报错）");
            Console.WriteLine("   S=    交换栈顶值与当前单元格的值（栈空则报错）");
            Console.WriteLine("   Sc    清空栈");
            Console.WriteLine("   Ss    将当前栈的深度（元素个数）写入当前单元格（若深度>255则截断）");
            Console.WriteLine();

            Console.WriteLine("6. 按位运算（与相邻单元格进行位操作）");
            Console.WriteLine("   &x    当前值 &= 相邻单元格的值");
            Console.WriteLine("   |x    当前值 |= 相邻单元格的值");
            Console.WriteLine("   ^x    当前值 ^= 相邻单元格的值");
            Console.WriteLine("   ~x    当前值 = ~相邻单元格的值（取反）");
            Console.WriteLine("   ,x    当前值左移，移位数等于相邻单元格的值（保留低8位）");
            Console.WriteLine("   .x    当前值右移，移位数等于相邻单元格的值");
            Console.WriteLine("   x 为方向：u（上）、d（下）、l（左）、r（右）");
            Console.WriteLine("   相邻单元格必须存在，否则报缓冲区溢出。");
            Console.WriteLine();

            NextPage();

            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("7. 输入操作");
            Console.WriteLine("   I+    读取一个字符，其 ASCII 值加到当前单元格");
            Console.WriteLine("   I-    读取一个字符，当前单元格减去该值");
            Console.WriteLine("   I*    读取一个字符，当前单元格乘以该值");
            Console.WriteLine("   I/    读取一个字符，当前单元格除以该值（除数不能为0）");
            Console.WriteLine("   I= 或 Iw  读取一个字符，直接写入当前单元格");
            Console.WriteLine();

            Console.WriteLine("8. 文件操作");
            Console.WriteLine("   输入文件默认为源码目录下的 input.spri，输出文件默认为 output.spro，可用命令行参数指定。");
            Console.WriteLine("   F+    从输入文件读取一个字节，加到当前单元格");
            Console.WriteLine("   F-    从输入文件读取一个字节，当前单元格减去该值");
            Console.WriteLine("   F*    从输入文件读取一个字节，当前单元格乘以该值");
            Console.WriteLine("   F/    从输入文件读取一个字节，当前单元格除以该值（除数不能为0）");
            Console.WriteLine("   F=    从输入文件读取一个字节，直接写入当前单元格");
            Console.WriteLine("   Fc    将当前单元格的值以字符形式追加到输出文件");
            Console.WriteLine("   Fi    将当前单元格的值以整数形式（十进制）追加到输出文件");
            Console.WriteLine("   注意：文件按顺序读取，读完后继续执行会报错。");
            Console.WriteLine();

            NextPage();

            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("9. 无条件跳转（相对偏移）");
            Console.WriteLine("   <*    向前跳转 * 条指令（* 为十六进制数，0 表示不跳转）");
            Console.WriteLine("   >*    向后跳转 * 条指令（* 为十六进制数，0 表示不跳转）");
            Console.WriteLine("   ※ 跳转目标以指令为单位，向前跳转不能超出程序开头。");
            Console.WriteLine();

            Console.WriteLine("10. 条件跳转（比较当前单元格与相邻单元格）");
            Console.WriteLine("   [x    若当前值 == 相邻值，则跳转到对应的 /] 处（支持嵌套）");
            Console.WriteLine("   {x    若当前值 >  相邻值，则跳转到对应的 /} 处（支持嵌套）");
            Console.WriteLine("   (x    若当前值 <  相邻值，则跳转到对应的 /) 处（支持嵌套）");
            Console.WriteLine("   x 为方向：u（上）、d（下）、l（左）、r（右）");
            Console.WriteLine("   相邻单元格必须存在，否则报缓冲区溢出。");
            Console.WriteLine();

            NextPage();

            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("11. 其他");
            Console.WriteLine("   /* */ 注释块（不支持嵌套）");
            Console.WriteLine();

            Console.WriteLine("【简单示例】");
            Console.WriteLine("输出数字 0~50：");
            Console.WriteLine("  r1 =3 l1 pi +1 [r <3 /]");
            Console.WriteLine("解释：");
            Console.WriteLine("  r1        → 右移一格");
            Console.WriteLine("  =3        → 写入字符 '3' (ASCII 51) 作为比较基准");
            Console.WriteLine("  l1        → 左移回原位");
            Console.WriteLine("  pi +1     → 输出当前值并加1");
            Console.WriteLine("  [r <3 /]  → 若当前值不等于右边(51)则向前跳3条指令（回到 pi）继续循环；否则跳出");
            Console.WriteLine();

            Console.WriteLine("【文件操作示例】");
            Console.WriteLine("从 input.spri 读取一个字节，将其加 1 后写入 output.spro：");
            Console.WriteLine("  f= +1 fc");
            Console.WriteLine();

            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("\n--- 按任意键退出帮助 ---");
            Console.ReadKey(true);
        }

        public static void helpNoInp()
        {
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("               SPRH 语言帮助                ");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("【简介】");
            Console.WriteLine("SPRH 是一种基于二维图灵机模型的极简编程语言，名称源于其设计哲学：");
            Console.WriteLine("  • Slow         – 执行速度慢（解释执行）");
            Console.WriteLine("  • Poor Readability – 代码可读性差（类似 Brainfuck）");
            Console.WriteLine("  • Hard         – 编程难度高（锻炼思维）");
            Console.WriteLine();
            Console.WriteLine($"当前版本：{Program.version}　　缓冲区大小：{Interpreter.bufsize} × {Interpreter.bufsize}");
            Console.WriteLine("作者：巨硬科技LHT");
            Console.WriteLine();

            Console.WriteLine("【使用方法】");
            Console.WriteLine("  sprh <文件名.sprh> [选项]");
            Console.WriteLine();
            Console.WriteLine("选项：");
            Console.WriteLine("  -d, -debug     调试模式，每执行一条指令打印状态信息");
            Console.WriteLine("  -s, -strict    严格模式，检查算术溢出（char 超出 0~255 时报错）");
            Console.WriteLine("  -t, -timer     计时模式，输出程序运行时间");
            Console.WriteLine("  -c, -tocpp     编译为 C++ 文件（需同时指定输出 .cpp 文件名）");
            Console.WriteLine("  <文件名.spri/o> 从选择的文件里输入/输出字符，默认input.spri，output.spro");
            Console.WriteLine("  /?              显示本帮助");
            Console.WriteLine();
            Console.WriteLine("示例：");
            Console.WriteLine("  sprh test.sprh -d -s");
            Console.WriteLine("  sprh test.sprh -c output.cpp 1.spri 2.spro");
            Console.WriteLine();

            Console.WriteLine("【语法格式】");
            Console.WriteLine("每条指令由两个 ASCII 字符组成：命令符 + 参数。");
            Console.WriteLine("参数可以是十六进制数字（0-9 A-F a-f）或特定符号（见下表）。");
            Console.WriteLine("注释使用 /* ... */，不支持嵌套，编译器会忽略注释内容。");
            Console.WriteLine();

            NextPage();
            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("1. 移动指针（修改 X,Y 坐标）");
            Console.WriteLine("   U*    Y 减少 *（向上）");
            Console.WriteLine("   D*    Y 增加 *（向下）");
            Console.WriteLine("   L*    X 减少 *（向左）");
            Console.WriteLine("   R*    X 增加 *（向右）");
            Console.WriteLine("   * 可为十六进制数（0~F）、'v'（按变量值移动）或 '?'（随机移动）。");
            Console.WriteLine("   使用 'v' 时移动距离等于变量 variable 当前值；");
            Console.WriteLine("   使用 '?' 时向上/左可移动 1~当前坐标，向下/右可移动 1~剩余空间。");
            Console.WriteLine("   非严格模式下越界会自动移动到边界，严格模式则会报错。");
            Console.WriteLine();

            Console.WriteLine("2. 算术运算（修改当前单元格的值）");
            Console.WriteLine("   +*    当前值加 *（十六进制，0~F）");
            Console.WriteLine("   -*    当前值减 *（十六进制，0~F）");
            Console.WriteLine("   **    当前值乘 *（十六进制）");
            Console.WriteLine("   /*    当前值除以 *（十六进制，*≠0）");
            Console.WriteLine("   ++    将当前单元格设为 255");
            Console.WriteLine("   --    将当前单元格设为 0");
            Console.WriteLine("   +?    随机增加（至少加1，不超过255，满则警告）");
            Console.WriteLine("   -?    随机减少（至少减1，不小于0，空则警告）");
            Console.WriteLine();

            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("3. 赋值与读写");
            Console.WriteLine("   =?    将 ASCII 字符 ? 写入当前单元格");
            Console.WriteLine("   Pi    以整数形式输出当前单元格的值");
            Console.WriteLine("   Pc    以字符形式输出当前单元格的值");
            Console.WriteLine();

            Console.WriteLine("4. 变量操作（单字节寄存器 variable）");
            Console.WriteLine("   V=    将当前单元格的值复制到 variable");
            Console.WriteLine("   Vw    将 variable 的值写入当前单元格");
            Console.WriteLine("   V+    variable += 当前单元格的值（严格模式检查溢出）");
            Console.WriteLine("   V-    variable -= 当前单元格的值");
            Console.WriteLine("   V*    variable *= 当前单元格的值");
            Console.WriteLine("   V/    variable /= 当前单元格的值（除数不能为0）");
            Console.WriteLine("   V?    将 variable 设为随机值（0~255）");
            Console.WriteLine();


            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("5. 栈操作（Stack<byte>）");
            Console.WriteLine("   S+    将当前单元格的值压入栈");
            Console.WriteLine("   S-    弹出栈顶值并写入当前单元格（栈空则报错）");
            Console.WriteLine("   S=    交换栈顶值与当前单元格的值（栈空则报错）");
            Console.WriteLine("   Sc    清空栈");
            Console.WriteLine("   Ss    将当前栈的深度（元素个数）写入当前单元格（若深度>255则截断）");
            Console.WriteLine();

            Console.WriteLine("6. 按位运算（与相邻单元格进行位操作）");
            Console.WriteLine("   &x    当前值 &= 相邻单元格的值");
            Console.WriteLine("   |x    当前值 |= 相邻单元格的值");
            Console.WriteLine("   ^x    当前值 ^= 相邻单元格的值");
            Console.WriteLine("   ~x    当前值 = ~相邻单元格的值（取反）");
            Console.WriteLine("   ,x    当前值左移，移位数等于相邻单元格的值（保留低8位）");
            Console.WriteLine("   .x    当前值右移，移位数等于相邻单元格的值");
            Console.WriteLine("   x 为方向：u（上）、d（下）、l（左）、r（右）");
            Console.WriteLine("   相邻单元格必须存在，否则报缓冲区溢出。");
            Console.WriteLine();


            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("7. 输入操作");
            Console.WriteLine("   I+    读取一个字符，其 ASCII 值加到当前单元格");
            Console.WriteLine("   I-    读取一个字符，当前单元格减去该值");
            Console.WriteLine("   I*    读取一个字符，当前单元格乘以该值");
            Console.WriteLine("   I/    读取一个字符，当前单元格除以该值（除数不能为0）");
            Console.WriteLine("   I= 或 Iw  读取一个字符，直接写入当前单元格");
            Console.WriteLine();

            Console.WriteLine("8. 文件操作");
            Console.WriteLine("   输入文件默认为源码目录下的 input.spri，输出文件默认为 output.spro，可用命令行参数指定。");
            Console.WriteLine("   F+    从输入文件读取一个字节，加到当前单元格");
            Console.WriteLine("   F-    从输入文件读取一个字节，当前单元格减去该值");
            Console.WriteLine("   F*    从输入文件读取一个字节，当前单元格乘以该值");
            Console.WriteLine("   F/    从输入文件读取一个字节，当前单元格除以该值（除数不能为0）");
            Console.WriteLine("   F=    从输入文件读取一个字节，直接写入当前单元格");
            Console.WriteLine("   Fc    将当前单元格的值以字符形式追加到输出文件");
            Console.WriteLine("   Fi    将当前单元格的值以整数形式（十进制）追加到输出文件");
            Console.WriteLine("   注意：文件按顺序读取，读完后继续执行会报错。");
            Console.WriteLine();


            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("9. 无条件跳转（相对偏移）");
            Console.WriteLine("   <*    向前跳转 * 条指令（* 为十六进制数，0 表示不跳转）");
            Console.WriteLine("   >*    向后跳转 * 条指令（* 为十六进制数，0 表示不跳转）");
            Console.WriteLine("   ※ 跳转目标以指令为单位，向前跳转不能超出程序开头。");
            Console.WriteLine();

            Console.WriteLine("10. 条件跳转（比较当前单元格与相邻单元格）");
            Console.WriteLine("   [x    若当前值 == 相邻值，则跳转到对应的 /] 处（支持嵌套）");
            Console.WriteLine("   {x    若当前值 >  相邻值，则跳转到对应的 /} 处（支持嵌套）");
            Console.WriteLine("   (x    若当前值 <  相邻值，则跳转到对应的 /) 处（支持嵌套）");
            Console.WriteLine("   x 为方向：u（上）、d（下）、l（左）、r（右）");
            Console.WriteLine("   相邻单元格必须存在，否则报缓冲区溢出。");
            Console.WriteLine();


            Console.WriteLine("【指令详表】");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("11. 其他");
            Console.WriteLine("   /* */ 注释块（不支持嵌套）");
            Console.WriteLine();

            Console.WriteLine("【简单示例】");
            Console.WriteLine("输出数字 0~50：");
            Console.WriteLine("  r1 =3 l1 pi +1 [r <3 /]");
            Console.WriteLine("解释：");
            Console.WriteLine("  r1        → 右移一格");
            Console.WriteLine("  =3        → 写入字符 '3' (ASCII 51) 作为比较基准");
            Console.WriteLine("  l1        → 左移回原位");
            Console.WriteLine("  pi +1     → 输出当前值并加1");
            Console.WriteLine("  [r <3 /]  → 若当前值不等于右边(51)则向前跳3条指令（回到 pi）继续循环；否则跳出");
            Console.WriteLine();

            Console.WriteLine("【文件操作示例】");
            Console.WriteLine("从 input.spri 读取一个字节，将其加 1 后写入 output.spro：");
            Console.WriteLine("  f= +1 fc");
            Console.WriteLine();

            Console.WriteLine("═══════════════════════════════════════════");

        }
        public static void introduce()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("        欢迎使用 SPRH 语言解释器！          ");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("输入 sprh /? 查看详细帮助。");
            Console.WriteLine("按任意键继续...");
            Console.ReadKey(true);
            Console.Clear();
            help();
        }
    }
}