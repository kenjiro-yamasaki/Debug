using System;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class NullAssertsTests
    {
        public class NotNull
        {
            [Fact]
            public void NotNullを指定_成功する()
            {
                Assert.NotNull(new object());
            }

            [Fact]
            public void Nullを指定_失敗する()
            {
                var ex = XAssert.Throws<NotNullException>(() => Assert.NotNull(null));

                XAssert.Equal("Assert.NotNull() Failure", ex.Message);
            }
        }

        public class Null
        {
            [Fact]
            public void Nullを指定_成功する()
            {
                Assert.Null(null);
            }

            [Fact]
            public void NotNullを指定_失敗する()
            {
                var ex = XAssert.Throws<NullException>(() => Assert.Null(new object()));

                XAssert.Equal("Assert.Null() Failure" + Environment.NewLine + "Expected: (null)" + Environment.NewLine + "Actual:   Object { }", ex.Message);
            }
        }
    }
}

