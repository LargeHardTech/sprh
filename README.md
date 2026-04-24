# SPRH Programming Language

<p align="center">
  <img src="https://img.shields.io/badge/version-2.0-blue.svg" alt="Version 2.0"/>
  <img src="https://img.shields.io/badge/license-MIT-green.svg" alt="MIT License"/>
  <img src="https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg" alt="Platforms"/>
  <img src="https://img.shields.io/badge/.NET-8.0-purple.svg" alt=".NET 8.0"/>
  <img src="https://img.shields.io/badge/PRs-welcome-brightgreen.svg" alt="PRs Welcome"/>
</p>

**SPRH**（读作“S-P-R-H”）是一个极简但图灵完备的**教学用编程语言**，其名称源于设计哲学：

- **S**low – 执行速度慢（解释执行，让你感受每一步的变化）
- **P**oor Readability – 代码可读性差（类似 Brainfuck，挑战思维）
- **H**ard – 编程难度高（用最少的指令实现复杂逻辑）

灵感来源于二维图灵机模型，SPRH 提供了一个 **1024×1024 的字节网格**作为内存，一个**单字节寄存器**（变量），一个**字节栈**，以及丰富的指令集，包括算术、位运算、条件跳转、文件 I/O 等。它既可以作为**解释器**逐条运行，也可以通过内置的**编译器**转换为 C++ 代码，进而编译为本地机器码，获得接近原生的执行效率。

---

## ✨ 特性一览

- **二维内存网格**：1024×1024 个字节单元，通过 X/Y 指针访问。
- **单字节变量**：一个独立的寄存器，可与当前单元格进行算术/赋值操作。
- **字节栈**：支持压栈、弹栈、交换、清空、获取深度等操作。
- **算术运算**：加、减、乘、除（支持立即数或变量），以及置 255/0 的快捷方式。
- **按位运算**：与、或、异或、取反、左移、右移（可与相邻单元格进行）。
- **条件跳转**：基于当前单元格与相邻单元格的相等、大于、小于关系，实现循环和分支。
- **无条件跳转**：向前/向后相对跳转，支持 0~15 的步长。
- **输入/输出**：
  - 控制台输入（I 指令）可进行算术运算或直接赋值。
  - 文件输入（F 指令）从 `input.spri` 顺序读取字节并参与运算。
  - 文件输出（Fc/Fi）将当前单元格的值以字符或整数形式追加到 `output.spro`。
- **两种运行模式**：
  - **解释执行**：直接运行 `.sprh` 文件，适合调试和教学。
  - **编译执行**：将 SPRH 代码编译为 C++ 源码，再用任意 C++ 编译器生成原生可执行文件，速度大幅提升。
- **跨平台**：基于 .NET 8，可运行于 Windows、Linux、macOS；编译后的 C++ 程序同样可跨平台。

---

## 🚀 快速开始

### 获取 SPRH 解释器

