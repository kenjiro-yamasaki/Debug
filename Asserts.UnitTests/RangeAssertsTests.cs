using System.Collections.Generic;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class RangeAssertsTests
    {
        #region テスト用クラス

        private class DoubleComparer : IComparer<double>
        {
            int returnValue;

            public DoubleComparer(int returnValue)
            {
                this.returnValue = returnValue;
            }

            public int Compare(double x, double y)
            {
                return returnValue;
            }
        }

        #endregion

        public class InRange
        {
            [Fact]
            public void 下限_成功する()
            {
                Assert.InRange(1, 1, 2);
            }

            [Fact]
            public void 下限外_失敗する()
            {
                XAssert.Throws<InRangeException>(() => Assert.InRange(0, 1, 2));
            }

            [Fact]
            public void 上限_成功する()
            {
                Assert.InRange(1, 0, 1);
            }

            [Fact]
            public void 上限外_失敗する()
            {
                XAssert.Throws<InRangeException>(() => Assert.InRange(2, 0, 1));
            }

            [Fact]
            public void 範囲内_成功する()
            {
                Assert.InRange(2, 1, 3);
            }

            [Fact]
            public void 範囲外のdouble_失敗する()
            {
                XAssert.Throws<InRangeException>(() => Assert.InRange(1.50, .75, 1.25));
            }

            [Fact]
            public void 範囲内のdouble_成功する()
            {
                Assert.InRange(1.0, .75, 1.25);
            }

            [Fact]
            public void 範囲外のstring_失敗する()
            {
                XAssert.Throws<InRangeException>(() => Assert.InRange("adam", "bob", "scott"));
            }

            [Fact]
            public void 範囲内のstring_成功する()
            {
                Assert.InRange("bob", "adam", "scott");
            }
        }

        public class InRange_WithComparer
        {
            [Fact]
            public void 範囲内のdouble_成功する()
            {
                Assert.InRange(400.0, .75, 1.25, new DoubleComparer(-1));
            }

            [Fact]
            public void 範囲外のdouble_失敗する()
            {
                XAssert.Throws<InRangeException>(() => Assert.InRange(1.0, .75, 1.25, new DoubleComparer(1)));
            }
        }

        public class NotInRange
        {
            [Fact]
            public void 下限_失敗する()
            {
                XAssert.Throws<NotInRangeException>(() => Assert.NotInRange(1, 1, 3));
            }

            [Fact]
            public void 下限外_成功する()
            {
                Assert.NotInRange(1, 2, 3);
            }

            [Fact]
            public void 上限_失敗する()
            {
                XAssert.Throws<NotInRangeException>(() => Assert.NotInRange(3, 1, 3));
            }

            [Fact]
            public void 上限外_成功する()
            {
                Assert.NotInRange(4, 2, 3);
            }

            [Fact]
            public void 範囲内_失敗する()
            {
                XAssert.Throws<NotInRangeException>(() => Assert.NotInRange(2, 1, 3));
            }

            [Fact]
            public void 範囲外のdouble_成功する()
            {
                Assert.NotInRange(1.50, .75, 1.25);
            }

            [Fact]
            public void 範囲内のdouble_失敗する()
            {
                XAssert.Throws<NotInRangeException>(() => Assert.NotInRange(1.0, .75, 1.25));
            }

            [Fact]
            public void 範囲外のstring_成功する()
            {
                Assert.NotInRange("adam", "bob", "scott");
            }

            [Fact]
            public void 範囲内のstring_失敗する()
            {
                XAssert.Throws<NotInRangeException>(() => Assert.NotInRange("bob", "adam", "scott"));
            }
        }

        public class NotInRange_WithComparer
        {
            [Fact]
            public void 範囲内のdouble_失敗する()
            {
                XAssert.Throws<NotInRangeException>(() => Assert.NotInRange(400.0, .75, 1.25, new DoubleComparer(-1)));
            }

            [Fact]
            public void 範囲外のdouble_成功する()
            {
                Assert.NotInRange(1.0, .75, 1.25, new DoubleComparer(1));
            }
        }
    }
}
