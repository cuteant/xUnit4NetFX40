using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	/// <summary>
	/// This is an internal class, and is not intended to be called from end-user code.
	/// </summary>
	public class MessageBus : IMessageBus
	{
		private volatile bool continueRunning = true;
		private readonly IMessageSink messageSink;
		private readonly ConcurrentQueue<IMessageSinkMessage> reporterQueue = new ConcurrentQueue<IMessageSinkMessage>();
		private readonly XunitWorkerThread reporterThread;
		private readonly AutoResetEvent reporterWorkEvent = new AutoResetEvent(initialState: false);
		private volatile bool shutdownRequested;

		/// <summary/>
		public MessageBus(IMessageSink messageSink)
		{
			this.messageSink = messageSink;

			reporterThread = new XunitWorkerThread(ReporterWorker);
		}

		private void DispatchMessages()
		{
			IMessageSinkMessage message;
			while (reporterQueue.TryDequeue(out message))
				try
				{
					if (!messageSink.OnMessage(message))
						continueRunning = false;
				}
				catch (Exception ex)
				{
					try
					{
						var errorMessage = new ErrorMessage(Enumerable.Empty<ITestCase>(), ex);
						if (!messageSink.OnMessage(errorMessage))
							continueRunning = false;
					}
					catch { }
				}
		}

		/// <summary/>
		public void Dispose()
		{
			shutdownRequested = true;

			reporterWorkEvent.Set();
			reporterThread.Join();
			reporterWorkEvent.Dispose();
		}

		/// <summary/>
		public bool QueueMessage(IMessageSinkMessage message)
		{
			if (shutdownRequested)
				throw new ObjectDisposedException("MessageBus");

			reporterQueue.Enqueue(message);
			reporterWorkEvent.Set();
			return continueRunning;
		}

		private void ReporterWorker()
		{
			while (!shutdownRequested)
			{
				reporterWorkEvent.WaitOne();
				DispatchMessages();
			}

			// One final dispatch pass
			DispatchMessages();
		}
	}
}