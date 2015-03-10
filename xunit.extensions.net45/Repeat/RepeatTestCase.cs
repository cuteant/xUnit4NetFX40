using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	[Serializable]
	public class RepeatTestCase : XunitTestCase
	{
		private Int32 m_count;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Called by the de-serializer", true)]
		public RepeatTestCase() { }

		public RepeatTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay testMethodDisplay, ITestMethod testMethod, Int32 count)
			: base(diagnosticMessageSink, testMethodDisplay, testMethod, testMethodArguments: null)
		{
			m_count = count;
		}

		// This method is called by the xUnit test framework classes to run the test case. We will do the
		// loop here, forwarding on to the implementation in XunitTestCase to do the heavy lifting. We will
		// continue to re-run the test until the aggregator has an error (meaning that some internal error
		// condition happened), or the test runs without failure, or we've hit the maximum number of tries.
		public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
																										IMessageBus messageBus,
																										Object[] constructorArguments,
																										ExceptionAggregator aggregator,
																										CancellationTokenSource cancellationTokenSource)
		{
			var runCount = 0;

			while (true)
			{
				var summary = await base.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);
				if (aggregator.HasExceptions || summary.Failed != 0 || ++runCount >= m_count)
				{
					return summary;
				}
			}
		}

		public override void Serialize(IXunitSerializationInfo data)
		{
			base.Serialize(data);

			data.AddValue("Count", m_count);
		}

		public override void Deserialize(IXunitSerializationInfo data)
		{
			base.Deserialize(data);

			m_count = data.GetValue<Int32>("Count");
		}
	}
}
