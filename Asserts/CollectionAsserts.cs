using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        #region All

        /// <summary>
        /// すべての項目が検査に合格することを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">項目コレクション。</param>
        /// <param name="inspector">検査。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/>が null である場合、投げられます。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="inspector"/>が null である場合、投げられます。</exception>
        /// <exception cref="AllException">検査に合格しない項目がある場合、投げられます。</exception>
        public static void All<TItem>(IEnumerable<TItem> collection, Action<TItem> inspector)
        {
            GuardArgumentNotNull(nameof(collection), collection);
            GuardArgumentNotNull(nameof(inspector), inspector);

            var errors = new Stack<(int Index, object Object, Exception Exception)>();
            var array  = collection.ToArray();

            for (var index = 0; index < array.Length; index++)
            {
                try
                {
                    inspector(array[index]);
                }
                catch (Exception ex)
                {
                    errors.Push((index, array[index], ex));
                }
            }

            if (0 < errors.Count)
            {
                throw new AllException(array.Length, errors.ToArray());
            }
        }

        #endregion

        #region Collection

        /// <summary>
        /// すべての項目が、それぞれの対応する検査に合格することを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">項目コレクション。</param>
        /// <param name="inspectors">検査コレクション (項目数と検査数は正確に一致しなくてはならない)。</param>
        /// <exception cref="CollectionException">検査に合格しない項目がある場合、投げられます。</exception>
        public static void Collection<TItem>(IEnumerable<TItem> collection, params Action<TItem>[] inspectors)
        {
            var items         = collection.ToArray();
            int expectedCount = inspectors.Length;
            int actualCount   = items.Length;

            if (expectedCount != actualCount)
            {
                throw new CollectionException(collection, expectedCount, actualCount);
            }

            for (int index = 0; index < actualCount; index++)
            {
                try
                {
                    inspectors[index](items[index]);
                }
                catch (Exception ex)
                {
                    throw new CollectionException(collection, expectedCount, actualCount, index, ex);
                }
            }
        }

        #endregion

        #region Contains

        /// <summary>
        /// 指定項目 (期待値) がコレクションに含まれることを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">コレクション。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/>がnullである場合、投げられます。</exception>
        /// <exception cref="ContainsException">指定項目 (期待値) を含まない場合、投げられます。</exception>
        public static void Contains<TItem>(TItem expected, IEnumerable<TItem> collection)
        {
            var icollection = collection as ICollection<TItem>;
            if (icollection != null && icollection.Contains(expected))
            {
                return;
            }

            Contains(expected, collection, GetEqualityComparer<TItem>());
        }

        /// <summary>
        /// 指定項目 (期待値) がコレクションに含まれることを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">コレクション。</param>
        /// <param name="comparer">等値比較子</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/>がnullである場合、投げられます。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/>がnullである場合、投げられます。</exception>
        /// <exception cref="ContainsException">指定項目(期待値)が含まれない場合、投げられます。</exception>
        public static void Contains<TItem>(TItem expected, IEnumerable<TItem> collection, IEqualityComparer<TItem> comparer)
        {
            GuardArgumentNotNull(nameof(collection), collection);
            GuardArgumentNotNull(nameof(comparer), comparer);

            if (collection.Contains(expected, comparer))
            {
                return;
            }

            throw new ContainsException(expected, collection);
        }

        /// <summary>
        /// フィルターに通る項目がコレクションに含まれることを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">コレクション。</param>
        /// <param name="filter">フィルター。</param>
        /// <exception cref="ContainsException">フィルターに通る項目が含まれない場合、投げられます。</exception>
        public static void Contains<TItem>(IEnumerable<TItem> collection, Predicate<TItem> filter)
        {
            GuardArgumentNotNull(nameof(collection), collection);
            GuardArgumentNotNull(nameof(filter), filter);

            foreach (var item in collection)
            {
                if (filter(item))
                {
                    return;
                }
            }

            throw new ContainsException("(filter expression)", collection);
        }

        /// <summary>
        /// 指定キー (期待値) が辞書に含まれることを検証します。
        /// </summary>
        /// <typeparam name="TKey">キーの型。</typeparam>
        /// <typeparam name="TValue">値の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">辞書。</param>
        /// <returns><paramref name="expected"/>に関連した値。</returns>
        /// <exception cref="ContainsException">指定キー (期待値) が含まれない場合、投げられます。</exception>
        public static TValue Contains<TKey, TValue>(TKey expected, IReadOnlyDictionary<TKey, TValue> collection)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(collection), collection);

            if (!collection.TryGetValue(expected, out var value))
            {
                throw new ContainsException(expected, collection.Keys);
            }

            return value;
        }

        /// <summary>
        /// 指定キー (期待値) が辞書に含まれることを検証します。
        /// </summary>
        /// <typeparam name="TKey">キーの型。</typeparam>
        /// <typeparam name="TValue">値の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">辞書。</param>
        /// <returns><paramref name="expected"/>に関連した値。</returns>
        /// <exception cref="ContainsException">指定キー (期待値) が含まれない場合、投げられます。</exception>
        public static TValue Contains<TKey, TValue>(TKey expected, IDictionary<TKey, TValue> collection)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(collection), collection);

            if (!collection.TryGetValue(expected, out var value))
            {
                throw new ContainsException(expected, collection.Keys);
            }

            return value;
        }

        #endregion

        #region DoesNotContain

        /// <summary>
        /// 指定項目 (期待値) がコレクションに含まれないことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">コレクション。</param>
        /// <exception cref="DoesNotContainException">指定項目 (期待値) を含まる場合、投げられます。</exception>
        public static void DoesNotContain<TItem>(TItem expected, IEnumerable<TItem> collection)
        {
            var icollection = collection as ICollection<TItem>;
            if (icollection != null && icollection.Contains(expected))
            {
                throw new DoesNotContainException(expected, collection);
            }

            DoesNotContain(expected, collection, GetEqualityComparer<TItem>());
        }

        /// <summary>
        /// 指定項目 (期待値) がコレクションに含まれないことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">コレクション。</param>
        /// <param name="comparer">等値比較子</param>
        /// <exception cref="DoesNotContainException">指定項目 (期待値) を含まる場合、投げられます。</exception>
        public static void DoesNotContain<TItem>(TItem expected, IEnumerable<TItem> collection, IEqualityComparer<TItem> comparer)
        {
            GuardArgumentNotNull(nameof(collection), collection);
            GuardArgumentNotNull(nameof(comparer), comparer);

            if (!collection.Contains(expected, comparer))
            {
                return;
            }

            throw new DoesNotContainException(expected, collection);
        }

        /// <summary>
        /// フィルターに通る項目がコレクションに含まれないことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">コレクション。</param>
        /// <param name="filter">フィルター。</param>
        /// <exception cref="DoesNotContainException">フィルターに通る項目が含まれる場合、投げられます。</exception>
        public static void DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> filter)
        {
            GuardArgumentNotNull(nameof(collection), collection);
            GuardArgumentNotNull(nameof(filter), filter);

            foreach (var item in collection)
            {
                if (filter(item))
                {
                    throw new DoesNotContainException("(filter expression)", collection);
                }
            }
        }

        /// <summary>
        /// 指定キー (期待値) が辞書に含まれないことを検証します。
        /// </summary>
        /// <typeparam name="TKey">キーの型。</typeparam>
        /// <typeparam name="TValue">値の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">辞書。</param>
        /// <exception cref="DoesNotContainException">指定キー (期待値) が含まれる場合、投げられます。</exception>
        public static void DoesNotContain<TKey, TValue>(TKey expected, IReadOnlyDictionary<TKey, TValue> collection)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(collection), collection);

            DoesNotContain(expected, collection.Keys);
        }

        /// <summary>
        /// 指定キー (期待値) が辞書に含まれないことを検証します。
        /// </summary>
        /// <typeparam name="TKey">キーの型。</typeparam>
        /// <typeparam name="TValue">値の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="collection">辞書。</param>
        /// <exception cref="DoesNotContainException">指定キー (期待値) が含まれる場合、投げられます。</exception>
        public static void DoesNotContain<TKey, TValue>(TKey expected, IDictionary<TKey, TValue> collection)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(collection), collection);

            DoesNotContain(expected, collection.Keys);
        }

        #endregion

        #region Empty

        /// <summary>
        /// コレクションが空であることを検証します。
        /// </summary>
        /// <param name="collection">コレクション。</param>
        /// <exception cref="ArgumentNullException">コレクションが null の場合、投げられます。</exception>
        /// <exception cref="EmptyException">コレクションが空の場合、投げられます。</exception>
        public static void Empty(IEnumerable collection)
        {
            GuardArgumentNotNull(nameof(collection), collection);

            var enumerator = collection.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    throw new EmptyException(collection);
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        #endregion

        #region Equal

        /// <summary>
        /// コレクションの項目数と各項目が等しいことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="EqualException">コレクションの項目数と各項目が等しくない場合、投げられます。</exception>
        public static void Equal<TItem>(IEnumerable<TItem> expected, IEnumerable<TItem> actual)
        {
            Equal(expected, actual, GetEqualityComparer<IEnumerable<TItem>>());
        }

        /// <summary>
        /// コレクションの項目数と各項目が等しいことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="comparer">等値比較子</param>
        /// <exception cref="EqualException">コレクションの項目数と各項目が等しくない場合、投げられます。</exception>
        public static void Equal<TItem>(IEnumerable<TItem> expected, IEnumerable<TItem> actual, IEqualityComparer<TItem> comparer)
        {
            Equal(expected, actual, GetEqualityComparer<IEnumerable<TItem>>(new AssertEqualityComparerAdapter<TItem>(comparer)));
        }

        #endregion

        #region NotEmpty

        /// <summary>
        /// コレクションが空ではないことを検証します。
        /// </summary>
        /// <param name="collection">コレクション。</param>
        /// <exception cref="ArgumentNullException">コレクションが null の場合、投げられます。</exception>
        /// <exception cref="NotEmptyException">コレクションが空である場合、投げられます。</exception>
        public static void NotEmpty(IEnumerable collection)
        {
            GuardArgumentNotNull("collection", collection);

            var enumerator = collection.GetEnumerator();
            try
            {
                if (!enumerator.MoveNext())
                {
                    throw new NotEmptyException();
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        #endregion

        #region NotEqual

        /// <summary>
        /// コレクションの項目数と各項目が等しくないことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="NotEqualException">コレクションの項目数と各項目が等しい場合、投げられます。</exception>
        public static void NotEqual<TItem>(IEnumerable<TItem> expected, IEnumerable<TItem> actual)
        {
            NotEqual(expected, actual, GetEqualityComparer<IEnumerable<TItem>>());
        }

        /// <summary>
        /// コレクションの項目数と各項目が等しくないことを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="comparer">等値比較子</param>
        /// <exception cref="NotEqualException">コレクションの項目数と各項目が等しい場合、投げられます。</exception>
        public static void NotEqual<TItem>(IEnumerable<TItem> expected, IEnumerable<TItem> actual, IEqualityComparer<TItem> comparer)
        {
            NotEqual(expected, actual, GetEqualityComparer<IEnumerable<TItem>>(new AssertEqualityComparerAdapter<TItem>(comparer)));
        }

        #endregion

        #region Single

        /// <summary>
        /// コレクションに項目が一つだけ含まれることを検証します。
        /// </summary>
        /// <param name="collection">コレクション。</param>
        /// <returns>コレクションの唯一の項目。</returns>
        /// <exception cref="SingleException">項目が一つではない場合、投げられます。</exception>
        public static object Single(IEnumerable collection)
        {
            return Single(collection.Cast<object>());
        }

        /// <summary>
        /// 指定項目 (期待値) がコレクションに一つだけ含まれることを検証します。
        /// </summary>
        /// <param name="collection">コレクション。</param>
        /// <param name="expected">期待値。</param>
        /// <exception cref="SingleException">項目が一つではない場合、投げられます。</exception>
        public static void Single(IEnumerable collection, object expected)
        {
            GuardArgumentNotNull("collection", collection);

            var (_, exception) = GetSingleResult(collection.Cast<object>(), item => object.Equals(item, expected), ArgumentFormatter.Format(expected));

            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// コレクションに項目が一つだけ含まれることを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">コレクション。</param>
        /// <returns>コレクションの唯一の項目。</returns>
        /// <exception cref="SingleException">項目が一つではない場合、投げられます。</exception>
        public static TItem Single<TItem>(IEnumerable<TItem> collection)
        {
            GuardArgumentNotNull("collection", collection);

            var (result, exception) = GetSingleResult(collection, null, null);

            if (exception != null)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// 指定述語にマッチする項目がコレクションに一つだけ含まれることを検証します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">コレクション。</param>
        /// <param name="predicate">述語。</param>
        /// <returns>指定述語にマッチする項目。</returns>
        /// <exception cref="SingleException">指定述語にマッチする項目が一つではない場合、投げられます。</exception>
        public static TItem Single<TItem>(IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            GuardArgumentNotNull(nameof(collection), collection);
            GuardArgumentNotNull(nameof(predicate), predicate);

            var (result, exception) = GetSingleResult(collection, predicate, "(filter expression)");

            if (exception != null)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// 指定述語にマッチする Single の検索結果を取得します。
        /// </summary>
        /// <typeparam name="TItem">項目の型。</typeparam>
        /// <param name="collection">コレクション。</param>
        /// <param name="predicate">述語。</param>
        /// <param name="expected">期待値。</param>
        /// <returns>最初に述語にマッチした項目, 投げるべき例外。</returns>
        private static (TItem Item, SingleException Exception) GetSingleResult<TItem>(IEnumerable<TItem> collection, Predicate<TItem> predicate, string expected)
        {
            int count = 0;
            var result = default(TItem);

            foreach (var item in collection)
            {
                if (predicate == null || predicate(item))
                {
                    if (++count == 1)
                    {
                        result = item;
                    }
                }
            }

            switch (count)
            {
                case 0:
                    return (result , new SingleException(expected));

                case 1:
                    return (result , null);

                default:
                    return (result, new SingleException(count, expected));
            }
        }

        #endregion

        #endregion
    }
}
