using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

#if NET40
namespace TestOrderExamples40
#else
namespace TestOrderExamples45
#endif
{
	public class DisplayNameOrderer : ITestCollectionOrderer
	{
		public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
		{
			return testCollections.OrderBy(collection => collection.DisplayName);
		}
	}
}