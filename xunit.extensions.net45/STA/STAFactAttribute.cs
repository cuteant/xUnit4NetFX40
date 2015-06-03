using System;
using Xunit.Sdk;

namespace Xunit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[XunitTestCaseDiscoverer("Xunit.Sdk.STAFactDiscoverer", "xunit.extensions2")]
	public class STAFactAttribute : FactAttribute
	{
	}
}