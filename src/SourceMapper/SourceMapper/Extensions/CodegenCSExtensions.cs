using System;
using CodegenCS;

namespace SourceMapper.Extensions
{
    public static class CodegenCSExtensions
    {
        public static void WriteCBlock(
            this CodegenTextWriter w,
            string? beforeBlock,
            Action<CodegenTextWriter> innerBlock,
            string? afterBlock)
        {
            w.WriteLine($@"
                {beforeBlock}
                {{
                    {innerBlock}
                }}{afterBlock}");
        }
    }
}
