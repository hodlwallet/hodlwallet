using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HodlWallet2.Core.Utils
{
	public static class BuildInfo
	{
		static Assembly _Assembly { get; } = Assembly.GetExecutingAssembly();
		static IEnumerable<CustomAttributeData> _CustomAttributes { get; } = _Assembly?.CustomAttributes ?? Enumerable.Empty<CustomAttributeData>();

		public static string GitHead { get => GetValue("GitHead"); }
		public static string GitBranch { get => GetValue("GitBranch"); }
		public static string BuildDateText { get => GetValue("BuildDateTime"); }

		static string GetValue(string key)
		{
			return _CustomAttributes
				// [assembly: AssemblyMetadata("GitHead", "COMMIT_HASH")]
				.Where(c => c.ConstructorArguments.Count >= 2 && c.ConstructorArguments[0].Value as string == key)
				.Select(c => c.ConstructorArguments[1].Value as string)
				.FirstOrDefault() ?? "??";
		}
	}
}
