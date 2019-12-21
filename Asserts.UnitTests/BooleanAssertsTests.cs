using System;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class BooleanAssertsTests
    {
        public class False
        {
            [Fact]
            public static void falseを指定_成功する()
            {
                Assert.False(false);
            }

            [Fact]
            public static void trueを指定_失敗する()
            {
                var ex = XAssert.Throws<FalseException>(() => Assert.False(true));

                XAssert.Equal("Assert.False() Failure" + Environment.NewLine + "Expected: False" + Environment.NewLine + "Actual:   True", ex.Message);
            }

            [Fact]
            public static void nullを指定_失敗する()
            {
                var ex = XAssert.Throws<FalseException>(() => Assert.False(null));

                XAssert.Equal("Assert.False() Failure" + Environment.NewLine + "Expected: False" + Environment.NewLine + "Actual:   (null)", ex.Message);
            }

            [Fact]
            public static void ユーザーメッセージが例外に反映される()
            {
                var ex = XAssert.Throws<FalseException>(() => Assert.False(true, "Custom User Message"));

                XAssert.Equal("Custom User Message" + Environment.NewLine + "Expected: False" + Environment.NewLine + "Actual:   True", ex.Message);
            }
        }

        public class True
        {
            [Fact]
            public static void trueを指定_成功する()
            {
                Assert.True(true);
            }

            [Fact]
            public static void falseを指定_失敗する()
            {
                var ex = XAssert.Throws<TrueException>(() => Assert.True(false));

                XAssert.Equal("Assert.True() Failure" + Environment.NewLine + "Expected: True" + Environment.NewLine + "Actual:   False", ex.Message);
            }

            [Fact]
            public static void nullを指定_失敗する()
            {
                var ex = XAssert.Throws<TrueException>(() => Assert.True(null));

                XAssert.Equal("Assert.True() Failure" + Environment.NewLine + "Expected: True" + Environment.NewLine + "Actual:   (null)", ex.Message);
            }

            [Fact]
            public static void ユーザーメッセージが例外に反映される()
            {
                var ex = XAssert.Throws<TrueException>(() => Assert.True(false, "Custom User Message"));

                XAssert.Equal("Custom User Message" + Environment.NewLine + "Expected: True" + Environment.NewLine + "Actual:   False", ex.Message);
            }
        }
    }
}


