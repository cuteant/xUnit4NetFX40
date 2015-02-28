using System;
using Xunit.Sdk;

namespace Xunit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[XunitTestCaseDiscoverer("Xunit.Sdk.STAFactDiscoverer", "xunit.extensions")]
	public class STAFactAttribute : FactAttribute
	{
	}
}