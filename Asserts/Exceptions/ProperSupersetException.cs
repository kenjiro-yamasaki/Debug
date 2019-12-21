using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// ProperSuperset アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.ProperSuperset"/> の失敗時に投げられます。
    /// </remarks>
    public class ProperSupersetException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public ProperSupersetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.ProperSuperset() Failure")
        {
        }

        #endregion
    }
}
