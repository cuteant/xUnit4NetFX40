using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nito.AsyncEx.Internal.PlatformEnlightenment
{
	/// <summary>This class taken from Stephen Cleary's AsyncEx Library(https://github.com/StephenCleary/AsyncEx)</summary>
	public static class ReflectionHelper
	{
		/// <summary>Type</summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static Type Type(String typeName)
		{
			try
			{
				return System.Type.GetType(typeName, false);
			}
			catch (ArgumentException)
			{
			}
			catch (TargetInvocationException)
			{
			}
			catch (TypeLoadException)
			{
			}
			catch (IOException)
			{
			}
			catch (BadImageFormatException)
			{
			}
			return null;
		}

		/// <summary>Compile</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="body"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static T Compile<T>(Expression body, params ParameterExpression[] parameters) where T : class
		{
			if (body == null || parameters.Any(x => x == null))
				return null;
			try
			{
				return Expression.Lambda<T>(body, parameters).Compile();
			}
			catch (ArgumentException)
			{
			}
			return null;
		}

		/// <summary>Call</summary>
		/// <param name="type"></param>
		/// <param name="methodName"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static MethodCallExpression Call(Type type, string methodName, params Expression[] arguments)
		{
			if (type == null || arguments.Any(x => x == null))
				return null;
			try
			{
				return Expression.Call(type, methodName, null, arguments);
			}
			catch (InvalidOperationException)
			{
			}
			return null;
		}

		/// <summary>Call</summary>
		/// <param name="instance"></param>
		/// <param name="methodName"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static MethodCallExpression Call(Expression instance, string methodName, params Expression[] arguments)
		{
			if (instance == null || arguments.Any(x => x == null))
				return null;
			try
			{
				return Expression.Call(instance, methodName, null, arguments);
			}
			catch (InvalidOperationException)
			{
			}
			return null;
		}

		/// <summary>Call</summary>
		/// <param name="instance"></param>
		/// <param name="methodName"></param>
		/// <param name="flags"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static MethodCallExpression Call(Expression instance, string methodName, BindingFlags flags, params Expression[] arguments)
		{
			if (instance == null || arguments.Any(x => x == null))
				return null;
			MethodInfo method;
			try
			{
				method = instance.Type.GetMethod(methodName, flags);
			}
			catch (AmbiguousMatchException)
			{
				return null;
			}
			try
			{
				return Expression.Call(instance, method, arguments);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}
	}
}
