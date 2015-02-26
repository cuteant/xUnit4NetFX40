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
	public class STATheoryDiscoverer : IXunitTestCaseDiscoverer
	{
		readonly TheoryDiscoverer theoryDiscoverer;

		public STATheoryDiscoverer(IMessageSink diagnosticMessageSink)
		{
			theoryDiscoverer = new TheoryDiscoverer(diagnosticMessageSink);
		}

		public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
		{
			return theoryDiscoverer.Discover(discoveryOptions, testMethod, factAttribute)
														 .Select(testCase => new STATestCase(testCase));
		}
	}
}
