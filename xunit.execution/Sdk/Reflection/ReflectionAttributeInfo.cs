using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	/// <summary>
	/// Reflection-based implementation of <see cref="IReflectionAttributeInfo"/>.
	/// </summary>
	public class ReflectionAttributeInfo : LongLivedMarshalByRefObject, IReflectionAttributeInfo
	{
		static readonly AttributeUsageAttribute DefaultAttributeUsageAttribute = new AttributeUsageAttribute(AttributeTargets.All);

		/// <summary>
		/// Initializes a new instance of the <see cref="ReflectionAttributeInfo"/> class.
		/// </summary>
		/// <param name="attribute">The attribute to be wrapped.</param>
		public ReflectionAttributeInfo(CustomAttributeData attribute)
		{
			AttributeData = attribute;
			Attribute = Instantiate(AttributeData);
		}

		/// <inheritdoc/>
		public Attribute Attribute { get; private set; }

		/// <inheritdoc/>
		public CustomAttributeData AttributeData { get; private set; }

		static IEnumerable<object> Convert(IEnumerable<CustomAttributeTypedArgument> arguments)
		{
			foreach (var argument in arguments)
			{
				var value = argument.Value;

				// Collections are recursively IEnumerable<CustomAttributeTypedArgument> rather than
				// being the exact matching type, so the inner values must be converted.
				var valueAsEnumerable = value as IEnumerable<CustomAttributeTypedArgument>;
				if (valueAsEnumerable != null)
				{
					value = Convert(valueAsEnumerable).ToArray();
				}
#if NET_4_0_ABOVE
				else if (value != null && value.GetType() != argument.ArgumentType && argument.ArgumentType.GetTypeInfo().IsEnum)
#else
				else if (value != null && value.GetType() != argument.ArgumentType && argument.ArgumentType.IsEnum)
#endif
				{
					value = Enum.Parse(argument.ArgumentType, value.ToString());
				}

#if NET_4_0_ABOVE
				if (value != null && value.GetType() != argument.ArgumentType && argument.ArgumentType.GetTypeInfo().IsArray)
#else
				if (value != null && value.GetType() != argument.ArgumentType && argument.ArgumentType.IsArray)
#endif
				{
					value = Reflector.ConvertArgument(value, argument.ArgumentType);
				}

				yield return value;
			}
		}

		internal static AttributeUsageAttribute GetAttributeUsage(Type attributeType)
		{
#if NET_4_0_ABOVE
			return (AttributeUsageAttribute)attributeType.GetTypeInfo().GetCustomAttributes(typeof(AttributeUsageAttribute), true).FirstOrDefault()
					?? DefaultAttributeUsageAttribute;
#else
			return attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), true)
													.Cast<AttributeUsageAttribute>()
													.FirstOrDefault()
					?? DefaultAttributeUsageAttribute;
