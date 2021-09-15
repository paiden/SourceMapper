using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapper.Samples
{
    // Input Objects
    public class A { public int Property { get; set; } }
    public class B {  public int Property { get; set; } }

    // Configuration
    internal class MappingSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config.Make<A>(it => it.MapTo<B>())
                .Make<B>(it => it.MapTo<A>());
        }
    }

    // Usage
    public static class MapToUsage
    {
        public static void Use()
        {
            var mapMe = new A();
            var b = mapMe.MapTo<B>();
        }
    }
}
