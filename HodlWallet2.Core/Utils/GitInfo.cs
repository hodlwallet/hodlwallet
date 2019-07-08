using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HodlWallet2.Core.Utils
{

    public static class GitInfo
    {
        static Assembly _Assembly => typeof(GitInfo).Assembly;
        //static IEnumerable<AssemblyMetadataAttribute> _Attrs => _Assembly.GetCustomAttributes<OrigHeadAttribute>();

        public static string GetOrigHead()
        {
            //AppDomain
            //var assembly = Assembly.Load("GitHead");
            //var avariable = _Attrs.FirstOrDefault(a => a.Key == "GitOrigHead")?.Value;
            //Assembly.LoadFile("");

            return "";
        }
    }
}
