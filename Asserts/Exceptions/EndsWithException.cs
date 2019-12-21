using System;
using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// EndsWith アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.EndsWith"/> の失敗時に投げられます。
    /// </remarks>
    public class EndsWithException : AssertException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public EndsWithException(string expected, string actual)
            : base(string.Format(CultureInfo.CurrentCulture, "Assert.EndsWith() Failure:{2}Expected: {0}{2}Actual:   {1}", ShortenExpected(expected, actual) ?? "(null)", ShortenActual(expected, actual) ?? "(null)", Environment.NewLine))
        {
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 期待値を省略します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <returns>省略された期待値。</returns>
        private static string ShortenExpected(string expected, string actual)
        {
            if (expected == null || actual == null || actual.Length <= expected.Length)
            {
                return expected;
            }

            return "   " + expected;
        }

        /// <summary>
        /// 実測値を省略します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <returns>省略された実測値。</returns>
        private static string ShortenActual(string expected, string actual)
        {
            if (expected == null || actual == null || actual.Length <= expected.Length)
            {
                return actual;
            }

            return "···" + actual.Substring(actual.Length - expected.Length);
        }

        #endregion
    }
}