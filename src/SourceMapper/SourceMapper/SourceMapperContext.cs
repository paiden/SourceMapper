using System;

#pragma warning disable IDE0060 // Remove unused parameter

namespace SourceMapper
{
    public sealed class CloneConfig<T>
    {
        public CloneConfig<T> Ignore<P>(Func<T, P> propertySelector) { return this; }
    }

    public sealed class MakeConfig<T>
    {
        public MakeConfig<T> Cloneable(Action<CloneConfig<T>>? config = null) { return this; }
    }


    public sealed class MappingConfig
    {
        private MappingConfig()
        {
        }

        public MappingConfig Make<T>(Action<MakeConfig<T>> it) { return this; }
    }

    public abstract class SourceMapperContext
    {
        internal const string ConfigureName = nameof(Configure);

        protected abstract void Configure(MappingConfig config);
    }
}
