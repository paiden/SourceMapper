using System.Diagnostics;
using System.IO;

namespace SourceMapper.Utils
{
    internal static class DbgUtils
    {
        public static void LaunchDebugger(bool launch = true)
        {
            if (launch && File.Exists(@"C:\Temp\sgd"))
            {
                Debugger.Launch();
            }
        }
    }
}
