using Microsoft.CodeAnalysis;

namespace SourceMapper.Parsers
{
    internal sealed class ParseContext
    {
        public Compilation Compilation { get; }

        public SemanticModel SemanticModel { get; }

        public ParseContext(Compilation compilation, SemanticModel model)
        {
            this.Compilation = compilation;
            this.SemanticModel = model;
        }
    }
}
