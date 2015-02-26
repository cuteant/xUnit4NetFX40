using System;
using System.Collections.Generic;
using System.Linq;
#if NET_4_0_ABOVE
using System.Reflection;
#endif
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	/// <summary>
	/// Reflection-based implementation of <see cref="IReflectionTypeInfo"/>.
	/// </summary>
	public class ReflectionTypeInfo : LongLivedMarshalByRefObject, IReflectionTypeInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReflectionTypeInfo"/> class.
		/// </summary>
		/// <param name="type">The type to wrap.</param>
		public ReflectionTypeInfo(Type type)
		{
			Type = type;
		}

		/// <inheritdoc/>
		public IAssemblyInfo Assembly
		{
			get { return Reflector.Wrap(Type.GetAssembly()); }
		}

		/// <inheritdoc/>
		public ITypeInfo BaseType
		{
#if NET_4_0_ABOVE
			get { return Reflector.Wrap(Type.GetTypeInfo().BaseType); }
#else
			get { return Reflector.Wrap(Type.BaseType); }
#endif
		}

		/// <inheritdoc/>
		public IEnumerable<ITypeInfo> Interfaces
		{
#if NET_4_0_ABOVE
			get { return Type.GetTypeInfo().ImplementedInterfaces.Select(Reflector.Wrap).Cast<ITypeInfo>().ToList(); }
#else
			get { return Type.GetInterfaces().Select(Reflector.Wrap).Cast<ITypeInfo>().ToList(); }
#endif
		}

		/// <inheritdoc/>
		public bool IsAbstract
		{
#if NET_4_0_ABOVE
			get { return Type.GetTypeInfo().IsAbstract; }
#else
			get { return Type.IsAbstract; }
#endif
		}

		/// <inheritdoc/>
		public bool IsGenericParameter
		{
			get { return Type.IsGenericParameter; }
		}

		/// <inheritdoc/>
		public bool IsGenericType
		{
#if NET_4_0_ABOVE
			get { return Type.GetTypeInfo().IsGenericType; }
#else
			get { return Type.IsGenericType; }
#endif
		}

		/// <inheritdoc/>
		public bool IsSealed
		{
#if NET_4_0_ABOVE
			get { return Type.GetTypeInfo().IsSealed; }
#else
			get { return Type.IsSealed; }
#endif
		}

		/// <inheritdoc/>
		public bool IsValueType
		{
#if NET_4_0_ABOVE
			get { return Type.GetTypeInfo().IsValueType; }
#else
			get { return Type.IsValueType; }
#endif
		}

		/// <inheritdoc/>
		public string Name
		{
			get { return Type.FullName ?? Type.Name; }
		}

		/// <inheritdoc/>
		public Type Type { get; private set; }

		/// <inheritdoc/>
		public IEnumerable<IAttributeInfo> GetCustomAttributes(string assemblyQualifiedAttributeTypeName)
		{
			return ReflectionAttributeInfo.GetCustomAttributes(Type, assemblyQualifiedAttributeTypeName).ToList();
		}

		/// <inheritdoc/>
		public IEnumerable<ITypeInfo> GetGenericArguments()
		{
#if NET_4_0_ABOVE
			return Type.GetTypeInfo().GenericTypeArguments.Select(Reflector.Wrap).ToList();
#else
			var typeArgs = Type.IsGenericType && !Type.IsGenericTypeDefinition
										? Type.GetGenericArguments()
										: Type.EmptyTypes;
			return typeArgs.Select(Reflector.Wrap).ToList();
#endif
		}

		/// <inheritdoc/>
		public IMethodInfo GetMethod(string methodName, bool includePrivateMethod)
		{
			var method = Type.GetRuntimeMethodsEx()
											 .FirstOrDefault(m => (includePrivateMethod || m.IsPublic && m.DeclaringType != typeof(object)) && m.Name == methodName);
			if (method == null)
				return null;

			return Reflector.Wrap(method);
		}

		/// <inheritdoc/>
		public IEnumerable<IMethodInfo> GetMethods(bool includePrivateMethods)
		{
			return Type.GetRuntimeMethodsEx().Where(m => includePrivateMethods || m.IsPublic)
								 .Select(Reflector.Wrap)
								 .Cast<IMethodInfo>()
								 .ToList();
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Type.ToString();
		}
	}
}