#endif
		}

		/// <inheritdoc/>
		public IEnumerable<object> GetConstructorArguments()
		{
			return Convert(AttributeData.ConstructorArguments).ToList();
		}

		/// <inheritdoc/>
		public IEnumerable<IAttributeInfo> GetCustomAttributes(string assemblyQualifiedAttributeTypeName)
		{
			return GetCustomAttributes(Attribute.GetType(), assemblyQualifiedAttributeTypeName).ToList();
		}

		internal static IEnumerable<IAttributeInfo> GetCustomAttributes(Type type, string assemblyQualifiedAttributeTypeName)
		{
			Type attributeType = SerializationHelper.GetType(assemblyQualifiedAttributeTypeName);

			return GetCustomAttributes(type, attributeType, GetAttributeUsage(attributeType));
		}

		internal static IEnumerable<IAttributeInfo> GetCustomAttributes(Type type, Type attributeType, AttributeUsageAttribute attributeUsage)
		{
			IEnumerable<IAttributeInfo> results = Enumerable.Empty<IAttributeInfo>();

#if NET_4_0_ABOVE
			if (type != null)
			{
				List<ReflectionAttributeInfo> list = null;
				foreach (CustomAttributeData attr in type.GetTypeInfo().CustomAttributes)
				{
					if (attributeType.GetTypeInfo().IsAssignableFrom(attr.AttributeType.GetTypeInfo()))
					{
						if (list == null)
							list = new List<ReflectionAttributeInfo>();

						list.Add(new ReflectionAttributeInfo(attr));
					}
				}

				if (list != null)
					list.Sort((left, right) => string.Compare(left.AttributeData.AttributeType.Name, right.AttributeData.AttributeType.Name, StringComparison.Ordinal));

				results = list ?? Enumerable.Empty<IAttributeInfo>();

				if (attributeUsage.Inherited && (attributeUsage.AllowMultiple || list == null))
					results = results.Concat(GetCustomAttributes(type.GetTypeInfo().BaseType, attributeType, attributeUsage));
			}
#else
			if (type != null)
			{
				List<ReflectionAttributeInfo> list = null;
				foreach (CustomAttributeData attr in type.GetCustomAttributesData())
				{
					if (attributeType.IsAssignableFrom(attr.Constructor.DeclaringType))
					{
						if (list == null)
							list = new List<ReflectionAttributeInfo>();

						list.Add(new ReflectionAttributeInfo(attr));
					}
				}

				if (list != null)
					list.Sort((left, right) => string.Compare(left.AttributeData.Constructor.DeclaringType.Name, right.AttributeData.Constructor.DeclaringType.Name, StringComparison.Ordinal));

				results = list ?? Enumerable.Empty<IAttributeInfo>();

				if (attributeUsage.Inherited && (attributeUsage.AllowMultiple || list == null))
					results = results.Concat(GetCustomAttributes(type.BaseType, attributeType, attributeUsage));
			}
#endif

			return results;
		}

		/// <inheritdoc/>
		public TValue GetNamedArgument<TValue>(string propertyName)
		{
			PropertyInfo propInfo = default(PropertyInfo);
			foreach (var pi in Attribute.GetType().GetRuntimePropertiesEx())
			{
				if (pi.Name == propertyName)
				{
					propInfo = pi;
					break;
				}
			}

			Guard.ArgumentValid("propertyName", $"Could not find property {propertyName} on instance of {Attribute.GetType().FullName}", propInfo != null);

			return (TValue)propInfo.GetValue(Attribute, Reflector.EmptyArgs);
		}

		Attribute Instantiate(CustomAttributeData attributeData)
		{
			var ctorArgs = GetConstructorArguments().ToArray();
			Type[] ctorArgTypes = Reflector.EmptyTypes;
			if (ctorArgs.Length > 0)
			{
				ctorArgTypes = new Type[attributeData.ConstructorArguments.Count];
				for (int i = 0; i < ctorArgTypes.Length; i++)
					ctorArgTypes[i] = attributeData.ConstructorArguments[i].ArgumentType;
			}

#if NET_4_0_ABOVE
			var attribute = (Attribute)Activator.CreateInstance(attributeData.AttributeType, Reflector.ConvertArguments(ctorArgs, ctorArgTypes));
#else
			var attribute = (Attribute)Activator.CreateInstance(attributeData.Constructor.DeclaringType, Reflector.ConvertArguments(ctorArgs, ctorArgTypes));
#endif

			var ati = attribute.GetType();

			for (int i = 0; i < attributeData.NamedArguments.Count; i++)
			{
				var namedArg = attributeData.NamedArguments[i];
#if NET_4_0_ABOVE
				(ati.GetRuntimeProperty(namedArg.MemberName)).SetValue(attribute, GetTypedValue(namedArg.TypedValue), null);
#else
				(ati.GetPropertyEx(namedArg.MemberInfo.Name, false)).SetValue(attribute, namedArg.TypedValue.Value, index: null);
#endif
			}

			return attribute;
		}

#if NET_4_0_ABOVE
		object GetTypedValue(CustomAttributeTypedArgument arg)
		{
			var collect = arg.Value as IReadOnlyCollection<CustomAttributeTypedArgument>;

			if (collect == null)
				return arg.Value;

			var argType = arg.ArgumentType.GetElementType();
			Array destinationArray = Array.CreateInstance(argType, collect.Count);

			if (argType.IsEnum())
				Array.Copy(collect.Select(x => Enum.ToObject(argType, x.Value)).ToArray(), destinationArray, collect.Count);
			else
				Array.Copy(collect.Select(x => x.Value).ToArray(), destinationArray, collect.Count);

			return destinationArray;
		}
#endif

		/// <inheritdoc/>
		public override string ToString()
		{
			return Attribute.ToString();
		}
	}
}