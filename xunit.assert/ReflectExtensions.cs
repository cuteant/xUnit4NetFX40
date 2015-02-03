using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if NET_4_0_ABOVE

using System.Runtime.CompilerServices;

#endif

namespace Xunit
{
	internal static class ReflectExtensions
	{
		internal const BindingFlags DefaultLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		internal const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

		/// <summary>确定 Type 的实例是否可以从指定 Type 的实例分配。</summary>
		/// <param name="type"></param>
		/// <param name="c"></param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static Boolean IsAssignableFromEx(this Type type, Type c)
		{
#if NET_4_0_ABOVE
			return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
#else
			return type.IsAssignableFrom(c);
#endif
		}

		/// <summary>获取属性，统一 Type.GetProperty、Type.GetRuntimeProperty、TypeInfo.GetDeclaredProperty方法的调用</summary>
		/// <param name="type">类型</param>
		/// <param name="name">名称</param>
		/// <param name="isDeclaredOnly">表示仅搜索在 Type 上声明的方法，而不搜索简单继承的方法</param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static PropertyInfo GetPropertyEx(this Type type, String name, Boolean isDeclaredOnly = true)
		{
			if (String.IsNullOrWhiteSpace(name)) { return null; }

			// 父类属性的获取需要递归，有些类型的父类为空，比如接口
			while (type != null && type != typeof(Object))
			{
				if (isDeclaredOnly)
				{
#if NET_4_0_ABOVE
					var pi = type.GetTypeInfo().GetDeclaredProperty(name);
#else
					var pi = GetSafeProperty(type, name, DeclaredOnlyLookup);
#endif
					if (pi != null) { return pi; }
				}
				else
				{
					var pi = GetSafeProperty(type, name, DefaultLookup);
					if (pi != null) { return pi; }
				}

				type = type.BaseType;
			}
			return null;
		}

		/// <summary>获取字段，统一 Type.GetField、Type.GetRuntimeField、TypeInfo.GetDeclaredField方法的调用</summary>
		/// <param name="type">类型</param>
		/// <param name="name">名称</param>
		/// <param name="isDeclaredOnly">表示仅搜索在 Type 上声明的方法，而不搜索简单继承的方法</param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static FieldInfo GetFieldEx(this Type type, String name, Boolean isDeclaredOnly = true)
		{
			if (String.IsNullOrWhiteSpace(name)) { return null; }

			// 父类字段的获取需要递归，有些类型的父类为空，比如接口
			while (type != null && type != typeof(Object))
			{
				if (isDeclaredOnly)
				{
#if NET_4_0_ABOVE
					var fi = type.GetTypeInfo().GetDeclaredField(name);
#else
					var fi = GetSafeField(type, name, DeclaredOnlyLookup);
#endif
					if (fi != null) { return fi; }
				}
				else
				{
					var fi = GetSafeField(type, name, DefaultLookup);
					if (fi != null) { return fi; }
				}

				type = type.BaseType;
			}
			return null;
		}

		/// <summary>获取事件，统一 Type.GetEvent、Type.GetRuntimeEvent、TypeInfo.GetDeclaredEvent方法的调用</summary>
		/// <param name="type">类型</param>
		/// <param name="name">名称</param>
		/// <param name="isDeclaredOnly">表示仅搜索在 Type 上声明的方法，而不搜索简单继承的方法</param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static EventInfo GetEventEx(this Type type, String name, Boolean isDeclaredOnly = true)
		{
			if (String.IsNullOrWhiteSpace(name)) { return null; }

			// 父类字段的获取需要递归，有些类型的父类为空，比如接口
			while (type != null && type != typeof(Object))
			{
				if (isDeclaredOnly)
				{
#if NET_4_0_ABOVE
					var ei = type.GetTypeInfo().GetDeclaredEvent(name);
#else
					var ei = GetSafeEvent(type, name, DeclaredOnlyLookup);
#endif
					if (ei != null) { return ei; }
				}
				else
				{
					var ei = GetSafeEvent(type, name, DefaultLookup);
					if (ei != null) { return ei; }
				}

				type = type.BaseType;
			}
			return null;
		}

		/// <summary>获取指定类型上定义的所有字段的集合。</summary>
		/// <param name="type"></param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IEnumerable<FieldInfo> GetRuntimeFieldsEx(this Type type)
		{
#if NET_4_0_ABOVE
			return type.GetRuntimeFields();
#else
			return type.GetFields(DefaultLookup);
#endif
		}

		/// <summary>获取指定类型上定义的所有属性的集合。</summary>
		/// <param name="type"></param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IEnumerable<PropertyInfo> GetRuntimePropertiesEx(this Type type)
		{
#if NET_4_0_ABOVE
			return type.GetRuntimeProperties();
#else
			return type.GetProperties(DefaultLookup);
#endif
		}

		/// <summary>获取指定类型上定义的所有方法的集合。</summary>
		/// <param name="type"></param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IEnumerable<MethodInfo> GetRuntimeMethodsEx(this Type type)
		{
#if NET_4_0_ABOVE
			return type.GetRuntimeMethods();
#else
			return type.GetMethods(DefaultLookup);
#endif
		}

		/// <summary>获取指定类型声明的构造函数的集合</summary>
		/// <param name="type"></param>
		/// <param name="isDeclaredOnly"></param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IEnumerable<ConstructorInfo> GetConstructorsEx(this Type type, Boolean isDeclaredOnly = true)
		{
			if (isDeclaredOnly)
			{
#if NET_4_0_ABOVE
				return type.GetTypeInfo().DeclaredConstructors;
#else
				return type.GetConstructors(DeclaredOnlyLookup);
#endif
			}
			else
			{
				return type.GetConstructors(DefaultLookup);
			}
		}

		/// <summary>获取指定类型定义的方法的集合</summary>
		/// <param name="type"></param>
		/// <param name="isDeclaredOnly"></param>
		/// <returns></returns>
#if NET_4_0_ABOVE

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static IEnumerable<MethodInfo> GetMethodsEx(this Type type, Boolean isDeclaredOnly = true)
		{
			if (isDeclaredOnly)
			{
#if NET_4_0_ABOVE
				return type.GetTypeInfo().DeclaredMethods;
#else
				return type.GetMethods(DeclaredOnlyLookup);
#endif
			}
			else
			{
				return type.GetMethods(DefaultLookup);
			}
		}

		private static FieldInfo GetSafeField(Type type, String propertyName, BindingFlags flags)
		{
			try
			{
				return type.GetField(propertyName, flags);
			}
			catch (AmbiguousMatchException)
			{
				return type.GetFields(flags).First(pi => pi.Name == propertyName);
			}
		}

		private static PropertyInfo GetSafeProperty(Type type, String propertyName, BindingFlags flags)
		{
			try
			{
				return type.GetProperty(propertyName, flags);
			}
			catch (AmbiguousMatchException)
			{
				return type.GetProperties(flags).First(pi => pi.Name == propertyName);
			}
		}

		private static EventInfo GetSafeEvent(Type type, String eventName, BindingFlags flags)
		{
			try
			{
				return type.GetEvent(eventName, flags);
			}
			catch (AmbiguousMatchException)
			{
				return type.GetEvents(flags).First(ei => ei.Name == eventName);
			}
		}
	}
}