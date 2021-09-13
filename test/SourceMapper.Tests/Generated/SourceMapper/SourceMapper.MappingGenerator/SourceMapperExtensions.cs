namespace SourceMapper.Tests
{
    public static class IgnorePropDtoCloningContextSourceMapperExtensions
    {
        public static IgnorePropDto Clone(this IgnorePropDto source)
        {
            var obj = new IgnorePropDto();

            obj.CloneMe = source.CloneMe;

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class VisiblityDtoCloningContextSourceMapperExtensions
    {
        public static VisiblityDto Clone(this VisiblityDto source)
        {
            var obj = new VisiblityDto();

            obj.PublicString = source.PublicString;
            obj.InternalString = source.InternalString;

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class WithCloneablePropertyDtoCloningContextSourceMapperExtensions
    {
        public static WithCloneablePropertyDto Clone(this WithCloneablePropertyDto source)
        {
            var obj = new WithCloneablePropertyDto(source.InnerGetOnly.Clone());

            obj.Prop = source.Prop;
            obj.Inner = source.Inner.Clone();

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class ObjWithPostProcessFuncCloningContextSourceMapperExtensions
    {
        public static ObjWithPostProcessFunc Clone(this ObjWithPostProcessFunc source)
        {
            var obj = new ObjWithPostProcessFunc();

            obj.Prop = source.Prop;

            ObjWithPostProcessFunc.PostProc(ref obj, source);

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class LambdaPostProcObjCloningContextSourceMapperExtensions
    {
        public static LambdaPostProcObj Clone(this LambdaPostProcObj source)
        {
            var obj = new LambdaPostProcObj();

            obj.Prop = source.Prop;

            var postProcess = (ref LambdaPostProcObj a, LambdaPostProcObj b) => a.Prop = b.Prop + " Post Processed";
            postProcess(ref obj, source);

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class FuncActivatorObjCloningContextSourceMapperExtensions
    {
        public static FuncActivatorObj Clone(this FuncActivatorObj source)
        {
            var obj = FuncActivatorObj.Create(source);

            obj.ShouldBeCloned = source.ShouldBeCloned;

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class LambdaActivatorObjCloningContextSourceMapperExtensions
    {
        public static LambdaActivatorObj Clone(this LambdaActivatorObj source)
        {
            Func<LambdaActivatorObj, LambdaActivatorObj> instanceCreator = src => new LambdaActivatorObj($"Custom Make Inst + {src.Prop}");
            var obj = instanceCreator(source);

            obj.ShouldBeCloned = source.ShouldBeCloned;

            return obj;
        }
    }
}

namespace SomeCustomNamespace
{
    public static class ClassInSomeNamespaceCloningContextSourceMapperExtensions
    {
        public static ClassInSomeNamespace Clone(this ClassInSomeNamespace source)
        {
            var obj = new ClassInSomeNamespace();

            obj.Prop = source.Prop;

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class TestClassMappingContextSourceMapperExtensions
    {
        public static TestClassDto MapTo<T>(this TestClass source) where T : TestClassDto
        {
            var obj = new TestClassDto();

            obj.F = source.F;
            obj.D = source.D;
            obj.S = source.S;

            return obj;
        }
    }
}

namespace SourceMapper.Tests
{
    public static class LambdaActivatorObjMappingContextSourceMapperExtensions
    {
        public static FuncActivatorObj MapTo<T>(this LambdaActivatorObj source) where T : FuncActivatorObj
        {
            Func<LambdaActivatorObj, FuncActivatorObj> instanceCreator = x => new FuncActivatorObj($"Custom Activator Lambda + {x.Prop}");
            var obj = instanceCreator(source);

            obj.ShouldBeCloned = source.ShouldBeCloned;

            return obj;
        }
    }
}