#### 方法一：下载预编译可执行文件（推荐）
从 [Releases](https://github.com/LargeHardTech/sprh/releases) 页面下载对应平台的最新版本（例如 `sprh-win-x64.exe`），重命名为 `sprh`（或 `sprh.exe`）并放入 PATH 目录。

#### 方法二：从源码构建
```bash
git clone https://github.com/LargeHardTech/sprh.git
cd sprh/src/SprhInterpreter
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true
```
发布产物位于 `bin/Release/net8.0/win-x64/publish/`，单文件大小仅约 3MB。

### 运行第一个程序

创建一个文件 `hello.sprh`，内容如下：
```sprh
/* 输出 Hello, World! 的每个字符 */
=H Pc
=e Pc
=l Pc
=l Pc
=o Pc
=, Pc
=  Pc
=W Pc
=o Pc
=r Pc
=l Pc
=d Pc
=! Pc
```
在终端执行：
```bash
sprh hello.sprh
```
你将看到输出：`Hello, World!`

### 一个更有趣的例子：打印 0 到 50
```sprh
r1 =3 l1      /* 在 (1,0) 存放字符 '3'（ASCII 51）作为比较基准 */
pi +1         /* 输出当前值并加 1 */
[r <3 /]      /* 若当前值不等于右边单元格的值，则跳回 pi 继续循环 */
```
运行后输出：`01234567891011121314151617181920212223242526272829303132333435363738394041424344454647484950`

---

## 📜 指令参考

| 类别 | 指令 | 参数 | 说明 |
|------|------|------|------|
| **指针移动** | `U`/`D`/`L`/`R` | 1-F (hex) | 向上/下/左/右移动 n 步，越界报错 |
| **算术运算** | `+`/`-`/`*`/`/` | 1-F (hex) | 当前单元格加/减/乘/除立即数 |
| | `++` | | 当前单元格设为 255 |
| | `--` | | 当前单元格设为 0 |
| **赋值** | `=` | 任意 ASCII | 将字符的 ASCII 值写入当前单元格 |
| **输出** | `Pc`/`Pi` | | 以字符/整数形式输出当前单元格的值 |
| **变量** | `V=` | | 当前单元格的值 → 变量 |
| | `Vw` | | 变量 → 当前单元格 |
| | `V+`/`V-`/`V*`/`V/` | | 变量与当前单元格进行算术运算，结果存入变量 |
| **栈操作** | `S+` | | 当前单元格值压栈 |
| | `S-` | | 弹栈至当前单元格（栈空报错） |
| | `S=` | | 交换栈顶与当前单元格值（栈空报错） |
| | `Sc` | | 清空栈 |
| | `Ss` | | 栈大小（截断至 8 位）写入当前单元格 |
| **按位运算** | `&`/`|`/`^`/`~`/`,`/`.` | u/d/l/r | 与上/下/左/右单元格进行与/或/异或/取反/左移/右移，结果存入当前单元格 |
| **输入** | `I+`/`I-`/`I*`/`I/` | | 读取一个字符，其 ASCII 值与当前单元格进行运算 |
| | `I=`/`Iw` | | 读取一个字符，直接写入当前单元格 |
| **文件输入** | `F+`/`F-`/`F*`/`F/`/`F=` | | 从 `input.spri` 顺序读取一个字节，与当前单元格进行运算（读完后报错） |
| **文件输出** | `Fc`/`Fi` | | 将当前单元格的值以字符/整数形式追加到 `output.spro` |
| **无条件跳转** | `<`/`>` | 1-F (hex) | 向前/向后跳转 n 条指令（0 为无跳转） |
| **条件跳转** | `[`/`{`/`(` | u/d/l/r | 若当前值与相邻单元格相等/大于/小于，则跳转到对应的 `/]`、`/}`、`/)` 处（支持嵌套） |
| **注释** | `/*` ... `*/` | | 注释块，不支持嵌套 |

> 注：所有十六进制参数大小写不敏感（如 `A` 和 `a` 都表示 10）。

---

## 🛠️ 构建与安装

### 前置条件
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- （可选）C++ 编译器（如需将 SPRH 编译为 C++ 后再编译，如 GCC、Clang、MSVC）

### 从源码构建
```bash
git clone https://github.com/LargeHardTech/sprh.git
cd sprh/src/SprhInterpreter
dotnet build -c Release
```
编译后的可执行文件位于 `bin/Release/net8.0/`。

### 发布为独立可执行文件（NativeAOT）
```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true
```
输出单文件位于 `bin/Release/net8.0/win-x64/publish/`。

---

## 📖 示例程序

更多示例请查看 [examples](./examples) 目录：
- [`hello.sprh`](./examples/hello.sprh) – 输出 "Hello, World!"
- [`loop.sprh`](./examples/loop.sprh) – 打印 0~50
- [`fib.sprh`](./examples/fib.sprh) – 计算斐波那契数列
- [`fileio.sprh`](./examples/fileio.sprh) – 演示文件读写
- [`stack.sprh`](./examples/stack.sprh) – 栈操作示例
- [`bitops.sprh`](./examples/bitops.sprh) – 按位运算示例

---

## 🤝 贡献指南

欢迎任何形式的贡献！无论是报告 bug、提出新特性、改进文档，还是提交代码，我们都非常欢迎。

1. Fork 本仓库
2. 创建你的特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交你的修改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开一个 Pull Request

请确保代码风格与现有代码一致，并为新功能添加适当的测试。

---

## 📄 许可证

本项目基于 **MIT 许可证** 开源，详情请见 [LICENSE](./LICENSE) 文件。

---

## 🙏 致谢

- 灵感来源于 [Brainfuck](https://en.wikipedia.org/wiki/Brainfuck) 和二维图灵机模型
- 感谢 [Deepseek-R1](https://www.deepseek.com/) 在语言设计上的启发
- 感谢所有为 SPRH 提出建议和贡献代码的朋友们

---

**Happy SPRH coding!** 🧠💻
