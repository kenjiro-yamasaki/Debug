namespace SoftCube.Asserts
{
    /// <summary>
    /// DoesNotContain アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.DoesNotContain"/> の失敗時に投げられます。
    /// </remarks>
    public class DoesNotContainException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public DoesNotContainException(object expected, object actual)
            : base(expected, actual, "Assert.DoesNotContain() Failure", "Found", "In value")
        {
        }

        #endregion
    }
}