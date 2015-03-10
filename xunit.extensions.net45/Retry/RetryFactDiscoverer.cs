using System;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	public class RetryFactDiscoverer : IXunitTestCaseDiscoverer
	{
		private readonly IMessageSink m_diagnosticMessageSink;

		public RetryFactDiscoverer(IMessageSink diagnosticMessageSink)
		{
			this.m_diagnosticMessageSink = diagnosticMessageSink;
		}

		public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
		{
			var maxRetries = factAttribute.GetNamedArgument<Int32>("MaxRetries");
			if (maxRetries < 1) { maxRetries = 3; }

			yield return new RetryTestCase(m_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, maxRetries);
		}
	}
}
