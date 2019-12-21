using System;
using System.Text.RegularExpressions;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class StringAssertsTests
    {
        public class Contains
        {
            [Fact]
            public void 期待値がnull_ArgumentNullExceptionを投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Contains((string)null, "Hello, world!"));
            }

            [Fact]
            public void 部分文字列を含む_成功する()
            {
                Assert.Contains("wor", "Hello, world!");
            }

            [Fact]
            public void 部分文字列の大文字小文字が違う_失敗する()
            {
                var ex = XAssert.Throws<ContainsException>(() => Assert.Contains("WORLD", "Hello, world!"));

                XAssert.Equal("Assert.Contains() Failure" + Environment.NewLine + "Not found: WORLD" + Environment.NewLine + "In value:  Hello, world!", ex.Message);
            }

            [Fact]
            public void 部分文字列を含まない_失敗する()
            {
                XAssert.Throws<ContainsException>(() => Assert.Contains("hey", "Hello, world!"));
            }

            [Fact]
            public void 実測値がnull_失敗する()
            {
                XAssert.Throws<ContainsException>(() => Assert.Contains("foo", (string)null));
            }
        }

        public class Contains_WithStringComparison
        {
            [Fact]
            public void OrdinalIgnoreCase_部分文字列の大文字小文字の違いを無視する()
            {
                Assert.Contains("WORLD", "Hello, world!", StringComparison.OrdinalIgnoreCase);
            }
        }

        public class DoesNotContain
        {
            [Fact]
            public void 期待値にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.DoesNotContain((string)null, "Hello, world!"));
            }

            [Fact]
            public void 部分文字列を含まない_成功する()
            {
                Assert.DoesNotContain("hey", "Hello, world!");
            }

            [Fact]
            public void 部分文字列の大文字小文字が違う_成功する()
            {
                Assert.DoesNotContain("WORLD", "Hello, world!");
            }

            [Fact]
            public void 部分文字列を含む_失敗する()
            {
                XAssert.Throws<DoesNotContainException>(() => Assert.DoesNotContain("world", "Hello, world!"));
            }

            [Fact]
            public void 実測値にnullを指定_成功する()
            {
                Assert.DoesNotContain("foo", (string)null);
            }
        }

        public class DoesNotContain_WithStringComparison
        {
            [Fact]
            public void OrdinalIgnoreCase_部分文字列の大文字小文字の違いを無視する()
            {
                XAssert.Throws<DoesNotContainException>(() => Assert.DoesNotContain("WORLD", "Hello, world!", StringComparison.OrdinalIgnoreCase));
            }
        }

        public class Equal
        {
            [Theory]
            [InlineData(null, null, false, false, false)]
            [InlineData("foo", "foo", false, false, false)]
            [InlineData("foo", "FoO", true, false, false)]
            [InlineData("foo \r\n bar", "foo \r bar", false, true, false)]
            [InlineData("foo \r\n bar", "foo \n bar", false, true, false)]
            [InlineData("foo \n bar", "foo \r bar", false, true, false)]
            [InlineData(" ", "\t", false, false, true)]
            [InlineData(" \t", "\t ", false, false, true)]
            [InlineData("    ", "\t", false, false, true)]
            public void 成功する(string value1, string value2, bool ignoreCase, bool ignoreLineEndingDifferences, bool ignoreWhiteSpaceDifferences)
            {
                Assert.Equal(value1, value2, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences);
                Assert.Equal(value2, value1, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences);
            }

            [Theory]
            [InlineData(null, "", false, false, false, -1, -1)]
            [InlineData("", null, false, false, false, -1, -1)]
            [InlineData("foo", "foo!", false, false, false, 3, 3)]
            [InlineData("foo", "foo\0", false, false, false, 3, 3)]
            [InlineData("foo bar", "foo   Bar", false, true, true, 4, 6)]
            [InlineData("foo \nbar", "FoO  \rbar", true, false, true, 4, 5)]
            [InlineData("foo\n bar", "FoO\r\n  bar", true, true, false, 5, 6)]
            public void 失敗する(string expected, string actual, bool ignoreCase, bool ignoreLineEndingDifferences, bool ignoreWhiteSpaceDifferences, int expectedIndex, int actualIndex)
            {
                var actualEx = Record.Exception(() => Assert.Equal(expected, actual, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences));

                var ex = XAssert.IsType<EqualException>(actualEx);
                XAssert.Equal(expectedIndex, ex.ExpectedIndex);
                XAssert.Equal(actualIndex, ex.ActualIndex);
            }
        }

        public class StartsWith
        {
            [Fact]
            public void 期待値にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.StartsWith((string)null, "Hello, world!"));
            }

            [Fact]
            public void 部分文字列から始まる_成功する()
            {
                Assert.StartsWith("Hello", "Hello, world!");
            }

            [Fact]
            public void 部分文字列の大文字小文字が違う_失敗する()
            {
                var ex = Record.Exception(() => Assert.StartsWith("HELLO", "Hello"));

                XAssert.IsType<StartsWithException>(ex);
                XAssert.Equal("Assert.StartsWith() Failure:" + Environment.NewLine + "Expected: HELLO" + Environment.NewLine + "Actual:   Hello", ex.Message);
            }

            [Fact]
            public void 部分文字列から始まらない_失敗する()
            {
                XAssert.Throws<StartsWithException>(() => Assert.StartsWith("hey", "Hello, world!"));
            }

            [Fact]
            public void 実測値にnullを指定_失敗する()
            {
                XAssert.Throws<StartsWithException>(() => Assert.StartsWith("foo", null));
            }
        }

        public class StartsWith_WithStringComparison
        {
            [Fact]
            public void OrdinalIgnoreCase_部分文字列の大文字小文字の違いを無視する()
            {
                Assert.StartsWith("HELLO", "Hello, world!", StringComparison.OrdinalIgnoreCase);
            }
        }

        public class EndsWith
        {
            [Fact]
            public void 期待値にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.EndsWith((string)null, "Hello, world!"));
            }

            [Fact]
            public void 部分文字列で終わる_成功する()
            {
                Assert.EndsWith("world!", "Hello, world!");
            }

            [Fact]
            public void 部分文字列の大文字小文字が違う_失敗する()
            {
                var ex = Record.Exception(() => Assert.EndsWith("WORLD!", "world!"));

                XAssert.IsType<EndsWithException>(ex);
                XAssert.Equal("Assert.EndsWith() Failure:" + Environment.NewLine + "Expected: WORLD!" + Environment.NewLine + "Actual:   world!", ex.Message);
            }

            [Fact]
            public void 部分文字列で終わらない_失敗する()
            {
                XAssert.Throws<EndsWithException>(() => Assert.EndsWith("hey", "Hello, world!"));
            }

            [Fact]
            public void 実測値にnullを指定_失敗する()
            {
                XAssert.Throws<EndsWithException>(() => Assert.EndsWith("foo", null));
            }
        }

        public class EndsWith_WithStringComparison
        {
            [Fact]
            public void OrdinalIgnoreCase_部分文字列の大文字小文字の違いを無視する()
            {
                Assert.EndsWith("WORLD!", "Hello, world!", StringComparison.OrdinalIgnoreCase);
            }
        }

        public class Matches_WithString
        {
            [Fact]
            public void 実測値にnullを指定_失敗する()
            {
                XAssert.Throws<MatchesException>(() => Assert.Matches(@"\w+", (string)null));
            }

            [Fact]
            public void 正規表現にマッチする_成功する()
            {
                Assert.Matches(@"\w", "Hello");
            }

            [Fact]
            public void 正規表現にマッチしない_失敗する()
            {
                var ex = Record.Exception(() => Assert.Matches(@"\d+", "Hello, world!"));

                XAssert.IsType<MatchesException>(ex);
                XAssert.Equal("Assert.Matches() Failure:" + Environment.NewLine + @"Regex: \d+" + Environment.NewLine + "Value: Hello, world!", ex.Message);
            }

            [Fact]
            public void 正規表現にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Matches((string)null, "Hello, world!"));
            }
        }

        public class Matches_WithRegex
        {
            [Fact]
            public void 正規表現にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.Matches((Regex)null, "Hello, world!"));
            }

            [Fact]
            public void 正規表現にマッチする_成功する()
            {
                Assert.Matches(new Regex(@"\w+"), "Hello");
            }

            [Fact]
            public void 正規表現オプションを使う()
            {
                Assert.Matches(new Regex(@"[a-z]+", RegexOptions.IgnoreCase), "HELLO");
            }

            [Fact]
            public void 正規表現にマッチしない_失敗する()
            {
                var ex = Record.Exception(() => Assert.Matches(new Regex(@"\d+"), "Hello, world!"));

                XAssert.IsType<MatchesException>(ex);
                XAssert.Equal("Assert.Matches() Failure:" + Environment.NewLine + @"Regex: \d+" + Environment.NewLine + "Value: Hello, world!", ex.Message);
            }

            [Fact]
            public void 実測値にnullを指定_失敗する()
            {
                XAssert.Throws<MatchesException>(() => Assert.Matches(new Regex(@"\w+"), (string)null));
            }
        }

        public class DoesNotMatch_WithString
        {
            [Fact]
            public void 正規表現にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.DoesNotMatch((string)null, "Hello, world!"));
            }

            [Fact]
            public void 正規表現にマッチしない_成功する()
            {
                Assert.DoesNotMatch(@"\d", "Hello");
            }

            [Fact]
            public void 正規表現にマッチする_失敗する()
            {
                var ex = Record.Exception(() => Assert.DoesNotMatch(@"\w", "Hello, world!"));

                XAssert.IsType<DoesNotMatchException>(ex);
                XAssert.Equal("Assert.DoesNotMatch() Failure:" + Environment.NewLine + @"Regex: \w" + Environment.NewLine + "Value: Hello, world!", ex.Message);
            }

            [Fact]
            public void 実測値にnullを指定_成功する()
            {
                Assert.DoesNotMatch(@"\w+", (string)null);
            }
        }

        public class DoesNotMatch_WithRegex
        {
            [Fact]
            public void 正規表現にnullを指定_ArgumentNullException例外を投げる()
            {
                XAssert.Throws<ArgumentNullException>(() => Assert.DoesNotMatch((Regex)null, "Hello, world!"));
            }

            [Fact]
            public void 正規表現にマッチしない_成功する()
            {
                Assert.DoesNotMatch(new Regex(@"\d"), "Hello");
            }

            [Fact]
            public void 正規表現にマッチする_失敗する()
            {
                var ex = Record.Exception(() => Assert.DoesNotMatch(new Regex(@"\w"), "Hello, world!"));

                XAssert.IsType<DoesNotMatchException>(ex);
                XAssert.Equal("Assert.DoesNotMatch() Failure:" + Environment.NewLine + @"Regex: \w" + Environment.NewLine + "Value: Hello, world!", ex.Message);
            }

            [Fact]
            public void 実測値にnullを指定_成功する()
            {
                Assert.DoesNotMatch(new Regex(@"\w+"), (string)null);
            }
        }
    }
}
