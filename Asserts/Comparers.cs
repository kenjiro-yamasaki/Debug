using System;
using System.Collections;
using System.Collections.Generic;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        /// <summary>
        /// 比較子を取得します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="itemEaualityComparer">項目の比較子 (比較対象のオブジェクトが反復子である場合、各項目の比較子に使用される)。</param>
        /// <returns>等値比較子</returns>
        private static IComparer<T> GetComparer<T>() where T : IComparable
        {
            return new AssertComparer<T>();
        }

        /// <summary>
        /// 等値比較子を取得します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="itemEaualityComparer">項目の等値比較子 (比較対象のオブジェクトが反復子である場合、各項目の等値比較子に使用される)。</param>
        /// <returns>等値比較子</returns>
        private static IEqualityComparer<T> GetEqualityComparer<T>(IEqualityComparer innerComparer = null)
        {
            return new AssertEqualityComparer<T>(innerComparer);
        }

        #endregion
    }
}