using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

#if NET40
namespace STAExamples40
#else
namespace STAExamples45
#endif
{
	public class STAFactDiscoverer : IXunitTestCaseDiscoverer
	{
		readonly FactDiscoverer factDiscoverer;

		public STAFactDiscoverer(IMessageSink diagnosticMessageSink)
		{
			factDiscoverer = new FactDiscoverer(diagnosticMessageSink);
		}

		public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
		{
			return factDiscoverer.Discover(discoveryOptions, testMethod, factAttribute)
													 .Select(testCase => new STATestCase(testCase));
		}
	}
}
