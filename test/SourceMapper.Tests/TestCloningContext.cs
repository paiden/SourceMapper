namespace SourceMapper.Tests
{
    internal class TestCloningContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
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
            config
                .Make<TestClass>(it => it
                    .MapTo<TestClassDto>());
            config
                .Make<ObjWithPostProcessFunc>(it => it
                    .Cloneable(cloning => cloning
                        .PostProcess(ObjWithPostProcessFunc.PostProc)));
            config
                .Make<LambdaPostProcObj>(it => it
                    .Cloneable(cloning => cloning
                        .PostProcess((ref LambdaPostProcObj a, LambdaPostProcObj b) => a.Prop = b.Prop + " Post Processed")));
        }
    }
}
