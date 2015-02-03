using System.Threading;
using Xunit;

#if NET40
namespace TraitExtensibility40
#else
namespace TraitExtensibility45
#endif
{
	public class Example
	{
		[Fact, Category("Trait")]
		public void ExampleFact()
		{
			Assert.True(true);
		}
	}
}