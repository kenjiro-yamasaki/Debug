using NSubstitute;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class EqualityAssertsTests
    {
        #region テスト用クラス

        /// <summary>
        /// <see cref="IEquatable{T}"/>も<see cref="IComparable"/>も<see cref="IComparable{T}"/>も実装しないクラス。
        /// </summary>
        private class NonEquatableComparable
        {
            public override bool Equals(object obj) => true;
            public override int GetHashCode() => throw new NotImplementedException();
        }

        /// <summary>
        /// 2つの型引数をもつISet<T>の実装クラス。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        private class TwoGenericSet<T, U> : HashSet<T>
        {
        }

        #endregion

        public class Equal
        {
            [Fact]
            public void Equalを失敗させる_例外オブジェクトが正しい()
            {
                var ex = XAssert.Throws<EqualException>(() => Assert.Equal(42, 2112));

                XAssert.Equal("42", ex.Expected);
                XAssert.Equal("2112", ex.Actual);
            }

            [Fact]
            public void NotEqualを失敗させる_例外オブジェクトが正しい()
            {
                var ex = Record.Exception(() => Assert.NotEqual("actual", "actual"));

                XAssert.IsType<NotEqualException>(ex);
                XAssert.Equal(@"Assert.NotEqual() Failure" + Environment.NewLine + @"Expected: Not ""actual""" + Environment.NewLine + @"Actual:   ""actual""", ex.Message);
            }

            #region int

            [Fact]
            public void 等値のint_Equal()
            {
                int expected = 42;
                int actual   = 42;

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public void 異なる値のint_NotEqual()
            {
                int expected = 42;
                int actual   = 2112;

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            #endregion

            #region double

            [Fact]
            public void 等値のdouble_Equal()
            {
                var expected = 0.11111;
                var actual   = 0.11444;

                Assert.Equal(expected, actual, 2);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual, 2));
            }

            [Fact]
            public void 異なる値のdouble_NotEqual()
            {
                var expected = 0.11111;
                var actual   = 0.11444;

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual, 3));
                Assert.NotEqual(expected, actual, 3);
            }

            #endregion

            #region Decimal

            [Fact]
            public void 等値のDecimal_Equal()
            {
                var expected = 0.11111M;
                var actual   = 0.11444M;

                Assert.Equal(expected, actual, 2);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual, 2));
            }

            [Fact]
            public void 異なる値のDecimal_NotEqual()
            {
                var expected = 0.11111M;
                var actual   = 0.11444M;

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual, 3));
                Assert.NotEqual(expected, actual, 3);
            }

            #endregion

            #region string

            [Fact]
            public void 等値のstring_Equal()
            {
                var expected = "bob";
                var actual   = "bob";

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public void 異なる値のstring_Equal()
            {
                var expected = "bob";
                var actual   = "jim";

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            [Fact]
            public void stringとdouble_NotEqual()
            {
                Assert.NotEqual("0", (object)0.0);
                Assert.NotEqual((object)0.0, "0");
            }

            #endregion

            #region IComparable

            [Fact]
            public void IComparable_CompareToが呼ばれる()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = new object();
                expected.CompareTo(actual).Returns(0);

                Assert.Equal(expected, actual);

                expected.Received(1).CompareTo(actual);
            }

            [Fact]
            public void ゼロを返すIComparable_Equal()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = Substitute.For<IComparable>();
                expected.CompareTo(actual).Returns(0);

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 非ゼロを返すIComparable_NotEqual()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = Substitute.For<IComparable>();
                expected.CompareTo(actual).Returns(1);

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値のIComparableかつCompareToが例外を投げる_Equal()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = expected;
                expected.CompareTo(actual).Returns(x => throw new InvalidOperationException());

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値のIComparableかつCompareToが例外を投げる_NotEqual()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = Substitute.For<IComparable>();
                expected.CompareTo(actual).Returns(x => throw new InvalidOperationException());

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));

                Assert.NotEqual(expected, actual);
            }

            #endregion

            #region IComparable<T>

            [Fact]
            public void IComparableT_CompareToが呼ばれる()
            {
                var expected = Substitute.For<IComparable<int>>();
                var actual   = 1;
                expected.CompareTo(actual).Returns(0);

                Assert.Equal(expected, actual);

                expected.Received(1).CompareTo(actual);
            }

            [Fact]
            public void ゼロを返すIComparableT_Equal()
            {
                var expected = Substitute.For<IComparable<int>>();
                var actual   = 1;
                expected.CompareTo(actual).Returns(0);

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public void 非ゼロを返すIComparableT_NotEqual()
            {
                var expected = Substitute.For<IComparable<int>>();
                var actual   = 1;
                expected.CompareTo(actual).Returns(1);

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            [Fact]
            public void 等値のIComparableTかつCompareToが例外を投げる_Equal()
            {
                var expected = Substitute.For<IComparable<object>>();
                var actual   = expected;
                expected.CompareTo(actual).Returns(x => throw new InvalidOperationException());

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値のIComparableTかつCompareToが例外を投げる_NotEqual()
            {
                var expected = Substitute.For<IComparable<object>>();
                var actual   = new object();
                expected.CompareTo(actual).Returns(x => throw new InvalidOperationException());

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));

                Assert.NotEqual(expected, actual);
            }

            #endregion

            #region IEquatable<T>

            [Fact]
            public void IEquatableT_Equalsが呼ばれる()
            {
                var expected = Substitute.For<IEquatable<int>>();
                var actual   = 1;
                expected.Equals(actual).Returns(true);

                Assert.Equal(expected, actual);

                expected.Received(1).Equals(actual);
            }

            [Fact]
            public void 等値と判断するIEquatableT_Equal()
            {
                var expected = Substitute.For<IEquatable<int>>();
                var actual   = 1;
                expected.Equals(actual).Returns(true);

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public void 異なる値と判断するIEquatableT_NotEqual()
            {
                var expected = Substitute.For<IEquatable<int>>();
                var actual   = 1;
                expected.Equals(actual).Returns(false);

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            #endregion

            #region IStructualEquatable

            [Fact]
            public void 等値のIStructuralEquatable_Equal()
            {
                var expected = new Tuple<string>("a");
                var actual   = new Tuple<string>("a");

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (IStructuralEquatable)actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (IStructuralEquatable)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値のIStructuralEquatable_NotEqual()
            {
                var expected = new Tuple<string>("a");
                var actual   = new Tuple<string>("b");

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (IStructuralEquatable)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (IStructuralEquatable)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            #endregion

            #region IDictionary

            [Fact]
            public void 等値のDictionary_Equal()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new Dictionary<string, string> { ["foo"] = "bar" };

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (IDictionary)actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (IDictionary)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 等値で異なる型のDictionary_Equal()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new ConcurrentDictionary<string, string>(expected);

                Assert.Equal(expected, (IDictionary)actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (IDictionary)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値のDictionary_NotEqual()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new Dictionary<string, string> { ["foo"] = "baz" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (IDictionary)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (IDictionary)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値と型のDictionary_NotEqual()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new ConcurrentDictionary<string, string> { ["foo"] = "baz" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (IDictionary)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, (IDictionary)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            #endregion

            #region ISet<T>

            [Fact]
            public void 等値のSetT_Equal()
            {
                var expected = new HashSet<string> { "foo", "bar" };
                var actual   = new HashSet<string> { "foo", "bar" };

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (ISet<string>)actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (ISet<string>)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 等値の型が異なるSetT_Equal()
            {
                var expected = new HashSet<string> { "bar", "foo" };
                var actual   = new SortedSet<string> { "foo", "bar" };

                Assert.Equal(expected, (ISet<string>)actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (ISet<string>)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 等値の型引数が2つのSetT_Equal()
            {
                var expected = new TwoGenericSet<string, int> { "foo", "bar" };
                var actual   = new TwoGenericSet<string, int> { "foo", "bar" };

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (ISet<string>)actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (ISet<string>)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値の型引数が2つのSetT_NotEqual()
            {
                var expected = new TwoGenericSet<string, int> { "foo", "bar" };
                var actual   = new TwoGenericSet<string, int> { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (ISet<string>)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる項目数の型引数が2つのSetT_NotEqual()
            {
                var expected = new TwoGenericSet<string, int> { "bar" };
                var actual   = new TwoGenericSet<string, int> { "foo", "bar" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (ISet<string>)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値のSetT_NotEqual()
            {
                var expected = new HashSet<string> { "foo", "bar" };
                var actual   = new HashSet<string> { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (ISet<string>)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値と型のSetT_NotEqual()
            {
                var expected = new HashSet<string> { "bar", "foo" };
                var actual   = new SortedSet<string> { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, (ISet<string>)actual);
                Assert.NotEqual(expected, (object)actual);
            }

            #endregion

            #region IEnumerable

            [Fact]
            public void 等値のIEnumerable_Equal()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = (IReadOnlyCollection<string>)new ReadOnlyCollection<string>(expected);

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値のIEnumerable_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = (IReadOnlyCollection<string>)new ReadOnlyCollection<string>(new string[] { "foo", "baz" });

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値の配列_Equal()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = new object[] { "foo", "bar" };

                Assert.Equal(expected, actual);
                Assert.Equal(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値の配列_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = new object[] { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));

                Assert.NotEqual(expected, actual);
                Assert.NotEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値の多次元配列_Equal()
            {
                var expected = new string[,] { { "foo" }, { "bar" } };
                var actual   = new string[,] { { "foo" }, { "bar" } };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public void 異なる値の多次元配列_NotEqual()
            {
                var expected = new string[,] { { "foo" }, { "bar" } };
                var actual   = new string[,] { { "foo" }, { "baz" } };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            [Fact]
            public void 異なる次元の多次元配列_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = new string[,] { { "foo" }, { "bar" } };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, (object)actual));
                Assert.NotEqual(expected, (object)actual);
            }

            #endregion

            #region その他

            [Fact]
            public void NonEquatableComparable_自分のEqualsが呼ばれる()
            {
                var expected = new NonEquatableComparable();
                var actual   = new NonEquatableComparable();

                Assert.Equal(expected, actual);
            }

            [Fact]
            public void 等値の深いネスト_Equal()
            {
                var expected = new List<object> { new List<object> { new List<object> { "a" } } };
                var actual   = new List<object> { new List<object> { new List<object> { "a" } } };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public void 異なる値の深いネスト_NotEqual()
            {
                var expected = new List<object> { new List<object> { new List<object> { "a" } } };
                var actual   = new List<object> { new List<object> { new List<object> { "b" } } };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            #endregion
        }

        public class StrictEqual
        {
            [Fact]
            public static void StrictEqualを失敗させる_例外オブジェクトが正しい()
            {
                var ex = XAssert.Throws<EqualException>(() => Assert.StrictEqual(42, 2112));

                XAssert.Equal("42", ex.Expected);
                XAssert.Equal("2112", ex.Actual);
            }

            [Fact]
            public static void NotStrictEqualを失敗させる_例外オブジェクトが正しい()
            {
                var ex = XAssert.Throws<NotEqualException>(() => Assert.NotStrictEqual("actual", "actual"));

                XAssert.Equal(@"Assert.NotEqual() Failure" + Environment.NewLine + @"Expected: Not ""actual""" + Environment.NewLine + @"Actual:   ""actual""", ex.Message);
            }

            #region int

            [Fact]
            public void 等値のint_Equal()
            {
                int expected = 42;
                int actual   = 42;

                Assert.StrictEqual(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotStrictEqual(expected, actual));
            }

            [Fact]
            public void 異なる値のint_NotEqual()
            {
                int expected = 42;
                int actual   = 2112;

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            #endregion

            #region string

            [Fact]
            public void 等値のstring_Equal()
            {
                var expected = "bob";
                var actual   = "bob";

                Assert.StrictEqual(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotStrictEqual(expected, actual));
            }

            [Fact]
            public void 異なる値のstring_Equal()
            {
                var expected = "bob";
                var actual   = "jim";

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            [Fact]
            public void stringとdouble_NotEqual()
            {
                Assert.NotStrictEqual("0", (object)0.0);
                Assert.NotStrictEqual((object)0.0, "0");
            }

            #endregion

            #region IComparable

            [Fact]
            public void IComparable_CompareToが呼ばれない()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = new object();
                expected.CompareTo(actual).Returns(0);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));

                expected.DidNotReceive().CompareTo(actual);
            }

            [Fact]
            public void ゼロを返すIComparable_NotEqual()
            {
                var expected = Substitute.For<IComparable>();
                var actual   = Substitute.For<IComparable>();
                expected.CompareTo(actual).Returns(0);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            #endregion

            #region IComparable<T>

            [Fact]
            public void IComparableT_CompareToが呼ばれない()
            {
                var expected = Substitute.For<IComparable<int>>();
                var actual   = 1;
                expected.CompareTo(actual).Returns(0);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));

                expected.DidNotReceive().CompareTo(actual);
            }

            [Fact]
            public void ゼロを返すIComparableT_Equal()
            {
                var expected = Substitute.For<IComparable<int>>();
                var actual   = 1;
                expected.CompareTo(actual).Returns(0);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            #endregion

            #region IEquatable<T>

            [Fact]
            public void IEquatableT_Equalsが呼ばれない()
            {
                var expected = Substitute.For<IEquatable<int>>();
                var actual   = 1;
                expected.Equals(actual).Returns(true);

                Assert.NotStrictEqual(expected, actual);

                expected.DidNotReceive().Equals(actual);
            }

            [Fact]
            public void 等値と判断するIEquatableT_NotEqual()
            {
                var expected = Substitute.For<IEquatable<int>>();
                var actual   = 1;
                expected.Equals(actual).Returns(true);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            #endregion

            #region IStructualEquatable

            [Fact]
            public void 等値のIStructualEquatable_Equal()
            {
                var expected = new Tuple<string>("a");
                var actual   = new Tuple<string>("a");

                Assert.StrictEqual(expected, actual);
                Assert.StrictEqual(expected, (IStructuralEquatable)actual);
                Assert.StrictEqual(expected, (object)actual);

                XAssert.Throws<NotEqualException>(() => Assert.NotStrictEqual(expected, actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotStrictEqual(expected, (IStructuralEquatable)actual));
                XAssert.Throws<NotEqualException>(() => Assert.NotStrictEqual(expected, (object)actual));
            }

            [Fact]
            public void 異なる値のIStructualEquatable_NotEqual()
            {
                var expected = new Tuple<string>("a");
                var actual   = new Tuple<string>("b");

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (IStructuralEquatable)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (IStructuralEquatable)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            #endregion

            #region IDictionary

            [Fact]
            public void 等値のDictionary_NotEqual()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new Dictionary<string, string> { ["foo"] = "bar" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (IDictionary)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (IDictionary)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値で異なる型のDictionary_NotEqual()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new ConcurrentDictionary<string, string>(expected);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (IDictionary)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, (IDictionary)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値のDictionary_NotEqual()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new Dictionary<string, string> { ["foo"] = "baz" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (IDictionary)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (IDictionary)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値と型のDictionary_NotEqual()
            {
                var expected = new Dictionary<string, string> { ["foo"] = "bar" };
                var actual   = new ConcurrentDictionary<string, string> { ["foo"] = "baz" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (IDictionary)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, (IDictionary)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            #endregion

            #region ISet<T>

            [Fact]
            public void 等値のSetT_NotEqual()
            {
                var expected = new HashSet<string> { "foo", "bar" };
                var actual   = new HashSet<string> { "foo", "bar" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値の型が異なるSetT_NotEqual()
            {
                var expected = new HashSet<string> { "bar", "foo" };
                var actual   = new SortedSet<string> { "foo", "bar" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値の型引数が2つのSetT_NotEqual()
            {
                var expected = new TwoGenericSet<string, int> { "foo", "bar" };
                var actual   = new TwoGenericSet<string, int> { "foo", "bar" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値の型引数が2つのSetT_NotEqual()
            {
                var expected = new TwoGenericSet<string, int> { "foo", "bar" };
                var actual   = new TwoGenericSet<string, int> { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる項目数の型引数が2つのSetT_NotEqual()
            {
                var expected = new TwoGenericSet<string, int> { "bar" };
                var actual   = new TwoGenericSet<string, int> { "foo", "bar" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値のSetT_NotEqual()
            {
                var expected = new HashSet<string> { "foo", "bar" };
                var actual   = new HashSet<string> { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値と型のSetT_NotEqual()
            {
                var expected = new HashSet<string> { "bar", "foo" };
                var actual   = new SortedSet<string> { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (ISet<string>)actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, (ISet<string>)actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            #endregion

            #region IEnumerable

            [Fact]
            public void 等値のIEnumerable_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = (IReadOnlyCollection<string>)new ReadOnlyCollection<string>(expected);

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値のIEnumerable_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = (IReadOnlyCollection<string>)new ReadOnlyCollection<string>(new string[] { "foo", "baz" });

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値の配列_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = new object[] { "foo", "bar" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 異なる値の配列_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = new object[] { "foo", "baz" };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));

                Assert.NotStrictEqual(expected, actual);
                Assert.NotStrictEqual(expected, (object)actual);
            }

            [Fact]
            public void 等値の多次元配列_NotEqual()
            {
                var expected = new string[,] { { "foo" }, { "bar" } };
                var actual   = new string[,] { { "foo" }, { "bar" } };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            [Fact]
            public void 異なる値の多次元配列_NotEqual()
            {
                var expected = new string[,] { { "foo" }, { "bar" } };
                var actual   = new string[,] { { "foo" }, { "baz" } };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            [Fact]
            public void 異なる次元の多次元配列_NotEqual()
            {
                var expected = new string[] { "foo", "bar" };
                var actual   = new string[,] { { "foo" }, { "bar" } };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, (object)actual));
                Assert.NotStrictEqual(expected, (object)actual);
            }

            #endregion

            #region その他

            [Fact]
            public void NonEquatableComparable_自分のEqualsが呼ばれる()
            {
                var expected = new NonEquatableComparable();
                var actual   = new NonEquatableComparable();

                Assert.StrictEqual(expected, actual);
            }

            [Fact]
            public void 等値の深いネスト_NotEqual()
            {
                var expected = new List<object> { new List<object> { new List<object> { "a" } } };
                var actual   = new List<object> { new List<object> { new List<object> { "a" } } };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            [Fact]
            public void 異なる値の深いネスト_NotEqual()
            {
                var expected = new List<object> { new List<object> { new List<object> { "a" } } };
                var actual   = new List<object> { new List<object> { new List<object> { "b" } } };

                XAssert.Throws<EqualException>(() => Assert.StrictEqual(expected, actual));
                Assert.NotStrictEqual(expected, actual);
            }

            #endregion
        }

        public class With_Comparer
        {
            #region int

            [Fact]
            public void 等値のint_Equal()
            {
                int expected = 42;
                int actual   = 24;
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(expected, actual).Returns(true);

                Assert.Equal(expected, actual, comparer);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual, comparer));
            }

            [Fact]
            public void 異なる値のint_NotEqual()
            {
                int expected = 42;
                int actual   = 42;
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(expected, actual).Returns(false);

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual, comparer));
                Assert.NotEqual(expected, actual, comparer);
            }

            #endregion
        }
    }
}
