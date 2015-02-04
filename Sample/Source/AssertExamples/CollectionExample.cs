using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;
#if NET40
namespace AssertExtensibility40
#else
namespace AssertExtensibility45
#endif
{
	// Collection equivalence means the collections have the exact same values
	// in any order.

	public class CollectionExamples
	{
		[Fact]
		public void CanFindNullInContainer()
		{
			var list = new List<object> { 16, null, "Hi there" };

			Assert.Contains(null, list);
		}

		[Fact]
		public void ListInContainer()
		{
			var list = new List<object> { 1, 2, 3 };
			var list2 = new List<object> { 4, 5, list };
			Assert.Contains(list, list2);
		}

		[Fact]
		public void CanSearchForNullInContainer()
		{
			var list = new List<object> { 16, "Hi there" };
			Assert.DoesNotContain(null, list);
		}

		[Fact]
		public void CanSearchForSubstringsCaseInsensitive()
		{
			Assert.Throws<DoesNotContainException>(() => Assert.DoesNotContain("WORLD", "Hello, world!", StringComparison.InvariantCultureIgnoreCase));
		}

		[Fact]
		public void IsEmpty()
		{
			var list = new List<int>();
			Assert.Empty(list);
		}

		[Fact]
		public void NullIsNotEmpty()
		{
			Assert.Throws<ArgumentNullException>(() => Assert.Empty(null));
		}

		[Fact]
		public void SingleItemCollectionReturnsTheItem()
		{
			var collection = new ArrayList { "Hello" };
			var result = Assert.Single(collection);

			Assert.Equal("Hello", result);
		}

		[Fact]
		public void ObjectSingleMatch()
		{
			IEnumerable collection = new[] { "Hello", "World!" };

			Assert.Single(collection, "Hello");
		}

		[Fact]
		public void CollectionEquality()
		{
			List<int> left = new List<int>(new int[] { 4, 12, 16, 27 });
			List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

			Assert.Equal(left, right, new CollectionEquivalenceComparer<int>());
		}

		[Fact]
		public void LeftCollectionSmallerThanRight()
		{
			List<int> left = new List<int>(new int[] { 4, 12, 16 });
			List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

			Assert.NotEqual(left, right, new CollectionEquivalenceComparer<int>());
		}

		[Fact]
		public void LeftCollectionLargerThanRight()
		{
			List<int> left = new List<int>(new int[] { 4, 12, 16, 27, 42 });
			List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

			Assert.NotEqual(left, right, new CollectionEquivalenceComparer<int>());
		}

		[Fact]
		public void SameValuesOutOfOrder()
		{
			List<int> left = new List<int>(new int[] { 4, 16, 12, 27 });
			List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

			Assert.Equal(left, right, new CollectionEquivalenceComparer<int>());
		}

		[Fact]
		public void DuplicatedItemInOneListOnly()
		{
			List<int> left = new List<int>(new int[] { 4, 16, 12, 27, 4 });
			List<int> right = new List<int>(new int[] { 4, 12, 16, 27 });

			Assert.NotEqual(left, right, new CollectionEquivalenceComparer<int>());
		}

		[Fact]
		public void DuplicatedItemInBothLists()
		{
			List<int> left = new List<int>(new int[] { 4, 16, 12, 27, 4 });
			List<int> right = new List<int>(new int[] { 4, 12, 16, 4, 27 });

			Assert.Equal(left, right, new CollectionEquivalenceComparer<int>());
		}
	}

	class CollectionEquivalenceComparer<T> : IEqualityComparer<IEnumerable<T>>
		 where T : IEquatable<T>
	{
		public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
		{
			List<T> leftList = new List<T>(x);
			List<T> rightList = new List<T>(y);
			leftList.Sort();
			rightList.Sort();

			IEnumerator<T> enumeratorX = leftList.GetEnumerator();
			IEnumerator<T> enumeratorY = rightList.GetEnumerator();

			while (true)
			{
				bool hasNextX = enumeratorX.MoveNext();
				bool hasNextY = enumeratorY.MoveNext();

				if (!hasNextX || !hasNextY)
					return (hasNextX == hasNextY);

				if (!enumeratorX.Current.Equals(enumeratorY.Current))
					return false;
			}
		}

		public int GetHashCode(IEnumerable<T> obj)
		{
			throw new NotImplementedException();
		}
	}
}