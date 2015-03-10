using System.Collections.Generic;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	/// <summary>Used to capture messages to potentially be forwarded later. Messages are forwarded by disposing of the message bus.</summary>
	public class DelayedMessageBus : IMessageBus
	{
		private readonly IMessageBus m_innerBus;
		private readonly List<IMessageSinkMessage> m_messages = new List<IMessageSinkMessage>();

		public DelayedMessageBus(IMessageBus innerBus)
		{
			this.m_innerBus = innerBus;
		}

		public bool QueueMessage(IMessageSinkMessage message)
		{
			lock (m_messages)
			{
				m_messages.Add(message);
			}

			// No way to ask the inner bus if they want to cancel without sending them the message, so
			// we just go ahead and continue always.
			return true;
		}

		public void Dispose()
		{
			foreach (var message in m_messages)
			{
				m_innerBus.QueueMessage(message);
			}
		}
	}
}
