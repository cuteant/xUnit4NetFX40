using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xunit.Sdk
{
	/// <summary>
	/// Default implementation of <see cref="IComparer{T}"/> used by the xUnit.net range assertions.
	/// </summary>
	/// <typeparam name="T">The type that is being compared.</typeparam>
	internal class AssertComparer<T> : IComparer<T> where T : IComparable
	{
#if NET_4_0_ABOVE
		static readonly TypeInfo NullableTypeInfo = typeof(Nullable<>).GetTypeInfo();
#else
		static readonly Type NullableTypeInfo = typeof(Nullable<>);
#endif

		/// <inheritdoc/>
		public int Compare(T x, T y)
		{
#if NET_4_0_ABOVE
			var typeInfo = typeof(T).GetTypeInfo();

			// Null?
			if (!typeInfo.IsValueType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().GetTypeInfo().IsAssignableFrom(NullableTypeInfo)))
#else
			var typeInfo = typeof(T);

			// Null?
			if (!typeInfo.IsValueType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().IsAssignableFrom(NullableTypeInfo)))
#endif
			{
				if (Equals(x, default(T)))
				{
					if (Equals(y, default(T)))
						return 0;
					return -1;
				}

				if (Equals(y, default(T)))
					return -1;
			}

			// Same type?
			if (x.GetType() != y.GetType())
				return -1;

			// Implements IComparable<T>?
			var comparable1 = x as IComparable<T>;
			if (comparable1 != null)
				return comparable1.CompareTo(y);

			// Implements IComparable
			return x.CompareTo(y);
		}
	}
}