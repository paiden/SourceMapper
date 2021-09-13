using System;

#pragma warning disable IDE0060 // Remove unused parameter

namespace SourceMapper
{
    internal delegate void PostProcessDelegate<TTarget, in TSource>(ref TTarget target, TSource source);

    internal interface IMapConfig<T, TTarget, Self>
    {
        Self Ignore<P>(Func<T, P> propSelector);
        Self PostProcess(PostProcessDelegate<TTarget, T> postProcess);
    }

    internal sealed class MapToConfig<T, Target> : IMapConfig<T, Target, MapToConfig<T, Target>>
    {
        public MapToConfig<T, Target> Ignore<P>(Func<T, P> propertySelector) { return this; }

        public MapToConfig<T, Target> PostProcess(PostProcessDelegate<Target, T> postProcess) { return this; }
    }

    internal sealed class CloneConfig<T> : IMapConfig<T, T, CloneConfig<T>>
    {
        public CloneConfig<T> Ignore<P>(Func<T, P> propertySelector) { return this; }

        public CloneConfig<T> PostProcess(PostProcessDelegate<T, T> postProcess) { return this; }
    }

    internal sealed class MakeConfig<T>
    {
        public MakeConfig<T> Cloneable(Action<CloneConfig<T>>? config = null) { return this; }

        public MakeConfig<T> MapTo<Target>(Action<MapToConfig<T, Target>>? config = null) { return this; }
    }

    internal sealed class ContextConfig
    {
        private ContextConfig()
        {
        }

        public ContextConfig Make<T>(Action<MakeConfig<T>> it) { return this; }

        public void Foo() { }
    }

    internal abstract class SourceMapperContext
    {
        internal const string ConfigureName = nameof(Configure);

        protected abstract void Configure(ContextConfig config);
    }
}
