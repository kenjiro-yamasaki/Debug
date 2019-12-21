using System;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        /// <summary>
        /// 引数が null ではないことを保証します。
        /// </summary>
        /// <param name="argName">引数の名前。</param>
        /// <param name="argValue">引数の値。</param>
        private static void GuardArgumentNotNull(string argName, object argValue)
        {
            if (argValue == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        #endregion
    }
}
