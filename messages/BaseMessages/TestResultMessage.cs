﻿using System;
using Xunit.Abstractions;

#if XUNIT_CORE_DLL

namespace Xunit.Sdk
#else
namespace Xunit
#endif
{
	/// <summary>
	/// Default implementation of <see cref="ITestResultMessage"/>.
	/// </summary>
	public class TestResultMessage : TestMessage, ITestResultMessage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TestResultMessage"/> class.
		/// </summary>
		public TestResultMessage(ITest test, decimal executionTime, string output)
			: base(test)
		{
			ExecutionTime = executionTime;
			Output = output ?? String.Empty;
		}

		/// <inheritdoc/>
		public decimal ExecutionTime { get; private set; }

		/// <inheritdoc/>
		public string Output { get; private set; }
	}
}