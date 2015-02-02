﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xunit.Sdk
{
	/// <summary>
	/// An implementation of <see cref="TestCaseRunner{TTestCase}"/> to support <see cref="LambdaTestCase"/>.
	/// </summary>
	public class LambdaTestCaseRunner : TestCaseRunner<LambdaTestCase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LambdaTestCaseRunner"/> class.
		/// </summary>
		/// <param name="testCase">The test case that the lambda represents.</param>
		/// <param name="messageBus">The message bus to report run status to.</param>
		/// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
		/// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
		public LambdaTestCaseRunner(LambdaTestCase testCase,
																IMessageBus messageBus,
																ExceptionAggregator aggregator,
																CancellationTokenSource cancellationTokenSource)
			: base(testCase, messageBus, aggregator, cancellationTokenSource) { }

		/// <inheritdoc/>
		protected override Task<RunSummary> RunTestAsync()
		{
			var test = new XunitTest(TestCase, TestCase.DisplayName);
			var summary = new RunSummary { Total = 1 };
			var timer = new ExecutionTimer();

			if (!MessageBus.QueueMessage(new TestStarting(test)))
				CancellationTokenSource.Cancel();
			else
			{
				try
				{
					timer.Aggregate(TestCase.Lambda);

					if (!MessageBus.QueueMessage(new TestPassed(test, timer.Total, null)))
						CancellationTokenSource.Cancel();
				}
				catch (Exception ex)
				{
					summary.Failed++;

					if (!MessageBus.QueueMessage(new TestFailed(test, timer.Total, null, ex)))
						CancellationTokenSource.Cancel();
				}
			}

			if (!MessageBus.QueueMessage(new TestFinished(test, timer.Total, null)))
				CancellationTokenSource.Cancel();

			summary.Time = timer.Total;
#if NET_4_0_ABOVE
			return Task.FromResult(summary);
#else
			return TaskEx.FromResult(summary);
#endif
		}
	}
}
