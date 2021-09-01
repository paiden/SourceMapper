namespace SourceMapper
{
    public sealed class MapBuilder
    {
        private MapBuilder()
        {
        }

        public MapBuilder MakeCloneable<T>() { return this; }
    }

    public abstract class SourceMapperContext
    {
        internal const string ConfigureName = nameof(Configure);

        protected abstract void Configure(MapBuilder builder);
    }
}
