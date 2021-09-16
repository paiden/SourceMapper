using SourceMapper.Tests.MappingTestObjects;

namespace SourceMapper.Tests
{
    internal class MappingContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
            .Make<CombinePropSource>(it => it
                .MapTo<CombinePropTarget>(mapping => mapping
                    .Ignore(src => src.A)
                    .Ignore(src => src.B)
                    .PostProcess((ref CombinePropTarget tgt, CombinePropSource src) => tgt.AB = src.A + src.B)));
            config
                .Make<TestClass>(it => it
                .MapTo<TestClassDto>());
            config.Make<LambdaActivatorObj>(it => it
                .MapTo<FuncActivatorObj>(map => map
                    .Activator(x => new FuncActivatorObj($"Custom Activator Lambda + {x.Prop}"))));
            config.Make<MissingTgtPropSource>(it => it
                .MapTo<MissingTgtPropTarget>());
        }
    }
}
