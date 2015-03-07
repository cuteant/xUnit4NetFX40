using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit
{
	public class CollectionDisplayNameOrderer : ITestCollectionOrderer
	{
		public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
		{
			return testCollections.OrderBy(collection => collection.DisplayName);
		}
	}
}
