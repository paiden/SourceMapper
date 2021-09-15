using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapper.Samples
{
    // Input objects
    public record RecA(int X)
    {
        public string Prop { get; set; }
    }

    public record RecB(int Y)
    {
        public static RecB MapActivator(RecA src)
        {
            return new RecB(src.X + 1);
        }

        public string Prop { get; set; }
    }

    // Configuration
    internal class ActivatorSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<RecA>(it => it
                    .Cloneable(cloning => cloning
                        .Activator(src => new RecA(123)))
                    .MapTo<RecB>(mapping => mapping
                        .Activator(RecB.MapActivator)));
        }
    }
}
