using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサートのデフォルト比較子。
    /// </summary>
    /// <typeparam name="T">比較対象の型。</typeparam>
    internal class AssertComparer<T> : IComparer<T>
        where T : IComparable
    {
        #region 定数

        /// <summary>
        /// <see cref="Nullable<>"/> の型情報。
        /// </summary>
        static readonly TypeInfo NullableTypeInfo = typeof(Nullable<>).GetTypeInfo();

        #endregion

        #region メソッド

        /// <summary>
        /// 指定したオブジェクトの大小関係を比較します。
        /// </summary>
        /// <param name="x">比較対象のオブジェクト。</param>
        /// <param name="y">比較対象のオブジェクト。</param>
        /// <returns>
        /// x < y ⇒ 0 より小さい値。
        /// x = y ⇒ 0。
        /// x > y ⇒ 0 より大きい値。
        /// </returns>
        /// <remarks>
        /// <c>null</c> と参照型オブジェクトとの比較は許容されています。例外は投げられません。
        /// <c>null</c> は、<c>null</c> 以外よりも小さいと見なします。
        /// </remarks>
        public int Compare(T x, T y)
        {
            var typeInfo = typeof(T).GetTypeInfo();

            // null と比較します。
            if (!typeInfo.IsValueType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().GetTypeInfo().IsAssignableFrom(NullableTypeInfo)))
            {
                if (Equals(x, default(T)))
                {
                    if (Equals(y, default(T)))
                    {
                        return 0;
                    }

                    return -1;
                }

                if (Equals(y, default(T)))
                {
                    return -1;
                }
            }

            // 型を比較します。
            if (x.GetType() != y.GetType())
            {
                return -1;
            }

            // 値を比較します。
            if (x is IComparable<T> comparable)
            {
                return comparable.CompareTo(y);
            }
            else
            {
                return x.CompareTo(y);
            }
        }

        #endregion
    }
}