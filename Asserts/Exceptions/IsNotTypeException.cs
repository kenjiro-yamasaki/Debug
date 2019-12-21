using System;

namespace SoftCube.Asserts
{
    /// <summary>
    /// IsNotType アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.IsNotType"/> の失敗時に投げられます。
    /// </remarks>
    public class IsNotTypeException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expectedTypeName">期待値 (型)。</param>
        /// <param name="actualTypeName">実測値。</param>
        public IsNotTypeException(Type expected, object actual)
            : base(expected, actual == null ? null : actual.GetType(), "Assert.IsNotType() Failure")
        {
        }

        #endregion
    }
}
