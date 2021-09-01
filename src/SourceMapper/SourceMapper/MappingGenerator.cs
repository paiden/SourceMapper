using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceMapper.Generators;
using SourceMapper.Parsers;

namespace SourceMapper
{
    [Generator]
    public class MappingGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (FindMapablesSyntaxReceiver)context.SyntaxReceiver;

            // Write the Context file (output precreated class)
            var names = typeof(MappingGenerator).Assembly.GetManifestResourceNames();
            var contextCodeStream = typeof(MappingGenerator).Assembly
                .GetManifestResourceStream("SourceMapper.SourceMapperContext.cs.txt");
            var reader = new StreamReader(contextCodeStream);
            context.AddSource("SourceMapperContext", SourceText.From(reader.ReadToEnd(), Encoding.UTF8));

            // Write the genreated code based on target configured contexts


            var extensionsWriter = new CodegenTextWriter();
            foreach (var ctx in receiver.Candidates)
            {
                ProcessContext(context.Compilation, ctx, extensionsWriter);
            }
            context.AddSource("SourceMapperExtensions", SourceText.From(extensionsWriter.GetContents(), Encoding.UTF8));
        }

        private static void ProcessContext(Compilation compilation, ClassDeclarationSyntax context, CodegenTextWriter writer)
        {
            var parseResult = ContextParser.Parse(compilation, context);
            foreach (var t in parseResult.ConfiguredTypes)
            {
                GenerateExtensionsClass(writer, parseResult, t);
            }
        }

        private static void GenerateExtensionsClass(
            CodegenTextWriter writer,
            ContextParseResult parseResult,
            TypeInfo targetType)
        {
            writer.WithCBlock($"namespace {targetType.Type.ContainingNamespace.ToDisplayString()}", ExtensionClassDeclaration);
            writer.WriteLine();

            void ExtensionClassDeclaration(CodegenTextWriter w)
            {
                w.WithCBlock($"public static class {targetType.Type.Name}SourceMapperExtensions", ExtensionClassBody);
            }

            void ExtensionClassBody(CodegenTextWriter writer)
            {
                CloneableCodeGen.Generate(writer, parseResult, targetType);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAppDomainResolve;

            if (false)
            {
                Debugger.Launch();
            }

            var dir = Environment.CurrentDirectory;

            context.RegisterForSyntaxNotifications(() => new FindMapablesSyntaxReceiver());
        }

        private System.Reflection.Assembly HandleAppDomainResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("CodegenCS"))
            {
                return System.Reflection.Assembly.LoadFrom(@"C:\temp\CodegenCS.dll");
            }

            return null;
        }
    }
}
