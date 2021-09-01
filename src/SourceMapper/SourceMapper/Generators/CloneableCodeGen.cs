using System;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using SourceMapper.Extensions;
using SourceMapper.Parsers;

namespace SourceMapper.Generators
{
    internal static class CloneableCodeGen
    {
        public static void Generate(CodegenTextWriter writer, ContextParseResult result, TypeInfo cloneableType)
        {
            var cloneable = result.ParseInfos[cloneableType];
            WriteCode(writer, cloneable);
        }

        private static void Foo(Func<object>? c)
        {
        }

        private static void WriteCode(CodegenTextWriter w, ParserTypeInfo cloneable)
        {
            var typeName = cloneable.TypeName;
            w.WithCBlock($"public static {typeName} Clone(this {typeName} source)", w => CloneBody(w, cloneable));


            static void CloneBody(CodegenTextWriter w, ParserTypeInfo cloneable)
            {
                var cloneableInfo = cloneable.GetCloneableInfo();
                if (cloneableInfo.BestConstructor == null)
                {
                    w.WriteLine($"#error No valid constructor for type '{cloneable.TypeName}' found");
                }
                else
                {
                    w.WriteCBlock(
                        GenerateMakeInstance(cloneableInfo),
                        w => WritePropertyAssignments(w, cloneableInfo),
                        ";");
                    w.WriteLine();
                    w.WriteLine($"return clone;");
                }
            }

            static string GenerateMakeInstance(CloneableParserTypeInfo cloneable)
            {
                return $"var clone = new {cloneable.TypeName}({GenerateConstructorArgs(cloneable)})";
            }

            static string GenerateConstructorArgs(CloneableParserTypeInfo cloneable)
            {
                var props = cloneable.ConstructionProps;
                var sb = new StringBuilder(256);
                foreach (var prop in props)
                {
                    sb.Append("source.")
                        .Append(prop.Name)
                        .Append(',')
                        .Append(' ');
                }

                sb.Remove(sb.Length - 2, 2);

                return sb.ToString();
            }

            static void WritePropertyAssignments(
                CodegenTextWriter w,
                CloneableParserTypeInfo cloneable)
            {
                foreach (var p in cloneable.AssignmentProps)
                {
                    w.EnsureEmptyLine();
                    w.Write($"{p.Name} = source.{p.Name},");
                }
            }
        }
    }
}
