using System;
using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// DoesNotMatch アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.DoesNotMatch"/> の失敗時に投げられます。
    /// </remarks>
    public class DoesNotMatchException : AssertException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expectedRegexPattern">期待値 (正規表現)。</param>
        /// <param name="actual">実測値。</param>
        public DoesNotMatchException(object expectedRegexPattern, object actual)
            : base(string.Format(CultureInfo.CurrentCulture, "Assert.DoesNotMatch() Failure:{2}Regex: {0}{2}Value: {1}", expectedRegexPattern, actual, Environment.NewLine))
        {
        }

        #endregion
    }
}
