using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapper.Samples
{
    // Input Objects
    public class PostProcessSource
    {
        public string A { get; set; }
        public string B { get; set; }
    }

    public class PostProcessTarget { public string AB { get; set; } }

    // Configuration
    internal class PostProcessSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<PostProcessSource>(it => it
                    .MapTo<PostProcessTarget>(mapping => mapping
                        .Ignore(src => src.A)
                        .Ignore(src => src.B)
                        .PostProcess((ref PostProcessTarget tgt, PostProcessSource src) => tgt.AB = src.A + src.B)));
        }
    }
}
