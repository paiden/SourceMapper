using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapper.Tests.MappingTestObjects
{
    public class CombinePropSource
    {
        public string A { get; set; } = "A";

        public string B { get; set; } = "B";

        public string C { get; set; } = "C";
    }

    public class CombinePropTarget
    {
        public string AB { get; set; }

        public string C { get; set; }
    }
}
