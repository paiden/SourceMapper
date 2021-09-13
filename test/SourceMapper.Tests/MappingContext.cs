namespace SourceMapper.Tests
{
    internal class MappingContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<TestClass>(it => it
                .MapTo<TestClassDto>());
            config.Make<LambdaActivatorObj>(it => it
                .MapTo<FuncActivatorObj>(map => map
                    .Activator(x => new FuncActivatorObj($"Custom Activator Lambda + {x.Prop}"))));
        }
    }
}
