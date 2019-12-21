using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Superset アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.Superset"/> の失敗時に投げられます。
    /// </remarks>
    public class SupersetException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public SupersetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.Superset() Failure")
        {
        }

        #endregion
    }
}
