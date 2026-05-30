using System;

namespace sprh
{
    /// <summary>
    /// SPRH 运行时异常，用于替代 Environment.Exit，
    /// 使上层代码可以统一进行资源清理。
    /// </summary>
    internal class SprhRuntimeException : Exception
    {
        public int InstructionIndex { get; }

        public SprhRuntimeException(string message, int instructionIndex)
            : base(message)
        {
            InstructionIndex = instructionIndex;
        }
    }
}
