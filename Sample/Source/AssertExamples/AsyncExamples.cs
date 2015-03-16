using System;
using System.Threading.Tasks;
using Xunit;

#if NET40
namespace AssertExtensibility40
#else
namespace AssertExtensibility45
#endif
{
	public class AsyncExamples
	{
		[Fact]
		public async Task CodeThrowsAsync()
		{
			Func<Task> testCode = () => Task.Factory.StartNew(ThrowingMethod);

			var ex = await Assert.ThrowsAsync<NotImplementedException>(testCode);

			Assert.IsType<NotImplementedException>(ex);
		}

		[Fact]
		public async Task RecordAsync()
		{
			Func<Task> testCode = () => Task.Factory.StartNew(ThrowingMethod);

			var ex = await Record.ExceptionAsync(testCode);

			Assert.IsType<NotImplementedException>(ex);
		}

		void ThrowingMethod()
		{
			throw new NotImplementedException();
		}
	}
}