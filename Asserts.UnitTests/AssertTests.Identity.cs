using Xunit;

namespace SoftCube.Asserts
{
    using XAssert = Xunit.Assert;

    public partial class AssertsTests
    {
        public class NotSame
        {
            [Fact]
            public void 違うインスタンス_失敗する()
            {
                Assert.NotSame("bob", "jim");
            }

            [Fact]
            public void 同じインスタンス_成功する()
            {
                object actual = new object();

                var ex = Record.Exception(() => Assert.NotSame(actual, actual));

                XAssert.IsType<NotSameException>(ex);
                XAssert.Equal("Assert.NotSame() Failure", ex.Message);
            }
        }

        public class Same
        {
            [Fact]
            public void 違うインスタンス_失敗する()
            {
                XAssert.Throws<SameException>(() => Assert.Same("bob", "jim"));
            }

            [Fact]
            public void 同じインスタンス_成功する()
            {
                var actual   = "Abc";
                var expected = "a".ToUpperInvariant() + "bc";

                var actualEx = Record.Exception(() => Assert.Same(expected, actual));

                var ex = Assert.IsType<SameException>(actualEx);
                XAssert.Equal("Assert.Same() Failure", ex.UserMessage);
                XAssert.DoesNotContain("Position:", ex.Message);
            }

            [Fact]
            public void 値型_失敗する()
            {
                int index = 0;

                XAssert.Throws<SameException>(() => Assert.Same(index, index));
            }
        }
    }
}
