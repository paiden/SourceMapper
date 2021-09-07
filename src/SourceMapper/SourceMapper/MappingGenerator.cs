using System;
using System.IO;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceMapper.Generators;
using SourceMapper.Parsers;
using SourceMapper.Utils;

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
            var codeText = reader.ReadToEnd();
            context.AddSource("SourceMapperContext", SourceText.From(codeText, Encoding.UTF8));

            // Write the genreated code based on target configured contexts
            var myCompilation = AddSourceMapperContextToCompilation(context.Compilation, codeText);

            var extensionsWriter = new CodegenTextWriter();
            foreach (var sourceMapperContext in receiver.Candidates)
            {
                DbgUtils.LaunchDebugger(true);
                var unitSyntax = sourceMapperContext.FirstAncestorOrSelf<CompilationUnitSyntax>();
                var semanticModel = myCompilation.GetSemanticModel(unitSyntax!.SyntaxTree);
                var parseContext = new ParseContext(myCompilation, semanticModel);
                var result = new ParseResult(context);
                ProcessContext(result, parseContext, extensionsWriter, sourceMapperContext);
            }

            context.AddSource("SourceMapperExtensions", SourceText.From(extensionsWriter.GetContents(), Encoding.UTF8));
        }

        private static void ProcessContext(
            ParseResult parseResult,
            ParseContext parseContext,
            CodegenTextWriter writer,
            ClassDeclarationSyntax sourceMapperContext)
        {
            ContextParser.Parse(parseResult, parseContext, sourceMapperContext);
            foreach (var t in parseResult.ConfiguredTypes)
            {
                GenerateExtensionsClass(writer, parseResult, t);
            }
        }

        private static void GenerateExtensionsClass(
            CodegenTextWriter writer,
            ParseResult parseResult,
            ITypeSymbol targetType)
        {
            writer.WithCBlock($"namespace {targetType.ContainingNamespace.ToDisplayString()}", ExtensionClassDeclaration);
            writer.WriteLine();

            void ExtensionClassDeclaration(CodegenTextWriter w)
            {
                w.WithCBlock($"public static class {targetType.Name}SourceMapperExtensions", ExtensionClassBody);
            }

            void ExtensionClassBody(CodegenTextWriter writer)
            {
                CloneableCodeGen.Generate(writer, parseResult, targetType);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAppDomainResolve;

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

        // It looks like source generated via the generattor themself are not part of the
        // compilation. But we need our generated context class in the compilation so that
        // we can resolve the configure calls correctly, so add them to it this logic
        private static Compilation AddSourceMapperContextToCompilation(Compilation compilation, string contextCode)
        {
            var fullCompilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(contextCode));
            return fullCompilation;
        }
    }
}
