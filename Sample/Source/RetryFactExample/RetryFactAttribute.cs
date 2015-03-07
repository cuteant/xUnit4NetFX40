using Xunit;
using Xunit.Sdk;

#if NET40
namespace RetryFactExample40
#else
namespace RetryFactExample45
#endif
{
    /// <summary>
    /// Works just like [Fact] except that failures are retried (by default, 3 times).
    /// </summary>
#if NET40
    [XunitTestCaseDiscoverer("RetryFactExample40.RetryFactDiscoverer", "RetryFactExample")]
#else
		[XunitTestCaseDiscoverer("RetryFactExample45.RetryFactDiscoverer", "RetryFactExample")]
#endif
    public class RetryFactAttribute : FactAttribute
    {
        /// <summary>
        /// Number of retries allowed for a failed test. If unset (or set less than 1), will
        /// default to 3 attempts.
        /// </summary>
        public int MaxRetries { get; set; }
    }
}
