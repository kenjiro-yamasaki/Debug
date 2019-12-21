namespace SoftCube.Asserts
{
    /// <summary>
    /// True アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.True"/> の失敗時に投げられます。
    /// </remarks>
    public class TrueException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="acutual">実測値。</param>
        /// <param name="message">メッセージ。</param>
        public TrueException(bool? acutual, string message)
            : base("True", acutual == null ? "(null)" : acutual.ToString(), message ?? "Assert.True() Failure")
        {
        }

        #endregion
    }
}