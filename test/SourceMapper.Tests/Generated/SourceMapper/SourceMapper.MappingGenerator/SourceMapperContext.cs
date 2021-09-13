using System;

#pragma warning disable IDE0060 // Remove unused parameter

namespace SourceMapper
{
    internal delegate void PostProcessDelegate<TTarget, in TSource>(ref TTarget target, TSource source);

    internal interface IMapConfig<TSource, TTarget, Self>
    {
        Self Activator(Func<TSource, TTarget> activator);
        Self Ignore<P>(Func<TSource, P> propSelector);
        Self PostProcess(PostProcessDelegate<TTarget, TSource> postProcess);
    }

    internal sealed class MapToConfig<TSource, TTarget> : IMapConfig<TSource, TTarget, MapToConfig<TSource, TTarget>>
    {
        public MapToConfig<TSource, TTarget> Activator(Func<TSource, TTarget> activator) { return this; }
        public MapToConfig<TSource, TTarget> Ignore<P>(Func<TSource, P> propertySelector) { return this; }
        public MapToConfig<TSource, TTarget> PostProcess(PostProcessDelegate<TTarget, TSource> postProcess) { return this; }
    }

    internal sealed class CloneConfig<T> : IMapConfig<T, T, CloneConfig<T>>
    {
        public CloneConfig<T> Activator(Func<T, T> activator) { return this; }
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
