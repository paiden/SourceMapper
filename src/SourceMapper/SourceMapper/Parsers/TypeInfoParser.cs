using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceMapper.Parsers
{
    public class TypeInfoParser
    {
        public static ParserTypeInfo? Parse(TypeInfo type)
        {
            if (type.Type == null)
            {
                return null;
            }

            var properties = type.Type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(pi => !pi.IsReadOnly)
                .Where(pi => !pi.IsStatic)
                .Where(pi => pi.DeclaredAccessibility != Accessibility.Private);

            var constructors = type.Type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility != Accessibility.Private)
                .Where(m => m.MethodKind == MethodKind.Constructor);

            return new ParserTypeInfo(type.Type.Name, properties, constructors);
        }
    }
}
