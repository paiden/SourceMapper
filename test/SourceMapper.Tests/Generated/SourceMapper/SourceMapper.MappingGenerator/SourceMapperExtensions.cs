namespace SourceMapper.Tests
{
    public static class NoDefConstDtoSourceMapperExtensions
    {
        public static NoDefConstDto Clone(this NoDefConstDto source)
        {
            var clone = new NoDefConstDto(source.X)
            {
                
            };

            return clone;
        }
    }
}

