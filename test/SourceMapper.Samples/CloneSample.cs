using SourceMapper;

namespace Samples
{
    // Input objects
    public class DataTransferObject
    {
        public string Property { get; set; }

        public InnerDataTranferObject Inner { get; set; } = new InnerDataTranferObject();
    }

    public class InnerDataTranferObject
    {
        public int Property { get; set; }
    }

    // Configuration
    internal class CloneSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<DataTransferObject>(it => it.Cloneable())
                .Make<InnerDataTranferObject>(it => it.Cloneable());
        }
    }

    // Usage
    public class CloneUsage
    {
        public static void Use()
        {
            var cloneable = new DataTransferObject();
            var cloned = cloneable.Clone();
        }
    }
}
