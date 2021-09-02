namespace SourceMapper.Tests
{
    internal class TestCloningContext : SourceMapperContext
    {
        protected override void Configure(MappingConfig config)
        {
            config
                //.MakeCloneable<TestClassDto>()
                //.MakeCloneable<VisiblityDto>()
                //.MakeCloneable<ReadonlyDto>()
                //.MakeCloneable<NoDefConstDto>()
                .Make<IgnorePropDto>(it => it
                    .Cloneable(cloning => cloning
                        .Ignore(x => x.IgnoreMe)));
        }
    }
}
