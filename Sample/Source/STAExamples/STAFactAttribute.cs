﻿using System;
using Xunit;
using Xunit.Sdk;

#if NET40
namespace STAExamples40
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[XunitTestCaseDiscoverer("STAExamples40.STAFactDiscoverer", "STAExamples")]
#else
namespace STAExamples45
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[XunitTestCaseDiscoverer("STAExamples45.STAFactDiscoverer", "STAExamples")]
#endif
	public class STAFactAttribute : FactAttribute
	{
	}
}