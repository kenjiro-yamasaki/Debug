using System;
using System.Collections.Generic;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        #region InRange

        /// <summary>
        /// 実測値が指定範囲内かを検証します。
        /// </summary>
        /// <typeparam name="T">値の型。</typeparam>
        /// <param name="actual">実測値。</param>
        /// <param name="low">下限値 (範囲は下限値を含む)。</param>
        /// <param name="high">上限値 (範囲は上限値を含む)。</param>
        /// <exception cref="InRangeException">実測値が指定範囲外である場合、投げられます。</exception>
        public static void InRange<T>(T actual, T low, T high)
            where T : IComparable
        {
            InRange(actual, low, high, GetComparer<T>());
        }

        /// <summary>
        /// 実測値が指定範囲内かを検証します。
        /// </summary>
        /// <typeparam name="T">値の型。</typeparam>
        /// <param name="actual">実測値。</param>
        /// <param name="low">下限値 (範囲は下限値を含む)。</param>
        /// <param name="high">上限値 (範囲は上限値を含む)。</param>
        /// <param name="comparer">比較子</param>
        /// <exception cref="InRangeException">実測値が指定範囲外である場合、投げられます。</exception>
        public static void InRange<T>(T actual, T low, T high, IComparer<T> comparer)
        {
            GuardArgumentNotNull("comparer", comparer);

            if (comparer.Compare(low, actual) > 0 || comparer.Compare(actual, high) > 0)
            {
                throw new InRangeException(actual, low, high);
            }
        }

        #endregion

        #region NotInRange

        /// <summary>
        /// 実測値が指定範囲外かを検証します。
        /// </summary>
        /// <typeparam name="T">値の型。</typeparam>
        /// <param name="actual">実測値。</param>
        /// <param name="low">下限値 (範囲は下限値を含む)。</param>
        /// <param name="high">上限値 (範囲は上限値を含む)。</param>
        /// <exception cref="NotInRangeException">実測値が指定範囲内である場合、投げられます。</exception>
        public static void NotInRange<T>(T actual, T low, T high)
            where T : IComparable
        {
            NotInRange(actual, low, high, GetComparer<T>());
        }

        /// <summary>
        /// 実測値が指定範囲外かを検証します。
        /// </summary>
        /// <typeparam name="T">値の型。</typeparam>
        /// <param name="actual">実測値。</param>
        /// <param name="low">下限値 (範囲は下限値を含む)。</param>
        /// <param name="high">上限値 (範囲は上限値を含む)。</param>
        /// <param name="comparer">比較子</param>
        /// <exception cref="NotInRangeException">実測値が指定範囲内である場合、投げられます。</exception>
        public static void NotInRange<T>(T actual, T low, T high, IComparer<T> comparer)
        {
            GuardArgumentNotNull("comparer", comparer);

            if (comparer.Compare(low, actual) <= 0 && comparer.Compare(actual, high) <= 0)
            {
                throw new NotInRangeException(actual, low, high);
            }
        }

        #endregion

        #endregion
    }
}