using System;
using Xunit.Sdk;

namespace Xunit
{
	/// <summary>Apply this attribute to your test method to specify a category.</summary>
	[TraitDiscoverer("Xunit.Sdk.CategoryDiscoverer", "xunit.extensions2")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class CategoryAttribute : Attribute, ITraitAttribute
	{
		public CategoryAttribute(String category) { }
	}
}