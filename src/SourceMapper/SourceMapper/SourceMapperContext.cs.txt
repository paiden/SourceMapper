namespace SourceMapper
{
    public class CloneBuilder<T>
    {
    }

    public class MapBuilder<F>
    {
        public void To<T>() { }
    }

    public class SourceMapperContext<T>
    {
    }

    public abstract class SourceMapperContext
    {
        public static SourceMapperContext<T> Cloneable<T>()
            => new();
    }
}
