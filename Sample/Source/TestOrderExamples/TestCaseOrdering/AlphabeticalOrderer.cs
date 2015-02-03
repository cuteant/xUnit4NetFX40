using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

#if NET40
namespace TestOrderExamples40
#else
namespace TestOrderExamples45
#endif
{
	public class AlphabeticalOrderer : ITestCaseOrderer
	{
		public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
			where TTestCase : ITestCase
		{
			var result = testCases.ToList();
			result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
			return result;
		}
	}
}