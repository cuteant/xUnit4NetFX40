﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xunit.Sdk
{
	/// <summary>
	/// Formats arguments for display in theories.
	/// </summary>
	internal static class ArgumentFormatter
	{
		private const int MAX_DEPTH = 3;
		private const int MAX_ENUMERABLE_LENGTH = 5;
		private const int MAX_OBJECT_PARAMETER_COUNT = 5;
		private const int MAX_STRING_LENGTH = 50;

		private static readonly object[] EmptyObjects = new object[0];
		private static readonly Type[] EmptyTypes = new Type[0];

#if NET_4_0_ABOVE
		// List of system types => C# type names
		private static readonly Dictionary<TypeInfo, string> TypeMappings = new Dictionary<TypeInfo, string>
				{
						{ typeof(bool).GetTypeInfo(), "bool" },
						{ typeof(byte).GetTypeInfo(), "byte" },
						{ typeof(sbyte).GetTypeInfo(), "sbyte" },
						{ typeof(char).GetTypeInfo(), "char" },
						{ typeof(decimal).GetTypeInfo(), "decimal" },
						{ typeof(double).GetTypeInfo(), "double" },
						{ typeof(float).GetTypeInfo(), "float" },
						{ typeof(int).GetTypeInfo(), "int" },
						{ typeof(uint).GetTypeInfo(), "uint" },
						{ typeof(long).GetTypeInfo(), "long" },
						{ typeof(ulong).GetTypeInfo(), "ulong" },
						{ typeof(object).GetTypeInfo(), "object" },
						{ typeof(short).GetTypeInfo(), "short" },
						{ typeof(ushort).GetTypeInfo(), "ushort" },
						{ typeof(string).GetTypeInfo(), "string" },
				};
#else
		// List of system types => C# type names
		private static readonly Dictionary<Type, string> TypeMappings = new Dictionary<Type, string>
				{
						{ typeof(bool), "bool" },
						{ typeof(byte), "byte" },
						{ typeof(sbyte), "sbyte" },
						{ typeof(char), "char" },
						{ typeof(decimal), "decimal" },
						{ typeof(double), "double" },
						{ typeof(float), "float" },
						{ typeof(int), "int" },
						{ typeof(uint), "uint" },
						{ typeof(long), "long" },
						{ typeof(ulong), "ulong" },
						{ typeof(object), "object" },
						{ typeof(short), "short" },
						{ typeof(ushort), "ushort" },
						{ typeof(string), "string" },
				};
#endif

		/// <summary>
		/// Format the value for presentation.
		/// </summary>
		/// <param name="value">The value to be formatted.</param>
		/// <returns>The formatted value.</returns>
		public static string Format(object value)
		{
			return Format(value, 1);
		}

		private static string Format(object value, int depth)
		{
			if (value == null)
				return "null";

			var valueAsType = value as Type;
			if (valueAsType != null)
			{
				return String.Format("typeof({0})", FormatTypeName(valueAsType));
			}

			if (value is char)
			{
				var charValue = (char)value;
				if (char.IsLetterOrDigit(charValue) || char.IsPunctuation(charValue) || char.IsSymbol(charValue) || charValue == ' ')
					return String.Format("'{0}'", value);

				return String.Format("0x{0:x4}", (int)charValue);
			}

			if (value is DateTime || value is DateTimeOffset)
				return String.Format("{0:o}", value);

			var stringParameter = value as string;
			if (stringParameter != null)
			{
				if (stringParameter.Length > MAX_STRING_LENGTH)
					return String.Format("\"{0}\"...", stringParameter.Substring(0, MAX_STRING_LENGTH));

				return String.Format("\"{0}\"", stringParameter);
			}

			var enumerable = value as IEnumerable;
			if (enumerable != null)
				return FormatEnumerable(enumerable.Cast<object>(), depth);

			var type = value.GetType();
			if (type.IsValueType())
				return Convert.ToString(value, CultureInfo.CurrentCulture);

#if NEW_REFLECTION
			var toString = type.GetRuntimeMethod("ToString", EmptyTypes);
#else
			var toString = type.GetMethod("ToString", EmptyTypes);
#endif

			if (toString != null && toString.DeclaringType != typeof(Object))
				return (string)toString.Invoke(value, EmptyObjects);

			return FormatComplexValue(value, depth, type);
		}

		private static string FormatComplexValue(object value, int depth, Type type)
		{
			if (depth == MAX_DEPTH)
				return String.Format("{0} {{ ... }}", type.Name);

			var fields = type.GetRuntimeFieldsEx()
											 .Where(f => f.IsPublic && !f.IsStatic)
											 .Select(f => new { name = f.Name, value = WrapAndGetFormattedValue(() => f.GetValue(value), depth) });
			var properties = type.GetRuntimePropertiesEx()
#if NET_4_0_ABOVE
.Where(p => p.GetMethod != null && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)
.Select(p => new { name = p.Name, value = WrapAndGetFormattedValue(() => p.GetValue(value), depth) });
#else
.Where(p => p.GetGetMethod(true) != null && p.GetGetMethod(true).IsPublic && !p.GetGetMethod(true).IsStatic)
.Select(p => new { name = p.Name, value = WrapAndGetFormattedValue(() => p.GetValue(value, null), depth) });
#endif
			var parameters = fields.Concat(properties)
														 .OrderBy(p => p.name)
														 .Take(MAX_OBJECT_PARAMETER_COUNT + 1)
														 .ToList();

			if (parameters.Count == 0)
				return String.Format("{0} {{ }}", type.Name);

			var formattedParameters = String.Join(", ", parameters.Take(MAX_OBJECT_PARAMETER_COUNT)
																														.Select(p => String.Format("{0} = {1}", p.name, p.value)));

			if (parameters.Count > MAX_OBJECT_PARAMETER_COUNT)
				formattedParameters += ", ...";

			return String.Format("{0} {{ {1} }}", type.Name, formattedParameters);
		}

		private static string FormatEnumerable(IEnumerable<object> enumerableValues, int depth)
		{
			if (depth == MAX_DEPTH)
				return "[...]";

			var values = enumerableValues.Take(MAX_ENUMERABLE_LENGTH + 1).ToList();
			var printedValues = String.Join(", ", values.Take(MAX_ENUMERABLE_LENGTH).Select(x => Format(x, depth + 1)));

			if (values.Count > MAX_ENUMERABLE_LENGTH)
				printedValues += ", ...";

			return String.Format("[{0}]", printedValues);
		}

		private static string FormatTypeName(Type type)
		{
#if NET_4_0_ABOVE
			var typeInfo = type.GetTypeInfo();
			var arraySuffix = "";

			// Deconstruct and re-construct array
			while (typeInfo.IsArray)
			{
				var rank = typeInfo.GetArrayRank();
				arraySuffix += string.Format("[{0}]", new String(',', rank - 1));
				typeInfo = typeInfo.GetElementType().GetTypeInfo();
			}

			// Map C# built-in type names
			string result;
			if (TypeMappings.TryGetValue(typeInfo, out result))
				return result + arraySuffix;

			// Strip off generic suffix
			var name = typeInfo.FullName;
			var tickIdx = name.IndexOf('`');
			if (tickIdx > 0)
				name = name.Substring(0, tickIdx);

			if (typeInfo.IsGenericTypeDefinition)
				name = String.Format("{0}<{1}>", name, new string(',', typeInfo.GenericTypeParameters.Length - 1));
			else if (typeInfo.IsGenericType)
			{
				if (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
					name = FormatTypeName(typeInfo.GenericTypeArguments[0]) + "?";
				else
					name = String.Format("{0}<{1}>", name, string.Join(", ", typeInfo.GenericTypeArguments.Select(FormatTypeName)));
			}

			return name + arraySuffix;
#else
			var arraySuffix = "";

			// Deconstruct and re-construct array
			while (type.IsArray)
			{
				var rank = type.GetArrayRank();
				arraySuffix += string.Format("[{0}]", new String(',', rank - 1));
				type = type.GetElementType();
			}

			// Map C# built-in type names
			string result;
			if (TypeMappings.TryGetValue(type, out result))
				return result + arraySuffix;

			// Strip off generic suffix
			var name = type.FullName;
			var tickIdx = name.IndexOf('`');
			if (tickIdx > 0)
				name = name.Substring(0, tickIdx);

			if (type.IsGenericTypeDefinition)
				name = String.Format("{0}<{1}>", name, new string(',', type.GenericTypeParametersEx().Length - 1));
			else if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
					name = FormatTypeName(type.GetGenericArgumentsEx()[0]) + "?";
				else
					name = String.Format("{0}<{1}>", name, string.Join(", ", type.GetGenericArgumentsEx().Select(FormatTypeName)));
			}

			return name + arraySuffix;
#endif
		}

		private static string WrapAndGetFormattedValue(Func<object> getter, int depth)
		{
			try
			{
				return Format(getter(), depth + 1);
			}
			catch (Exception ex)
			{
				return String.Format("(throws {0})", UnwrapException(ex).GetType().Name);
			}
		}

		private static Exception UnwrapException(Exception ex)
		{
			while (true)
			{
				var tiex = ex as TargetInvocationException;
				if (tiex == null)
					return ex;

				ex = tiex.InnerException;
			}
		}
	}
}