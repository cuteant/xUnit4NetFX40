using System.Threading.Tasks;

namespace Xunit.Sdk
{
	static class CommonTasks
	{
#if NET_4_0_ABOVE
		internal static readonly Task Completed = Task.FromResult(0);
#else
		internal static readonly Task Completed = TaskEx.FromResult(0);
#endif
	}
}
