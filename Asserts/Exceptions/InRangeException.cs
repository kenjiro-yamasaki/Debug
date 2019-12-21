using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// InRange アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.InRange"/> の失敗時に投げられます。
    /// </remarks>
    public class InRangeException : AssertException
    {
        #region プロジェクト

        /// <summary>
        /// 実測値。
        /// </summary>
        public string Actual { get; }

        /// <summary>
        /// 上限値。
        /// </summary>
        public string High { get; }

        /// <summary>
        /// 下限値。
        /// </summary>
        public string Low { get; }

        /// <summary>
        /// メッセージ。
        /// </summary>
        public override string Message => string.Format(CultureInfo.CurrentCulture, "{0}\r\nRange:  ({1} - {2})\r\nActual: {3}", base.Message, Low, High, Actual ?? "(null)");

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="actual">実測値。</param>
        /// <param name="low">下限値。</param>
        /// <param name="high">上限値。</param>
        public InRangeException(object actual, object low, object high)
            : base("Assert.InRange() Failure")
        {
            Actual = actual == null ? null : actual.ToString();
            Low    = low == null ? null : low.ToString();
            High   = high == null ? null : high.ToString();
        }

        #endregion
    }
}
