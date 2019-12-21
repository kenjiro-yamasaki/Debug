using System;

namespace SoftCube.Asserts
{
    /// <summary>
    /// IsAssignableFrom アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.IsAssignableFrom"/> の失敗時に投げられます。
    /// </remarks>
    public class IsAssignableFromException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public IsAssignableFromException(Type expected, object actual)
            : base(expected, actual == null ? null : actual.GetType(), "Assert.IsAssignableFrom() Failure")
        {
        }

        #endregion
    }
}
