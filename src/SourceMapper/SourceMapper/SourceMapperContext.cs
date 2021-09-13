using System;

#pragma warning disable IDE0060 // Remove unused parameter

namespace SourceMapper
{
    interface IMapConfig<T, Target, Self>
    {
        Self Ignore<P>(Func<T, P> propSelector);
    }


    public sealed class MapToConfig<T, Target> : IMapConfig<T, Target, MapToConfig<T, Target>>
    {
        public MapToConfig<T, Target> Ignore<P>(Func<T, P> propertySelector) { return this; }
    }

    public sealed class CloneConfig<T> : IMapConfig<T, T, CloneConfig<T>>
    {
        public CloneConfig<T> Ignore<P>(Func<T, P> propertySelector) { return this; }
    }

    public sealed class MakeConfig<T>
    {
        public MakeConfig<T> Cloneable(Action<CloneConfig<T>>? config = null) { return this; }

        public MakeConfig<T> MapTo<Target>(Action<MapToConfig<T, Target>>? config = null) { return this; }
    }

    public sealed class ContextConfig
    {
        private ContextConfig()
        {
        }

        public ContextConfig Make<T>(Action<MakeConfig<T>> it) { return this; }

        public void Foo() { }
    }

    public abstract class SourceMapperContext
    {
        internal const string ConfigureName = nameof(Configure);

        protected abstract void Configure(ContextConfig config);
    }
}
