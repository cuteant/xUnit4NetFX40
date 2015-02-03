﻿using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace STAExamples
{
    public class STAFactDiscoverer : IXunitTestCaseDiscoverer
    {
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var xUnitTestCase = new XunitTestCase(discoveryOptions.MethodDisplay(), testMethod, new object[] { }); 
            yield return new STATestCase(xUnitTestCase);
        }
    }
}
