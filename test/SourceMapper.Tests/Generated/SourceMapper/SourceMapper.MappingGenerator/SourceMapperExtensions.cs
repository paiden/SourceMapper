namespace SourceMapper.Tests
{
    public static class TestClassSourceMapperExtensions
    {
        public static TestClass MapTo<T>(this TestClass source) where T : TestClassDto
        {
            var obj = new TestClass()
            {
                F = source.F,
                D = source.D,
                S = source.S,
            };

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class IgnorePropDtoSourceMapperExtensions
    {
        public static IgnorePropDto Clone(this IgnorePropDto source)
        {
            var obj = new IgnorePropDto()
            {
                CloneMe = source.CloneMe,
            };

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class VisiblityDtoSourceMapperExtensions
    {
        public static VisiblityDto Clone(this VisiblityDto source)
        {
            var obj = new VisiblityDto()
            {
                PublicString = source.PublicString,
                InternalString = source.InternalString,
            };

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class WithCloneablePropertyDtoSourceMapperExtensions
    {
        public static WithCloneablePropertyDto Clone(this WithCloneablePropertyDto source)
        {
            var obj = new WithCloneablePropertyDto(source.InnerGetOnly.Clone())
            {
                Prop = source.Prop,
                Inner = source.Inner.Clone(),
            };

            return obj;
        }
    }
}

