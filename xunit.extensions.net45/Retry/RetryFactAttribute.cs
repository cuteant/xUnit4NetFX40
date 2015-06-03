using System;
using Xunit.Sdk;

namespace Xunit
{
	/// <summary>Works just like [Fact] except that failures are retried (by default, 3 times).</summary>
	[XunitTestCaseDiscoverer("Xunit.Sdk.RetryFactDiscoverer", "xunit.extensions2")]
	public class RetryFactAttribute : FactAttribute
	{
		/// <summary>Number of retries allowed for a failed test. If unset (or set less than 1), will default to 3 attempts.</summary>
		public Int32 MaxRetries { get; set; }
	}
}
