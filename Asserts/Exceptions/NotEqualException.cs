namespace SoftCube.Asserts
{
    /// <summary>
    /// NotEqual アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.NotEqual"/> の失敗時に投げられます。
    /// </remarks>
    public class NotEqualException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public NotEqualException(string expected, string actual)
            : base("Not " + expected, actual, "Assert.NotEqual() Failure")
        {
        }

        #endregion
    }
}
