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
        /// 集合がほかの集合の真部分集合であることを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象の項目の型。</typeparam>
        /// <param name="expectedSuperset">期待値 (真上位集合)。</param>
        /// <param name="actual">実測値 (真部分集合)。</param>
        /// <exception cref="ProperSubsetException">実測値が期待値の真部分集合ではない場合、投げられます。</exception>
        public static void ProperSubset<T>(ISet<T> expectedSuperset, ISet<T> actual)
        {
            GuardArgumentNotNull(nameof(expectedSuperset), expectedSuperset);

            if (actual == null || !actual.IsProperSubsetOf(expectedSuperset))
            {
                throw new ProperSubsetException(expectedSuperset, actual);
            }
        }

        /// <summary>
        /// 集合がほかの集合の真上位集合であることを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象の項目の型。</typeparam>
        /// <param name="expectedSubset">期待値 (真部分集合)。</param>
        /// <param name="actual">実測値 (真上位集合)。</param>
        /// <exception cref="ProperSupersetException">実測値が期待値の真上位集合ではない場合、投げられます。</exception>
        public static void ProperSuperset<T>(ISet<T> expectedSubset, ISet<T> actual)
        {
            GuardArgumentNotNull(nameof(expectedSubset), expectedSubset);

            if (actual == null || !actual.IsProperSupersetOf(expectedSubset))
            {
                throw new ProperSupersetException(expectedSubset, actual);
            }
        }

        /// <summary>
        /// 集合がほかの集合の部分集合であることを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象の項目の型。</typeparam>
        /// <param name="expectedSuperset">期待値 (上位集合)。</param>
        /// <param name="actual">実測値 (部分集合)。</param>
        /// <exception cref="SubsetException">実測値が期待値の部分集合ではない場合、投げられます。</exception>
        public static void Subset<T>(ISet<T> expectedSuperset, ISet<T> actual)
        {
            GuardArgumentNotNull(nameof(expectedSuperset), expectedSuperset);

            if (actual == null || !actual.IsSubsetOf(expectedSuperset))
            {
                throw new SubsetException(expectedSuperset, actual);
            }
        }

        /// <summary>
        /// 集合がほかの集合の上位集合であることを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象の項目の型。</typeparam>
        /// <param name="expectedSubset">期待値 (部分集合)。</param>
        /// <param name="actual">実測値 (上位集合)。</param>
        /// <exception cref="ProperSupersetException">実測値が期待値の上位集合ではない場合、投げられます。</exception>
        public static void Superset<T>(ISet<T> expectedSubset, ISet<T> actual)
        {
            GuardArgumentNotNull(nameof(expectedSubset), expectedSubset);

            if (actual == null || !actual.IsSupersetOf(expectedSubset))
            {
                throw new SupersetException(expectedSubset, actual);
            }
        }

        #endregion
    }
}
