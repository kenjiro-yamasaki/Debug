namespace SoftCube.Asserts
{
    /// <summary>
    /// Single アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.Single"/> の失敗時に投げられます。
    /// </remarks>
    public class SingleException : AssertException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター (コレクションが期待値を含まない場合)。
        /// </summary>
        /// <param name="expected">期待値。</param>
        public SingleException(string expected)
            : base("The collection was expected to contain a single element" + (expected == null ? "" : " matching " + expected) + ", but it " + (expected == null ? "was empty." : "contained no matching elements."))
        {
        }

        /// <summary>
        /// コンストラクター (コレクションが期待値を複数含む場合)。
        /// </summary>
        /// <param name="count">コレクションに含まれる期待値の数。</param>
        /// <param name="expected">期待値。</param>
        public SingleException(int count, string expected)
            : base("The collection was expected to contain a single element" + (expected == null ? "" : " matching " + expected) + ", but it contained " + count + " " + (expected == null ? "" : "matching ") + "elements.")
        {
        }

        #endregion
    }
}
