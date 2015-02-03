using System;
using System.Threading;

namespace Xunit.Sdk
{
	internal class XunitWorkerThread
	{
		private readonly Thread thread;

		public XunitWorkerThread(Action threadProc)
		{
			thread = new Thread(() => threadProc());
			thread.Start();
		}

		public void Join()
		{
			thread.Join();
		}

		public static void QueueUserWorkItem(Action backgroundTask)
		{
			ThreadPool.QueueUserWorkItem(_ => backgroundTask());
		}
	}
}