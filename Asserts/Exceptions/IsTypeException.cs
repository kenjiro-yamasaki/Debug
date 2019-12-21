namespace SoftCube.Asserts
{
    /// <summary>
    /// IsType アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.IsType"/> の失敗時に投げられます。
    /// </remarks>
    public class IsTypeException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expectedTypeName">期待値 (型名)。</param>
        /// <param name="actualTypeName">実測値 (型名)。</param>
        public IsTypeException(string expectedTypeName, string actualTypeName)
            : base(expectedTypeName, actualTypeName, "Assert.IsType() Failure")
        {
        }

        #endregion
    }
}