namespace SoftCube.Asserts
{
    /// <summary>
    /// NotNull アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.NotNull"/> の失敗時に投げられます。
    /// </remarks>
    public class NotNullException : AssertException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public NotNullException()
            : base("Assert.NotNull() Failure")
        {
        }

        #endregion
    }
}