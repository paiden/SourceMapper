namespace SourceMapper.Tests
{
    internal class TestCloningContext : SourceMapperContext
    {
        protected override void Configure(MappingConfig config)
        {
            config
                .Make<IgnorePropDto>(it => it
                    .Cloneable(cloning => cloning
                        .Ignore(x => x.IgnoreMe)));
            config
                .Make<VisiblityDto>(it => it
                    .Cloneable());
            config
                .Make<WithCloneablePropertyDto>(it => it
                    .Cloneable());

        }
    }
}
