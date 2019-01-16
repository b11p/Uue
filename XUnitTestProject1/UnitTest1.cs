using System;
using UueLib;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var emptyEncoding = Class1.ToUUEncodingString(new byte[0], 0, 0, false);
            Assert.Equal(string.Empty, emptyEncoding);
        }
    }
}
