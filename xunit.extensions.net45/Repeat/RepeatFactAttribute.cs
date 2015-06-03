using System;
using Xunit.Sdk;

namespace Xunit
{
	/// <summary>RepeatFactAttribute may be applied to test case in order to run it multiple times (by default, 3 times).</summary>
	[XunitTestCaseDiscoverer("Xunit.Sdk.RepeatFactDiscoverer", "xunit.extensions2")]
	public class RepeatFactAttribute : FactAttribute
	{
		/// <summary>The number of times to run the test, will default to 3 times.</summary>
		public Int32 Count { get; set; }
	}
}
