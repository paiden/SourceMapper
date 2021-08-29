namespace SourceMapper.Tests
{
    internal class CloningContext : SourceMapperContext
    {
        static CloningContext()
        {
            Cloneable<TestClassDto>();
        }
    }
}
