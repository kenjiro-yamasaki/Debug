using System;
using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// StartsWith アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.StartsWith"/> の失敗時に投げられます。
    /// </remarks>
    public class StartsWithException : AssertException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public StartsWithException(string expected, string actual)
            : base(string.Format(CultureInfo.CurrentCulture, "Assert.StartsWith() Failure:{2}Expected: {0}{2}Actual:   {1}", expected ?? "(null)", ShortenActual(expected, actual) ?? "(null)", Environment.NewLine))
        {
        }

        #endregion

        #region 静的メソッド

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

            return actual.Substring(0, expected.Length) + "...";
        }

        #endregion
    }
}