using System;
using Xunit;
using Xunit.Abstractions;

#if NET40
namespace TestOutputExample40
#else
namespace TestOutputExample45
#endif
{
	public class Example
	{
		ITestOutputHelper output;

		public Example(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void TestThis()
		{
			output.WriteLine("I'm inside the test!");
		}
	}
}