using System;
using System.Linq;
using Xunit;
using Xunit.Extensions.AssertExtensions;

#if NET40
namespace AssertExtensions40
#else
namespace AssertExtensions45
#endif
{
	public class ExampleFacts
	{
		public class BooleanFacts
		{
			[Fact]
			public void ShouldBeTrue()
			{
				Boolean val = true;

				val.ShouldBeTrue();
			}

			[Fact]
			public void ShouldBeFalse()
			{
				Boolean val = false;

				val.ShouldBeFalse();
			}

			[Fact]
			public void ShouldBeTrueWithMessage()
			{
				Boolean val = false;

				Exception exception = Record.Exception(() => val.ShouldBeTrue("should be true"));

				Assert.Equal("should be true", exception.Message.Split(Environment.NewLine.ToArray())[0]);
			}

			[Fact]
			public void ShouldBeFalseWithMessage()
			{
				Boolean val = true;

				Exception exception = Record.Exception(() => val.ShouldBeFalse("should be false"));

				Assert.Equal("should be false", exception.Message.Split(Environment.NewLine.ToArray())[0]);
			}
		}
	}
}