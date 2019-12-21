using System;
using System.Collections.Generic;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class SetAssertsTests
    {
        public class Subset
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Subset(null, new HashSet<int>()));
                XAssert.Throws<SubsetException>(() => Assert.Subset(new HashSet<int>(), null));
            }

            [Fact]
            public static void 部分集合_成功する()
            {
                var expectedSuperset = new HashSet<int> { 1, 2, 3 };
                var actual           = new HashSet<int> { 1, 2, 3 };

                Assert.Subset(expectedSuperset, actual);
            }

            [Fact]
            public static void 真部分集合_成功する()
            {
                var expectedSuperset = new HashSet<int> { 1, 2, 3, 4 };
                var actual           = new HashSet<int> { 1, 2, 3 };

                Assert.Subset(expectedSuperset, actual);
            }

            [Fact]
            public static void 部分集合ではない_失敗する()
            {
                var expectedSuperset = new HashSet<int> { 1, 2, 3 };
                var actual           = new HashSet<int> { 1, 2, 7 };

                var ex = Record.Exception(() => Assert.Subset(expectedSuperset, actual));

                XAssert.IsType<SubsetException>(ex);
                XAssert.Equal(
                    @"Assert.Subset() Failure" + Environment.NewLine +
                    @"Expected: HashSet<Int32> [1, 2, 3]" + Environment.NewLine +
                    @"Actual:   HashSet<Int32> [1, 2, 7]", ex.Message);
            }
        }

        public class ProperSubset
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.ProperSubset(null, new HashSet<int>()));
                XAssert.Throws<ProperSubsetException>(() => Assert.ProperSubset(new HashSet<int>(), null));
            }

            [Fact]
            public static void 部分集合_失敗する()
            {
                var expectedSuperset = new HashSet<int> { 1, 2, 3 };
                var actual           = new HashSet<int> { 1, 2, 3 };

                var ex = Record.Exception(() => Assert.ProperSubset(expectedSuperset, actual));

                XAssert.IsType<ProperSubsetException>(ex);
                XAssert.Equal(
                    @"Assert.ProperSubset() Failure" + Environment.NewLine +
                    @"Expected: HashSet<Int32> [1, 2, 3]" + Environment.NewLine +
                    @"Actual:   HashSet<Int32> [1, 2, 3]", ex.Message);
            }

            [Fact]
            public static void 真部分集合_成功する()
            {
                var expectedSuperset = new HashSet<int> { 1, 2, 3, 4 };
                var actual           = new HashSet<int> { 1, 2, 3 };

                Assert.ProperSubset(expectedSuperset, actual);
            }

            [Fact]
            public static void 部分集合ではない_失敗する()
            {
                var expectedSuperset = new HashSet<int> { 1, 2, 3 };
                var actual           = new HashSet<int> { 1, 2, 7 };

                XAssert.Throws<ProperSubsetException>(() => Assert.ProperSubset(expectedSuperset, actual));
            }
        }

        public class Superset
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Superset(null, new HashSet<int>()));
                XAssert.Throws<SupersetException>(() => Assert.Superset(new HashSet<int>(), null));
            }

            [Fact]
            public static void 上位集合_成功する()
            {
                var expectedSubset = new HashSet<int> { 1, 2, 3 };
                var actual         = new HashSet<int> { 1, 2, 3 };

                Assert.Superset(expectedSubset, actual);
            }

            [Fact]
            public static void 真上位集合_成功する()
            {
                var expectedSubset = new HashSet<int> { 1, 2, 3 };
                var actual         = new HashSet<int> { 1, 2, 3, 4 };

                Assert.Superset(expectedSubset, actual);
            }

            [Fact]
            public static void 上位集合ではない_失敗する()
            {
                var expectedSubset = new HashSet<int> { 1, 2, 3 };
                var actual         = new HashSet<int> { 1, 2, 7 };

                var ex = Record.Exception(() => Assert.Superset(expectedSubset, actual));

                XAssert.IsType<SupersetException>(ex);
                XAssert.Equal(
                    @"Assert.Superset() Failure" + Environment.NewLine +
                    @"Expected: HashSet<Int32> [1, 2, 3]" + Environment.NewLine +
                    @"Actual:   HashSet<Int32> [1, 2, 7]", ex.Message);
            }
        }

        public class ProperSuperset
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.ProperSuperset(null, new HashSet<int>()));
                XAssert.Throws<ProperSupersetException>(() => Assert.ProperSuperset(new HashSet<int>(), null));
            }

            [Fact]
            public static void 上位集合_失敗する()
            {
                var expectedSubset = new HashSet<int> { 1, 2, 3 };
                var actual         = new HashSet<int> { 1, 2, 3 };

                var ex = Record.Exception(() => Assert.ProperSuperset(expectedSubset, actual));

                XAssert.IsType<ProperSupersetException>(ex);
                XAssert.Equal(
                    @"Assert.ProperSuperset() Failure" + Environment.NewLine +
                    @"Expected: HashSet<Int32> [1, 2, 3]" + Environment.NewLine +
                    @"Actual:   HashSet<Int32> [1, 2, 3]", ex.Message);
            }

            [Fact]
            public static void 真上位集合_成功する()
            {
                var expectedSubset = new HashSet<int> { 1, 2, 3 };
                var actual         = new HashSet<int> { 1, 2, 3, 4 };

                Assert.ProperSuperset(expectedSubset, actual);
            }

            [Fact]
            public static void 上位集合ではない_失敗する()
            {
                var expectedSubset = new HashSet<int> { 1, 2, 3 };
                var actual = new HashSet<int> { 1, 2, 7 };

                XAssert.Throws<ProperSupersetException>(() => Assert.ProperSuperset(expectedSubset, actual));
            }
        }
    }
}
