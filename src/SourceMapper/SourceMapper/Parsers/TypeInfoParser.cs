using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceMapper.Parsers
{
    public class TypeInfoParser
    {
        public static ParserTypeInfo Parse(ITypeSymbol type)
        {
            var properties = type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(pi => !pi.IsReadOnly)
                .Where(pi => !pi.IsStatic)
                .Where(pi => pi.DeclaredAccessibility != Accessibility.Private);

            var constructors = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility != Accessibility.Private)
                .Where(m => m.MethodKind == MethodKind.Constructor);

            return new ParserTypeInfo(type.Name, properties, constructors);
        }
    }
}
