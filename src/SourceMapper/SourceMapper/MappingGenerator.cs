using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using CodegenCS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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

            foreach (var ctx in receiver.Candidates)
            {
                ProcessContext(ctx);
            }
            var writer = new CodegenTextWriter();
            GenerateFile(writer, receiver);
            context.AddSource("SourceMapperExtensions", SourceText.From(writer.GetContents(), Encoding.UTF8));
        }

        private static void GenerateFile(CodegenTextWriter w, FindMapablesSyntaxReceiver syntaxReceiver)
        {
            w.Write($@"
using System;");
            w.WithCBlock("namespace X", w =>
            {
                foreach (var cds in syntaxReceiver.Candidates)
                    GenerateClass(w, cds);
            });
        }

        private static void ProcessContext(ClassDeclarationSyntax context)
        {
        }

        private static void GenerateClass(CodegenTextWriter w, ClassDeclarationSyntax cds)
        {
            w.WithCBlock($"public class {cds.Identifier.Text}", w =>
            {
                // Hello you
            });
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAppDomainResolve;


            if (true)
            {
                Debugger.Launch();
            }

            var dir = Environment.CurrentDirectory;

            context.RegisterForSyntaxNotifications(() => new FindMapablesSyntaxReceiver());
        }

        private Assembly HandleAppDomainResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("CodegenCS"))
            {
                return Assembly.LoadFrom(@"C:\temp\CodegenCS.dll");
            }

            return null;
        }
    }
}
