namespace SourceMapper.Tests
{
    internal class TestCloningContext : SourceMapperContext
    {
        protected override void Configure(MapBuilder mapBuilder)
        {
            mapBuilder
                //.MakeCloneable<TestClassDto>()
                //.MakeCloneable<VisiblityDto>()
                //.MakeCloneable<ReadonlyDto>()
                .MakeCloneable<NoDefConstDto>();
        }
    }
}
