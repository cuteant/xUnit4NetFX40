using System;
using System.Collections;
using System.Collections.Generic;
using Xunit.Sdk;

namespace Xunit
{
	public partial class Assert
	{
		private static IComparer<T> GetComparer<T>() where T : IComparable
		{
			return new AssertComparer<T>();
		}

		private static IEqualityComparer<T> GetEqualityComparer<T>(bool skipTypeCheck = false, IEqualityComparer innerComparer = null)
		{
			return new AssertEqualityComparer<T>(skipTypeCheck, innerComparer);
		}
	}
}