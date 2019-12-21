using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class CollectionAssertsTests
    {
        #region テスト用クラス

        sealed class SpyEnumerator<T> : IEnumerable<T>, IEnumerator<T>
        {
            IEnumerator<T> innerEnumerator;

            public SpyEnumerator(IEnumerable<T> enumerable)
            {
                innerEnumerator = enumerable.GetEnumerator();
            }

            public T Current => innerEnumerator.Current;

            object IEnumerator.Current => innerEnumerator.Current;

            public bool IsDisposed => innerEnumerator == null;

            public IEnumerator<T> GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;

            public bool MoveNext() => innerEnumerator.MoveNext();

            public void Reset() => throw new NotImplementedException();

            public void Dispose()
            {
                innerEnumerator.Dispose();
                innerEnumerator = null;
            }
        }

        #endregion

        public class All
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.All<object>(null, _ => { }));
                XAssert.Throws<ArgumentNullException>(() => Assert.All(new object[0], null));
            }

            [Fact]
            public static void 検査に合格しない項目がある_失敗する()
            {
                var items = new[] { 1, 1, 2, 2, 1, 1 };

                var acutual = Record.Exception(() => Assert.All(items, x => Assert.Equal(1, x)));

                var ex = XAssert.IsType<AllException>(acutual);
                XAssert.Equal(2, ex.Failures.Count);
                XAssert.All(ex.Failures, x => Assert.IsType<EqualException>(x));
            }

            [Fact]
            public static void すべての項目が検査に合格する_成功する()
            {
                var items = new[] { 1, 1, 1, 1, 1, 1 };

                Assert.All(items, x => Assert.Equal(1, x));
            }

            [Fact]
            public static void すべての項目が検査に合格しない_失敗する()
            {
                var items = new[] { 1, 1, 2, 2, 1, 1 };

                var acutual = Record.Exception(() => Assert.All(items, x => Assert.Equal(0, x)));

                var ex = XAssert.IsType<AllException>(acutual);
                XAssert.Equal(6, ex.Failures.Count);
                XAssert.All(ex.Failures, x => XAssert.IsType<EqualException>(x));
            }

            [Fact]
            public static void Nullの項目を含むコレクション_AllExceptonを投げる()
            {
                var collection = new object[]
                {
                    new object(),
                    null
                };

                var ex = Record.Exception(() => Assert.All(collection, Assert.NotNull));

                XAssert.IsType<AllException>(ex);
                XAssert.Contains("[1]: Item: ", ex.Message);
            }
        }

        public class Collection
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Collection((IEnumerable<int>)null));
            }

            [Fact]
            public static void 空のIEnumerable_成功する()
            {
                var list = new List<int>();

                Assert.Collection(list);
            }

            [Fact]
            public static void 項目と検査の数が合わない_失敗する()
            {
                var list = new List<int>();

                var actual = Record.Exception(() => Assert.Collection(list, item => Assert.True(false)));

                var ex = XAssert.IsType<CollectionException>(actual);
                XAssert.Equal(1, ex.ExpectedCount);
                XAssert.Equal(0, ex.ActualCount);
                XAssert.Equal("Assert.Collection() Failure" + Environment.NewLine + "Collection: []" + Environment.NewLine + "Expected item count: 1" + Environment.NewLine + "Actual item count:   0", ex.Message);
                XAssert.Null(ex.InnerException);
            }

            [Fact]
            public static void 空ではないIEnumerable_成功する()
            {
                var list = new List<int> { 42, 2112 };

                Assert.Collection(list, item => Assert.Equal(42, item), item => Assert.Equal(2112, item));
            }

            [Fact]
            public static void 検査に合格しない項目がある_失敗する()
            {
                var list = new List<int> { 42, 2112 };

                var actual = Record.Exception(() => Assert.Collection(list, item => Assert.Equal(42, item), item => Assert.Equal(2113, item)));

                var ex = XAssert.IsType<CollectionException>(actual);
                XAssert.Equal(1, ex.IndexFailurePoint);
                XAssert.Equal(
                    "Assert.Collection() Failure" + Environment.NewLine +
                    "Collection: [42, 2112]" + Environment.NewLine +
                    "Error during comparison of item at index 1" + Environment.NewLine +
                    "Inner exception: Assert.Equal() Failure" + Environment.NewLine +
                    "        Expected: 2113" + Environment.NewLine +
                    "        Actual:   2112", actual.Message);
            }
        }

        public class Contains
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>("collection", () => Assert.Contains(14, (List<int>)null));
            }

            [Fact]
            public static void Nullの期待値を含むIEnumerable_成功する()
            {
                var list = new List<object> { 16, null, "Hi there" };

                Assert.Contains(null, list);
            }

            [Fact]
            public static void 期待値を含むIEnumerable_成功する()
            {
                var list = new List<int> { 42 };

                Assert.Contains(42, list);
            }

            [Fact]
            public static void 期待値を含まないIEnumerable_失敗する()
            {
                var list = new List<int> { 41, 43 };

                var actual = Record.Exception(() => Assert.Contains(42, list));

                var ex = XAssert.IsType<ContainsException>(actual);
                XAssert.Equal(
                    "Assert.Contains() Failure" + Environment.NewLine +
                    "Not found: 42" + Environment.NewLine +
                    "In value:  List<Int32> [41, 43]", ex.Message);
            }

            [Fact]
            public static void Nullを含むIEnumerable_許容する()
            {
                var list = new List<object> { null, 16, "Hi there" };

                Assert.Contains("Hi there", list);
            }

            [Fact]
            public static void カスタム等値比較子で期待値を含むIEnumerable_成功する()
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Hi there" };

                Assert.Contains("HI THERE", set);
            }

            [Fact]
            public static void デフォルト等値比較子で期待値を含むIEnumerable_成功する()
            {
                var collections = new[] { new[] { 1, 2, 3, 4 } };

                Assert.Contains(new[] { 1, 2, 3, 4 }, collections);
            }

            [Fact]
            public static void キーを含むIDictionary_成功する()
            {
                var dictionary = (IDictionary<string, int>)new Dictionary<string, int>
                {
                    ["forty-two"] = 42
                };

                var actual = Assert.Contains("forty-two", dictionary);
                XAssert.Equal(42, actual);
            }

            [Fact]
            public static void キーを含まないIDictionary_失敗する()
            {
                var dictionary = (IDictionary<string, int>)new Dictionary<string, int>
                {
                    ["eleventeen"] = 110
                };

                var actual = Record.Exception(() => Assert.Contains("forty-two", dictionary));

                var ex = XAssert.IsType<ContainsException>(actual);
                XAssert.Equal(
                    "Assert.Contains() Failure" + Environment.NewLine +
                    "Not found: forty-two" + Environment.NewLine +
                   @"In value:  KeyCollection<String, Int32> [""eleventeen""]", ex.Message);
            }

            [Fact]
            public static void キーを含むIReadOnlyDictionary_成功する()
            {
                var dictionary = (IReadOnlyDictionary<string, int>)new Dictionary<string, int>
                {
                    ["forty-two"] = 42
                };

                var actual = Assert.Contains("forty-two", dictionary);
                XAssert.Equal(42, actual);
            }

            [Fact]
            public static void キーを含まないIReadOnlyDictionary_失敗する()
            {
                var dictionary = (IReadOnlyDictionary<string, int>)new Dictionary<string, int>
                {
                    ["eleventeen"] = 110
                };

                var actual = Record.Exception(() => Assert.Contains("forty-two", dictionary));

                var ex = Assert.IsType<ContainsException>(actual);
                XAssert.Equal(
                    "Assert.Contains() Failure" + Environment.NewLine +
                    "Not found: forty-two" + Environment.NewLine +
                   @"In value:  KeyCollection<String, Int32> [""eleventeen""]", ex.Message);
            }
        }

        public class Contains_WithComparer
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                var comparer = Substitute.For<IEqualityComparer<int>>();

                XAssert.Throws<ArgumentNullException>("collection", () => Assert.Contains(14, (List<int>)null, comparer));
                XAssert.Throws<ArgumentNullException>("comparer", () => Assert.Contains(14, new int[0], null));
            }

            [Fact]
            public static void ユーザー等値比較子を使う_成功する()
            {
                var list = new List<int> { 42 };
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(42, 43).Returns(true);

                Assert.Contains(43, list, comparer);
            }

            [Fact]
            public static void Assertとコレクションの両方に等値比較子を指定_Assertの等値比較子を優先する()
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Hi there" };

                var actual = Record.Exception(() => Assert.Contains("HI THERE", set, StringComparer.Ordinal));

                var ex = XAssert.IsType<ContainsException>(actual);
                XAssert.Equal($@"Assert.Contains() Failure{Environment.NewLine}Not found: HI THERE{Environment.NewLine}In value:  HashSet<String> [""Hi there""]", ex.Message);
            }
        }

        public class Contains_WithPredicate
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>("collection", () => Assert.Contains((List<int>)null, item => true));
                XAssert.Throws<ArgumentNullException>("filter", () => Assert.Contains(new int[0], (Predicate<int>)null));
            }

            [Fact]
            public static void 項目が見つかる_成功する()
            {
                var list = new[] { "Hello", "world" };

                Assert.Contains(list, item => item.StartsWith("w"));
            }

            [Fact]
            public static void 項目が見つからない_失敗する()
            {
                var list = new[] { "Hello", "world" };

                XAssert.Throws<ContainsException>(() => Assert.Contains(list, item => item.StartsWith("q")));
            }
        }

        public class DoesNotContain
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>("collection", () => Assert.DoesNotContain(14, (List<int>)null));
            }

            [Fact]
            public static void Nullを探す_許容する()
            {
                var list = new List<object> { 16, "Hi there" };

                Assert.DoesNotContain(null, list);
            }

            [Fact]
            public static void 項目を含む_失敗する()
            {
                var list = new List<int> { 42 };

                var actual = Record.Exception(() => Assert.DoesNotContain(42, list));

                var ex = XAssert.IsType<DoesNotContainException>(actual);
                XAssert.Equal(
                    "Assert.DoesNotContain() Failure" + Environment.NewLine +
                    "Found:    42" + Environment.NewLine +
                    "In value: List<Int32> [42]", ex.Message);
            }

            [Fact]
            public static void 項目を含まない_成功する()
            {
                var list = new List<int>();

                Assert.DoesNotContain(42, list);
            }

            [Fact]
            public static void Nullを含む_許容する()
            {
                var list = new List<object> { null, 16, "Hi there" };

                Assert.DoesNotContain(42, list);
            }

            [Fact]
            public static void コレクションに等値比較子を指定_等値比較子を使う()
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Hi there" };

                var actual = Record.Exception(() => Assert.DoesNotContain("HI THERE", set));

                var ex = XAssert.IsType<DoesNotContainException>(actual);
                XAssert.Equal(
                    "Assert.DoesNotContain() Failure" + Environment.NewLine +
                    "Found:    HI THERE" + Environment.NewLine +
                    @"In value: HashSet<String> [""Hi there""]", ex.Message);
            }

            [Fact]
            public static void キーを含まないIDictionary_成功する()
            {
                var dictionary = (IDictionary<string, int>)new Dictionary<string, int>
                {
                    ["eleventeen"] = 110
                };

                var actual = Record.Exception(() => Assert.DoesNotContain("forty-two", dictionary));

                XAssert.Null(actual);
            }

            [Fact]
            public static void キーを含むIDictionary_失敗する()
            {
                var dictionary = (IDictionary<string, int>)new Dictionary<string, int>
                {
                    ["forty-two"] = 42
                };

                var actual = Record.Exception(() => Assert.DoesNotContain("forty-two", dictionary));

                var ex = XAssert.IsType<DoesNotContainException>(actual);
                XAssert.Equal(
                    "Assert.DoesNotContain() Failure" + Environment.NewLine +
                    "Found:    forty-two" + Environment.NewLine +
                   @"In value: KeyCollection<String, Int32> [""forty-two""]", ex.Message);
            }

            [Fact]
            public static void キーを含まないIReadOnlyDictionary_成功する()
            {
                var dictionary = (IReadOnlyDictionary<string, int>)new Dictionary<string, int>
                {
                    ["eleventeen"] = 110
                };

                var actual = Record.Exception(() => Assert.DoesNotContain("forty-two", dictionary));

                XAssert.Null(actual);
            }

            [Fact]
            public static void キーを含むIReadOnlyDictionary_失敗する()
            {
                var dictionary = (IReadOnlyDictionary<string, int>)new Dictionary<string, int>
                {
                    ["forty-two"] = 42
                };

                var actual = Record.Exception(() => Assert.DoesNotContain("forty-two", dictionary));

                var ex = XAssert.IsType<DoesNotContainException>(actual);
                XAssert.Equal(
                    "Assert.DoesNotContain() Failure" + Environment.NewLine +
                    "Found:    forty-two" + Environment.NewLine +
                   @"In value: KeyCollection<String, Int32> [""forty-two""]", ex.Message);
            }
        }

        public class DoesNotContain_WithComparer
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                var comparer = Substitute.For<IEqualityComparer<int>>();

                XAssert.Throws<ArgumentNullException>("collection", () => Assert.DoesNotContain(14, (List<int>)null, comparer));
                XAssert.Throws<ArgumentNullException>("comparer", () => Assert.DoesNotContain(14, new int[0], null));
            }

            [Fact]
            public static void ユーザー等値比較子を使う_成功する()
            {
                var list = new List<int> { 42 };
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(42, 42).Returns(false);

                Assert.DoesNotContain(42, list, comparer);
            }

            [Fact]
            public static void Assertとコレクションの両方に等値比較子を指定_Assertの等値比較子を優先する()
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Hi there" };

                Assert.DoesNotContain("HI THERE", set, StringComparer.Ordinal);
            }
        }

        public class DoesNotContain_WithPredicate
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>("collection", () => Assert.DoesNotContain((List<int>)null, item => true));
                XAssert.Throws<ArgumentNullException>("filter", () => Assert.DoesNotContain(new int[0], (Predicate<int>)null));
            }

            [Fact]
            public static void 項目が見つかる_失敗する()
            {
                var list = new[] { "Hello", "world" };

                XAssert.Throws<DoesNotContainException>(() => Assert.DoesNotContain(list, item => item.StartsWith("w")));
            }

            [Fact]
            public static void 項目が見つからない_成功する()
            {
                var list = new[] { "Hello", "world" };

                Assert.DoesNotContain(list, item => item.StartsWith("q"));
            }
        }

        public class Empty
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Empty(null));
            }

            [Fact]
            public static void 空のIEnumerable_成功する()
            {
                var list = new List<int>();

                Assert.Empty(list);
            }

            [Fact]
            public static void 空ではないIEnumerable_失敗する()
            {
                var list = new List<int> { 42 };

                var actual = Record.Exception(() => Assert.Empty(list));

                var ex = XAssert.IsType<EmptyException>(actual);
                Assert.Equal($"Assert.Empty() Failure{Environment.NewLine}Expected: <empty>{Environment.NewLine}Actual:   [42]", ex.Message);
            }

            [Fact]
            public static void 反復子がDisposeされる()
            {
                var enumerator = new SpyEnumerator<int>(Enumerable.Empty<int>());

                Assert.Empty(enumerator);

                XAssert.True(enumerator.IsDisposed);
            }

            [Fact]
            public static void 空の文字列_成功する()
            {
                Assert.Empty("");
            }

            [Fact]
            public static void 空ではない文字列_失敗する()
            {
                var actual = Record.Exception(() => Assert.Empty("Foo"));

                var ex = XAssert.IsType<EmptyException>(actual);
                Assert.Equal($"Assert.Empty() Failure{Environment.NewLine}Expected: <empty>{Environment.NewLine}Actual:   \"Foo\"", ex.Message);
            }
        }

        public class Equal
        {
            [Fact]
            public static void 等値の配列_Equal()
            {
                string[] expected = { "@", "a", "ab", "b" };
                string[] actual   = { "@", "a", "ab", "b" };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public static void 等値の配列の配列_Equal()
            {
                string[][] expected = { new[] { "@", "a" }, new[] { "ab", "b" } };
                string[][] actual   = { new[] { "@", "a" }, new[] { "ab", "b" } };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public static void 長さが異なる配列_NotEqual()
            {
                string[] expected = { "@", "a", "ab", "b", "c" };
                string[] actual   = { "@", "a", "ab", "b" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            [Fact]
            public static void 値が異なる配列_NotEqual()
            {
                string[] expected = { "@", "d", "v", "d" };
                string[] actual   = { "@", "a", "ab", "b" };

                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
                Assert.NotEqual(expected, actual);
            }

            [Fact]
            public static void 等値の配列とList_Equal()
            {
                var expected = new int[] { 1, 2, 3, 4, 5 };
                var actual   = new List<int>(expected);

                Assert.Equal(expected, actual);
            }
        }

        public class EqualDictionary
        {
            [Fact]
            public static void 順番に並んだDictionary_Equal()
            {
                var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
                var actual   = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public static void 順番に並ばないDictionary_Equal()
            {
                var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
                var actual   = new Dictionary<string, int> { { "b", 2 }, { "c", 3 }, { "a", 1 } };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public static void 期待値の項目が多い_NotEqual()
            {
                var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
                var actual   = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };

                Assert.NotEqual(expected, actual);
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
            }

            [Fact]
            public static void 実測値の項目が多い_NotEqual()
            {
                var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
                var actual   = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };

                Assert.NotEqual(expected, actual);
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
            }
        }

        public class EqualSet
        {
            [Fact]
            public static void 順番に並んだSet_Equal()
            {
                var expected = new HashSet<int> { 1, 2, 3 };
                var actual   = new HashSet<int> { 1, 2, 3 };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public static void 順番に並ばないSet_Equal()
            {
                var expected = new HashSet<int> { 1, 2, 3 };
                var actual   = new HashSet<int> { 2, 3, 1 };

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }

            [Fact]
            public static void 期待値の項目が多い_NotEqual()
            {
                var expected = new HashSet<int> { 1, 2, 3 };
                var actual = new HashSet<int> { 1, 2 };

                Assert.NotEqual(expected, actual);
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
            }

            [Fact]
            public static void 実測値の項目が多い_NotEqual()
            {
                var expected = new HashSet<int> { 1, 2 };
                var actual = new HashSet<int> { 1, 2, 3 };

                Assert.NotEqual(expected, actual);
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
            }
        }

        public class Equal_WithComparer
        {
            [Fact]
            public static void trueを返す等値比較子_Equal()
            {
                var expected = new[] { 1, 2, 3, 4, 5 };
                var actual   = new List<int>(new int[] { 0, 0, 0, 0, 0 });
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

                Assert.Equal(expected, actual, comparer);
            }
        }

        public class NotEmpty
        {
            [Fact]
            public static void 空のIEnumerable_失敗する()
            {
                var list = new List<int>();

                var actual = Record.Exception(() => Assert.NotEmpty(list));

                var ex = XAssert.IsType<NotEmptyException>(actual);
                XAssert.Equal("Assert.NotEmpty() Failure", ex.Message);
            }

            [Fact]
            public static void 空ではないIEnumerable_成功する()
            {
                var list = new List<int> { 42 };

                Assert.NotEmpty(list);
            }

            [Fact]
            public static void 反復子がDisposeされる()
            {
                var enumerator = new SpyEnumerator<int>(Enumerable.Range(0, 1));

                Assert.NotEmpty(enumerator);

                XAssert.True(enumerator.IsDisposed);
            }
        }

        public class NotEqual
        {
            [Fact]
            public static void 等しくないIEnumerable_NotEqual()
            {
                var expected = new[] { 1, 2, 3, 4, 5 };
                var actual   = new List<int>(new[] { 1, 2, 3, 4, 6 });

                Assert.NotEqual(expected, actual);
                XAssert.Throws<EqualException>(() => Assert.Equal(expected, actual));
            }

            [Fact]
            public static void 等しいEnumerable_Equal()
            {
                var expected = new[] { 1, 2, 3, 4, 5 };
                var actual   = new List<int>(expected);

                Assert.Equal(expected, actual);
                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual));
            }
        }

        public class NotEqual_WithComparer
        {
            [Fact]
            public static void falseを返す等値比較子_NotEqual()
            {
                var expected = new[] { 1, 2, 3, 4, 5 };
                var actual   = new List<int>(new int[] { 1, 2, 3, 4, 5 });
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(Arg.Any<int>(), Arg.Any<int>()).Returns(false);

                Assert.NotEqual(expected, actual, comparer);
            }

            [Fact]
            public static void trueを返す等値比較子_Equal()
            {
                var expected = new[] { 1, 2, 3, 4, 5 };
                var actual   = new List<int>(new int[] { 0, 0, 0, 0, 0 });
                var comparer = Substitute.For<IEqualityComparer<int>>();
                comparer.Equals(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

                XAssert.Throws<NotEqualException>(() => Assert.NotEqual(expected, actual, comparer));
            }
        }

        public class Single_NonGeneric
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Single(null));
            }

            [Fact]
            public static void 空のIEnumerable_失敗する()
            {
                var collection = new ArrayList();

                var ex = Record.Exception(() => Assert.Single(collection));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element, but it was empty.", ex.Message);
            }

            [Fact]
            public static void 複数の項目をもつIEnumerable_失敗する()
            {
                var collection = new ArrayList { "Hello", "World" };

                var ex = Record.Exception(() => Assert.Single(collection));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element, but it contained 2 elements.", ex.Message);
            }

            [Fact]
            public static void 項目をひとつもつIEnumerable_成功する()
            {
                var collection = new ArrayList { "Hello" };

                var ex = Record.Exception(() => Assert.Single(collection));

                XAssert.Null(ex);
            }

            [Fact]
            public static void 項目をひとつもつIEnumerable_唯一の項目を返す()
            {
                var collection = new ArrayList { "Hello" };

                var result = Assert.Single(collection);

                XAssert.Equal("Hello", result);
            }
        }

        public class Single_NonGeneric_WithObject
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Single(null, null));
            }

            [Fact]
            public static void 項目をひとつもつIEnumerable_成功する()
            {
                var collection = new[] { "Hello", "World!" };

                Assert.Single(collection, "Hello");
            }

            [Fact]
            public static void Nullの項目をひとつもつIEnumerable_成功する()
            {
                var collection = new[] { "Hello", "World!", null };

                Assert.Single(collection, (string)null);
            }

            [Fact]
            public static void 項目が見つからない_失敗する()
            {
                var collection = new[] { "Hello", "World!" };

                var ex = Record.Exception(() => Assert.Single(collection, "foo"));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element matching \"foo\", but it contained no matching elements.", ex.Message);
            }

            [Fact]
            public static void 複数の項目が見つかる_失敗する()
            {
                var collection = new[] { "Hello", "World!", "Hello" };

                var ex = Record.Exception(() => Assert.Single(collection, "Hello"));

                Assert.IsType<SingleException>(ex);
                Assert.Equal("The collection was expected to contain a single element matching \"Hello\", but it contained 2 matching elements.", ex.Message);
            }
        }

        public class Single_Generic
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Single<object>(null));
            }

            [Fact]
            public static void 空のIEnumerable_失敗する()
            {
                var collection = new object[0];

                var ex = Record.Exception(() => Assert.Single(collection));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element, but it was empty.", ex.Message);
            }

            [Fact]
            public static void 複数の項目をもつIEnumerable_失敗する()
            {
                var collection = new[] { "Hello", "World!" };

                var ex = Record.Exception(() => Assert.Single(collection));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element, but it contained 2 elements.", ex.Message);
            }

            [Fact]
            public static void 項目をひとつもつIEnumerable_成功する()
            {
                var collection = new[] { "Hello" };

                var ex = Record.Exception(() => Assert.Single(collection));

                XAssert.Null(ex);
            }

            [Fact]
            public static void 項目をひとつもつIEnumerable_唯一の項目を返す()
            {
                var collection = new[] { "Hello" };

                var result = Assert.Single(collection);

                XAssert.Equal("Hello", result);
            }
        }

        public class Single_Generic_WithPredicate
        {
            [Fact]
            public static void 引数がNull_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Single<object>(null, _ => true));
                XAssert.Throws<ArgumentNullException>(() => Assert.Single<object>(new object[0], null));
            }

            [Fact]
            public static void 項目ひとつにマッチする述語_成功する()
            {
                var collection = new[] { "Hello", "World!" };

                var result = Assert.Single(collection, item => item.StartsWith("H"));

                XAssert.Equal("Hello", result);
            }

            [Fact]
            public static void 項目にマッチしない述語_失敗する()
            {
                var collection = new[] { "Hello", "World!" };

                var ex = Record.Exception(() => Assert.Single(collection, item => false));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element matching (filter expression), but it contained no matching elements.", ex.Message);
            }

            [Fact]
            public static void 複数の項目にマッチする述語_失敗する()
            {
                var collection = new[] { "Hello", "World!" };

                var ex = Record.Exception(() => Assert.Single(collection, item => true));

                XAssert.IsType<SingleException>(ex);
                XAssert.Equal("The collection was expected to contain a single element matching (filter expression), but it contained 2 matching elements.", ex.Message);
            }
        }
    }
}
