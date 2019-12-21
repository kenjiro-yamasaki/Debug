using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Subset アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.Subset"/> の失敗時に投げられます。
    /// </remarks>
    public class SubsetException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public SubsetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.Subset() Failure")
        {
        }

        #endregion
    }
}