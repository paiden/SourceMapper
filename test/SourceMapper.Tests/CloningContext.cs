using SomeCustomNamespace;
using SourceMapper.Tests.CloneTestObjects;

namespace SourceMapper.Tests
{
    internal class CloningContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config.Make<InternalCloneable>(it => it.Cloneable());
            config
                .Make<VisiblityDto>(it => it.Cloneable())
                .Make<WithCloneablePropertyDto>(it => it.Cloneable());
            config
                .Make<IgnorePropDto>(it => it
                    .Cloneable(cloning => cloning
                        .Ignore(x => x.IgnoreMe)));
            config
                .Make<ObjWithPostProcessFunc>(it => it
                    .Cloneable(cloning => cloning
                        .PostProcess(ObjWithPostProcessFunc.PostProc)));
            config
                .Make<LambdaPostProcObj>(it => it
                    .Cloneable(cloning => cloning
                        .PostProcess((ref LambdaPostProcObj a, LambdaPostProcObj b) => a.Prop = b.Prop + " Post Processed")));
            config
                .Make<FuncActivatorObj>(it => it
                    .Cloneable(cloning => cloning
                        .Activator(FuncActivatorObj.Create)));
            config
                .Make<LambdaActivatorObj>(it => it
                    .Cloneable(cloning => cloning
                        .Activator(src => new LambdaActivatorObj($"Custom Make Inst + {src.Prop}"))));
            config.Make<ClassInSomeNamespace>(it => it
                .Cloneable());
        }
    }
}
