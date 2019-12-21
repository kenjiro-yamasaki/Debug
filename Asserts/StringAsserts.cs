using System;
using System.Text.RegularExpressions;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        #region Contains

        /// <summary>
        /// 文字列が部分文字列を含むことを検証します。
        /// </summary>
        /// <param name="expectedSubstring">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="ContainsException">文字列が部分文字列を含まない場合、投げられます。</exception>
        public static void Contains(string expectedSubstring, string actualString)
        {
            Contains(expectedSubstring, actualString, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 文字列が部分文字列を含むことを検証します。
        /// </summary>
        /// <param name="expectedSubstring">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <param name="stringComparison">文字列比較</param>
        /// <exception cref="ContainsException">文字列が部分文字列を含まない場合、投げられます。</exception>
        public static void Contains(string expectedSubstring, string actualString, StringComparison stringComparison)
        {
            GuardArgumentNotNull(nameof(expectedSubstring), expectedSubstring);

            if (actualString == null || actualString.IndexOf(expectedSubstring, stringComparison) < 0)
            {
                throw new ContainsException(expectedSubstring, actualString);
            }
        }

        /// <summary>
        /// 文字列が部分文字列を含まないことを検証します。
        /// </summary>
        /// <param name="expectedSubstring">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="DoesNotContainException">文字列が部分文字列を含む場合、投げられます。</exception>
        public static void DoesNotContain(string expectedSubstring, string actualString)
        {
            DoesNotContain(expectedSubstring, actualString, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 文字列が部分文字列を含まないことを検証します。
        /// </summary>
        /// <param name="expectedSubstring">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <param name="stringComparison">文字列比較</param>
        /// <exception cref="DoesNotContainException">文字列が部分文字列を含む場合、投げられます。</exception>
        public static void DoesNotContain(string expectedSubstring, string actualString, StringComparison stringComparison)
        {
            GuardArgumentNotNull(nameof(expectedSubstring), expectedSubstring);

            if (actualString != null && actualString.IndexOf(expectedSubstring, stringComparison) >= 0)
            {
                throw new DoesNotContainException(expectedSubstring, actualString);
            }
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// 文字列が部分文字列から始まることを検証します。
        /// </summary>
        /// <param name="expectedStartString">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="StartsWithException">文字列が部分文字列から始まらない場合、投げられます。</exception>
        public static void StartsWith(string expectedStartString, string actualString)
        {
            StartsWith(expectedStartString, actualString, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 文字列が部分文字列から始まることを検証します。
        /// </summary>
        /// <param name="expectedStartString">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <param name="stringComparison">文字列比較</param>
        /// <exception cref="StartsWithException">文字列が部分文字列から始まらない場合、投げられます。</exception>
        public static void StartsWith(string expectedStartString, string actualString, StringComparison stringComparison)
        {
            GuardArgumentNotNull(nameof(expectedStartString), expectedStartString);

            if (actualString == null || !actualString.StartsWith(expectedStartString, stringComparison))
            {
                throw new StartsWithException(expectedStartString, actualString);
            }
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// 文字列が部分文字列で終わることを検証します。
        /// </summary>
        /// <param name="expectedEndString">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="EndsWithException">文字列が部分文字列で終わらない場合、投げられます。</exception>
        public static void EndsWith(string expectedEndString, string actualString)
        {
            EndsWith(expectedEndString, actualString, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 文字列が部分文字列で終わることを検証します。
        /// </summary>
        /// <param name="expectedEndString">期待値 (部分文字列)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <param name="stringComparison">文字列比較</param>
        /// <exception cref="EndsWithException">文字列が部分文字列で終わらない場合、投げられます。</exception>
        public static void EndsWith(string expectedEndString, string actualString, StringComparison stringComparison)
        {
            GuardArgumentNotNull(nameof(expectedEndString), expectedEndString);

            if (actualString == null || !actualString.EndsWith(expectedEndString, stringComparison))
            {
                throw new EndsWithException(expectedEndString, actualString);
            }
        }

        #endregion

        #region Matches

        /// <summary>
        /// 文字列が正規表現にマッチすることを検証します。
        /// </summary>
        /// <param name="expectedRegexPattern">期待値 (正規表現)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="MatchesException">文字列が正規表現にマッチしない場合、投げられます。</exception>
        public static void Matches(string expectedRegexPattern, string actualString)
        {
            GuardArgumentNotNull(nameof(expectedRegexPattern), expectedRegexPattern);

            if (actualString == null || !Regex.IsMatch(actualString, expectedRegexPattern))
            {
                throw new MatchesException(expectedRegexPattern, actualString);
            }
        }

        /// <summary>
        /// 文字列が正規表現にマッチすることを検証します。
        /// </summary>
        /// <param name="expectedRegexPattern">期待値 (正規表現)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="MatchesException">文字列が正規表現にマッチしない場合、投げられます。</exception>
        public static void Matches(Regex expectedRegex, string actualString)
        {
            GuardArgumentNotNull(nameof(expectedRegex), expectedRegex);

            if (actualString == null || !expectedRegex.IsMatch(actualString))
            {
                throw new MatchesException(expectedRegex.ToString(), actualString);
            }
        }

        /// <summary>
        /// 文字列が正規表現にマッチしないことを検証します。
        /// </summary>
        /// <param name="expectedRegexPattern">期待値 (正規表現)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="DoesNotMatchException">文字列が正規表現にマッチする場合、投げられます。</exception>
        public static void DoesNotMatch(string expectedRegexPattern, string actualString)
        {
            GuardArgumentNotNull(nameof(expectedRegexPattern), expectedRegexPattern);

            if (actualString != null && Regex.IsMatch(actualString, expectedRegexPattern))
            {
                throw new DoesNotMatchException(expectedRegexPattern, actualString);
            }
        }

        /// <summary>
        /// 文字列が正規表現にマッチしないことを検証します。
        /// </summary>
        /// <param name="expectedRegexPattern">期待値 (正規表現)。</param>
        /// <param name="actualString">実測値 (文字列)。</param>
        /// <exception cref="DoesNotMatchException">文字列が正規表現にマッチする場合、投げられます。</exception>
        public static void DoesNotMatch(Regex expectedRegex, string actualString)
        {
            GuardArgumentNotNull(nameof(expectedRegex), expectedRegex);

            if (actualString != null && expectedRegex.IsMatch(actualString))
            {
                throw new DoesNotMatchException(expectedRegex.ToString(), actualString);
            }
        }

        #endregion

        #region Equal

        /// <summary>
        /// 文字列が等しいことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="EqualException">文字列が等しくない場合、投げられます。</exception>
        public static void Equal(string expected, string actual)
        {
            Equal(expected, actual, false, false, false);
        }

        /// <summary>
        /// 文字列が等しいことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="ignoreCase">大文字・小文字の違いを無視するか</param>
        /// <param name="ignoreLineEndingDifferences">改行文字 (\r\n、\r、\n) の違いを無視するか</param>
        /// <param name="ignoreWhiteSpaceDifferences">空白文字 (タブとスペース) の違いを無視するか</param>
        /// <exception cref="EqualException">文字列が等しくない場合、投げられます。</exception>
        public static void Equal(string expected, string actual, bool ignoreCase = false, bool ignoreLineEndingDifferences = false, bool ignoreWhiteSpaceDifferences = false)
        {
            int expectedIndex  = -1;
            int actualIndex    = -1;
            int expectedLength = 0;
            int actualLength   = 0;

            if (expected == null)
            {
                if (actual == null)
                {
                    return;
                }
            }
            else if (actual != null)
            {
                expectedIndex  = 0;
                actualIndex    = 0;
                expectedLength = expected.Length;
                actualLength   = actual.Length;

                while (expectedIndex < expectedLength && actualIndex < actualLength)
                {
                    char expectedChar = expected[expectedIndex];
                    char actualChar   = actual[actualIndex];

                    if (ignoreLineEndingDifferences && IsLineEnding(expectedChar) && IsLineEnding(actualChar))
                    {
                        expectedIndex = SkipLineEnding(expected, expectedIndex);
                        actualIndex   = SkipLineEnding(actual, actualIndex);
                    }
                    else if (ignoreWhiteSpaceDifferences && IsWhiteSpace(expectedChar) && IsWhiteSpace(actualChar))
                    {
                        expectedIndex = SkipWhitespace(expected, expectedIndex);
                        actualIndex   = SkipWhitespace(actual, actualIndex);
                    }
                    else
                    {
                        if (ignoreCase)
                        {
                            expectedChar = char.ToUpperInvariant(expectedChar);
                            actualChar   = char.ToUpperInvariant(actualChar);
                        }

                        if (expectedChar != actualChar)
                        {
                            break;
                        }

                        expectedIndex++;
                        actualIndex++;
                    }
                }
            }

            if (expectedIndex < expectedLength || actualIndex < actualLength)
            {
                throw new EqualException(expected, actual, expectedIndex, actualIndex);
            }
        }

        /// <summary>
        /// 指定文字が改行文字かを判断します。
        /// </summary>
        /// <param name="char">文字。</param>
        /// <returns>指定文字が改行文字かを示す値。</returns>
        private static bool IsLineEnding(char @char)
        {
            return @char == '\r' || @char == '\n';
        }

        /// <summary>
        /// 指定文字が空白文字かを判断します。
        /// </summary>
        /// <param name="char">文字。</param>
        /// <returns>指定文字が空白文字かを示す値。</returns>
        private static bool IsWhiteSpace(char @char)
        {
            return @char == ' ' || @char == '\t';
        }

        /// <summary>
        /// 改行文字をスキップします。
        /// </summary>
        /// <param name="value">文字列。</param>
        /// <param name="index">文字列インデックス。</param>
        /// <returns>改行文字をスキップした文字列インデックス。</returns>
        private static int SkipLineEnding(string value, int index)
        {
            if (value[index] == '\r')
            {
                ++index;
            }
            if (index < value.Length && value[index] == '\n')
            {
                ++index;
            }

            return index;
        }

        /// <summary>
        /// 空白文字をスキップします。
        /// </summary>
        /// <param name="value">文字列。</param>
        /// <param name="index">文字列インデックス。</param>
        /// <returns>空白文字をスキップした文字列インデックス。</returns>
        private static int SkipWhitespace(string value, int index)
        {
            while (index < value.Length)
            {
                switch (value[index])
                {
                    case ' ':
                    case '\t':
                        index++;
                        break;

                    default:
                        return index;
                }
            }

            return index;
        }

        #endregion

        #endregion
    }
}