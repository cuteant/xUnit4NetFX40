using System;
using Xunit.Sdk;

#if NET40
namespace TraitExtensibility40
#else
namespace TraitExtensibility45
#endif
{
	/// <summary>
	/// Apply this attribute to your test method to specify a category.
	/// </summary>
	[TraitDiscoverer("CategoryDiscoverer", "TraitExtensibility")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	class CategoryAttribute : Attribute, ITraitAttribute
	{
		public CategoryAttribute(string category) { }
	}
}