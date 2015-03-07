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
#if NET40
	[TraitDiscoverer("TraitExtensibility40.CategoryDiscoverer", "TraitExtensibility")]
#else
	[TraitDiscoverer("TraitExtensibility45.CategoryDiscoverer", "TraitExtensibility")]
#endif
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	class CategoryAttribute : Attribute, ITraitAttribute
	{
		public CategoryAttribute(string category) { }
	}
}