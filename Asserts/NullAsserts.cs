using System.Diagnostics;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        /// <summary>
        /// オブジェクト参照が null ではないことを検証します。
        /// </summary>
        /// <param name="object">オブジェクト。</param>
        /// <exception cref="NotNullException">オブジェクト参照が null である場合、投げられます。</exception>
        public static void NotNull(object @object)
        {
            if (@object == null)
            {
                throw new NotNullException();
            }
        }

        /// <summary>
        /// オブジェクト参照が null であることを検証します。
        /// </summary>
        /// <param name="object">オブジェクト。</param>
        /// <exception cref="NullException">オブジェクト参照が null ではない場合、投げられます。</exception>
        public static void Null(object @object)
        {
            if (@object != null)
            {
                throw new NullException(@object);
            }
        }

        #endregion
    }
}
