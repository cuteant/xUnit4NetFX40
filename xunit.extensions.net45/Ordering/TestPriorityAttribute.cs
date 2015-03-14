using System;

namespace Xunit
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TestPriorityAttribute : Attribute
	{
		public TestPriorityAttribute(Int32 priority)
		{
			Priority = priority;
		}

		public Int32 Priority { get; private set; }
	}
}