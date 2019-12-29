namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region メソッド

        /// <summary>
        /// オブジェクト参照が <c>null</c> ではないことを検証します。
        /// </summary>
        /// <param name="object">オブジェクト参照。</param>
        /// <exception cref="NotNullException">オブジェクト参照が <c>null</c> である場合、投げられます。</exception>
        public static void NotNull(object @object)
        {
            if (@object == null)
            {
                throw new NotNullException();
            }
        }

        /// <summary>
        /// オブジェクト参照が <c>null</c> であることを検証します。
        /// </summary>
        /// <param name="object">オブジェクト参照。</param>
        /// <exception cref="NullException">オブジェクト参照が <c>null</c> ではない場合、投げられます。</exception>
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
