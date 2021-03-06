using System;
using System.IO;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceMapper.Codegen;
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
            context.AddSource("SourceMapperContext.g", SourceText.From(codeText, Encoding.UTF8));

            // Write the genreated code based on target configured contexts
            var myCompilation = AddSourceMapperContextToCompilation(context.Compilation, codeText);

            foreach (var sourceMapperContext in receiver.Candidates)
            {
                var extensionsWriter = new CodegenTextWriter();

                extensionsWriter.WriteLine("using System;");
                extensionsWriter.WriteLine();

                DbgUtils.LaunchDebugger(true);
                var unitSyntax = sourceMapperContext.FirstAncestorOrSelf<CompilationUnitSyntax>();
                var semanticModel = myCompilation.GetSemanticModel(unitSyntax!.SyntaxTree);
                var parseContext = new ParseContext(myCompilation, semanticModel);
                var result = new ParseResult(context, sourceMapperContext.Identifier.ValueText);
                ProcessContext(result, parseContext, extensionsWriter, sourceMapperContext);
                context.AddSource(result.ContextName + ".g", SourceText.From(extensionsWriter.GetContents(), Encoding.UTF8));
            }
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
                ExtensionsClassCodeGen.GenerateExtensionsClass(writer, parseResult, t);
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
