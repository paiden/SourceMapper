namespace SourceMapper.Tests
{
    public static class IgnorePropDtoSourceMapperExtensions
    {
        public static IgnorePropDto Clone(this IgnorePropDto source)
        {
            var clone = new IgnorePropDto()
            {
                CloneMe = source.CloneMe,
            };

            return clone;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class VisiblityDtoSourceMapperExtensions
    {
        public static VisiblityDto Clone(this VisiblityDto source)
        {
            var clone = new VisiblityDto()
            {
                PublicString = source.PublicString,
                InternalString = source.InternalString,
            };

            return clone;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class WithCloneablePropertyDtoSourceMapperExtensions
    {
        public static WithCloneablePropertyDto Clone(this WithCloneablePropertyDto source)
        {
            var clone = new WithCloneablePropertyDto(source.InnerGetOnly.Clone())
            {
                Prop = source.Prop,
                Inner = source.Inner.Clone(),
            };

            return clone;
        }
    }
}

