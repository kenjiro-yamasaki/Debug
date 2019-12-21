using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// ProperSubset アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.ProperSubset"/> の失敗時に投げられます。
    /// </remarks>
    public class ProperSubsetException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public ProperSubsetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.ProperSubset() Failure")
        {
        }

        #endregion
    }
}
