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

