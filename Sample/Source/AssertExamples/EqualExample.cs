using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

#if NET40
namespace AssertExtensibility40
#else
namespace AssertExtensibility45
#endif
{
	public class EqualExample
	{
		[Fact]
		public void BoxedTypesDontWork()
		{
			int index = 0;

			Assert.Throws<SameException>(() => Assert.Same(index, index));
		}

		[Fact]
		public void ValuesAreNotTheSame()
		{
			Assert.Throws<SameException>(() => Assert.Same("bob", "jim"));
		}

		[Fact]
		public void ValuesAreTheSame()
		{
			const string jim = "jim";

			Assert.Same(jim, jim);
		}

		[Fact]
		public void IsType()
		{
			var expected = new InvalidCastException();
			Assert.IsType(typeof(InvalidCastException), expected);
			Assert.IsType<InvalidCastException>(expected);
		}

		[Fact]
		public void DoubleValueWithinRange()
		{
			Assert.InRange(1.0, .75, 1.25);
		}

		[Fact]
		public void StringValueWithinRange()
		{
			Assert.InRange("bob", "adam", "scott");
		}

		[Fact]
		public void EqualStringIgnoreCase()
		{
			string expected = "TestString";
			string actual = "teststring";

			Assert.False(actual == expected);
			Assert.NotEqual(expected, actual);
			Assert.Equal(expected, actual, StringComparer.CurrentCultureIgnoreCase);
		}

		[Fact]
		public void Array()
		{
			string[] expected = { "@", "a", "ab", "b" };
			string[] actual = { "@", "a", "ab", "b" };

			Assert.Equal(expected, actual);
			Assert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));

			Assert.NotEqual("a", "b");
		}

		[Fact]
		public void EqualsSByte()
		{
			const sbyte valueType = 35;
			const sbyte referenceValue = 35;

			Assert.True(valueType == referenceValue);
			Assert.Equal(referenceValue, valueType);
			Assert.Equal<sbyte>(valueType, 35);
			Assert.Equal<sbyte>(referenceValue, 35);
		}

		[Fact]
		public void EqualsStringIgnoreCase()
		{
			string expected = "TestString";
			string actual = "testString";

			Assert.False(actual == expected);
			Assert.NotEqual(expected, actual);
			Assert.Equal(expected, actual, StringComparer.CurrentCultureIgnoreCase);
		}

		[Fact]
		public void NullableValueTypesCanBeNull()
		{
			DateTime? dt1 = null;
			DateTime? dt2 = null;

			Assert.Equal(dt1, dt2);
		}

		[Fact]
		public void AssertEqualWithDecimalWithPrecision()
		{
			Assert.Equal(0.11111M, 0.11444M, 2);
		}

		class DateComparer : IEqualityComparer<DateTime>
		{
			public bool Equals(DateTime x, DateTime y)
			{
				return x.Date == y.Date;
			}

			public int GetHashCode(DateTime obj)
			{
				return obj.GetHashCode();
			}
		}

		[Fact]
		public void DateShouldBeEqualEvenThoughTimesAreDifferent()
		{
			DateTime firstTime = DateTime.Now.Date;
			DateTime later = firstTime.AddMinutes(90);

			Assert.NotEqual(firstTime, later);
			Assert.Equal(firstTime, later, new DateComparer());
		}
	}
}