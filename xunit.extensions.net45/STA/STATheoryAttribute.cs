using System;
using Xunit.Sdk;

namespace Xunit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[XunitTestCaseDiscoverer("Xunit.Sdk.STATheoryDiscoverer", "xunit.extensions2")]
	public class STATheoryAttribute : TheoryAttribute
	{
	}
}