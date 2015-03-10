using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	public class RepeatFactDiscoverer : IXunitTestCaseDiscoverer
	{
		private readonly IMessageSink m_diagnosticMessageSink;

		public RepeatFactDiscoverer(IMessageSink diagnosticMessageSink)
		{
			this.m_diagnosticMessageSink = diagnosticMessageSink;
		}

		public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
		{
			var count = factAttribute.GetNamedArgument<Int32>("Count");
			if (count < 1) { count = 3; }

			yield return new RepeatTestCase(m_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, count);
		}
	}
}
