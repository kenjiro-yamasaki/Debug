using NSubstitute;
using System;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class TypeAssertTests
    {
        public class IsAssignableFrom_Generic
        {
            [Fact]
            public void null_失敗する()
            {
                XAssert.Throws<IsAssignableFromException>(() => Assert.IsAssignableFrom<object>(null));
            }

            [Fact]
            public void 同じ型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsAssignableFrom<InvalidCastException>(expected);
            }

            [Fact]
            public void 基底型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsAssignableFrom<Exception>(expected);
            }

            [Fact]
            public void インターフェイス_成功する()
            {
                var expected = Substitute.For<IDisposable>();

                Assert.IsAssignableFrom<IDisposable>(expected);
            }

            [Fact]
            public void 互換性のない型_失敗する()
            {
                var ex = XAssert.Throws<IsAssignableFromException>(() => Assert.IsAssignableFrom<InvalidCastException>(new InvalidOperationException()));

                XAssert.Equal("Assert.IsAssignableFrom() Failure", ex.UserMessage);
            }

            [Fact]
            public void 戻り値が正しい()
            {
                var expected = new InvalidCastException();
                var actual   = Assert.IsAssignableFrom<InvalidCastException>(expected);

                XAssert.Same(expected, actual);
            }
        }

        public class IsAssignableFrom_NonGeneric
        {
            [Fact]
            public void null_失敗する()
            {
                XAssert.Throws<IsAssignableFromException>(() => Assert.IsAssignableFrom(typeof(object), null));
            }

            [Fact]
            public void 同じ型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsAssignableFrom(typeof(InvalidCastException), expected);
            }

            [Fact]
            public void 基底型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsAssignableFrom(typeof(Exception), expected);
            }

            [Fact]
            public void インターフェイス_成功する()
            {
                var expected = Substitute.For<IDisposable>();

                Assert.IsAssignableFrom(typeof(IDisposable), expected);
            }

            [Fact]
            public void 互換性のない型_失敗する()
            {
                var ex = XAssert.Throws<IsAssignableFromException>(() => Assert.IsAssignableFrom(typeof(InvalidCastException), new InvalidOperationException()));

                Assert.Equal("Assert.IsAssignableFrom() Failure", ex.UserMessage);
            }
        }

        public class IsType_Generic
        {
            [Fact]
            public void 同じ型_成功する()
            {
                InvalidCastException expected = new InvalidCastException();

                Assert.IsType<InvalidCastException>(expected);
            }

            [Fact]
            public void 違う型_成功する()
            {
                var ex = XAssert.Throws<IsTypeException>(() => Assert.IsType<InvalidCastException>(new InvalidOperationException()));

                Assert.Equal("Assert.IsType() Failure", ex.UserMessage);
            }

            [Fact]
            public void null_失敗する()
            {
                XAssert.Throws<IsTypeException>(() => Assert.IsType<object>(null));
            }

            [Fact]
            public void 戻り値が正しい()
            {
                var expected = new InvalidCastException();

                var actual = Assert.IsType<InvalidCastException>(expected);

                XAssert.Same(expected, actual);
            }
        }

        public class IsType_NonGeneric
        {
            [Fact]
            public void 同じ型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsType(typeof(InvalidCastException), expected);
            }

            [Fact]
            public void 違う型_失敗する()
            {
                var ex = XAssert.Throws<IsTypeException>(() => Assert.IsType(typeof(InvalidCastException), new InvalidOperationException()));

                Assert.Equal("Assert.IsType() Failure", ex.UserMessage);
            }

            [Fact]
            public void null_失敗する()
            {
                XAssert.Throws<IsTypeException>(() => Assert.IsType(typeof(object), null));
            }
        }

        public class IsNotType_Generic
        {
            [Fact]
            public void 違う型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsNotType<Exception>(expected);
            }

            [Fact]
            public void 同じ型_失敗する()
            {
                var ex = XAssert.Throws<IsNotTypeException>(() => Assert.IsNotType<InvalidCastException>(new InvalidCastException()));

                Assert.Equal("Assert.IsNotType() Failure", ex.UserMessage);
            }

            [Fact]
            public void null_成功する()
            {
                Assert.IsNotType<object>(null);
            }
        }

        public class IsNotType_NonGeneric
        {
            [Fact]
            public void 違う型_成功する()
            {
                var expected = new InvalidCastException();

                Assert.IsNotType(typeof(Exception), expected);
            }

            [Fact]
            public void 同じ型_失敗する()
            {
                var ex = XAssert.Throws<IsNotTypeException>(() => Assert.IsNotType(typeof(InvalidCastException), new InvalidCastException()));

                Assert.Equal("Assert.IsNotType() Failure", ex.UserMessage);
            }

            [Fact]
            public void null_成功する()
            {
                Assert.IsNotType(typeof(object), null);
            }
        }
    }
}