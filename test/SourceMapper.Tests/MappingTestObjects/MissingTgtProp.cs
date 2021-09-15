using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapper.Tests.MappingTestObjects
{
    internal class MissingTgtPropTarget
    {
        public string X { get; set; }
    }

    internal class MissingTgtPropSource
    {
        public string X { get; set; } = "X-Source";

        public string Y { get; set; } = "Y-Source";
    }
}
