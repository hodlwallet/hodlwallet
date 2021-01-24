//
// BuildInfo.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HodlWallet.Core.Utils
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
