namespace SoftCube.Asserts
{
    /// <summary>
    /// Same アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.Same"/> の失敗時に投げられます。
    /// </remarks>
    public class SameException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public SameException(object expected, object actual)
            : base(expected, actual, "Assert.Same() Failure")
        {
        }

        #endregion
    }
}
