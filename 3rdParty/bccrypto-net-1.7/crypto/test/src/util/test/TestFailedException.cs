using System;

namespace Org.BouncyCastle.Utilities.Test
{
    public class TestFailedException
        : Exception
    {
        private ITestResult _result;

        public TestFailedException(
            ITestResult result)
        {
            _result = result;
        }

        public ITestResult GetResult()
        {
            return _result;
        }
    }
}
