using System;
using System.Collections;
using System.Collections.Generic;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサートの等値比較子アダプター。
    /// </summary>
    /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
    internal class AssertEqualityComparerAdapter<T> : IEqualityComparer
    {
        #region プロパティ

        /// <summary>
        /// 内部等値比較子。
        /// </summary>
        private IEqualityComparer<T> InnerComparer { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="innerComparer">アダプトされる内部等値比較子</param>
        public AssertEqualityComparerAdapter(IEqualityComparer<T> innerComparer)
        {
            this.InnerComparer = innerComparer;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定したオブジェクトが等しいかを判断します。
        /// </summary>
        /// <param name="x">比較対象のオブジェクト。</param>
        /// <param name="y">比較対象のオブジェクト。</param>
        /// <returns>指定したオブジェクトが等しいか</returns>
        public new bool Equals(object x, object y)
        {
            return InnerComparer.Equals((T)x, (T)y);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト。</param>
        /// <returns>ハッシュコード</returns>
        /// <remarks>
        /// このクラスはGetHashCodeを実装しない。
        /// このクラスをハッシュコンテナーに使用しないこと。
        /// </remarks>
        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}