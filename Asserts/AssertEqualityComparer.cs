using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサートのデフォルト等値比較子。
    /// </summary>
    /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
    internal class AssertEqualityComparer<T> : IEqualityComparer<T>
    {
        #region 定数

        /// <summary>
        /// デフォルトの項目の等値比較子。
        /// </summary>
        private static readonly IEqualityComparer DefaultItemEqualityComparer = new AssertEqualityComparerAdapter<object>(new AssertEqualityComparer<object>());

        /// <summary>
        /// <see cref="Nullable<>"/> の型情報。
        /// </summary>
        private static readonly TypeInfo NullableTypeInfo = typeof(Nullable<>).GetTypeInfo();

        #endregion

        #region プロパティ

        /// <summary>
        /// 項目の等値比較子ファクトリーを取得します。
        /// </summary>
        /// <remarks>
        /// 項目の等値比較子は、比較対象のオブジェクトが反復子である場合、各項目の等値比較子として使用されます。
        /// </remarks>
        private Func<IEqualityComparer> ItemEqualityComparerFactory { get; }

        /// <summary>
        /// <see cref="CompareTypedSets{TElement}(IEnumerable, IEnumerable)"/> のメソッド情報を取得します。
        /// </summary>
        private MethodInfo CompareTypedSetsMethod
        {
            get
            {
                if (compareTypedSetsMethod is null)
                {
                    compareTypedSetsMethod = GetType().GetTypeInfo().GetDeclaredMethod(nameof(CompareTypedSets));
                }
                return compareTypedSetsMethod;
            }
        }
        private static MethodInfo compareTypedSetsMethod;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="itemEaualityComparer">項目の等値比較子 (比較対象のオブジェクトが反復子である場合、各項目の等値比較子に使用されます)。</param>
        public AssertEqualityComparer(IEqualityComparer itemEaualityComparer = null)
        {
            ItemEqualityComparerFactory = () => itemEaualityComparer ?? DefaultItemEqualityComparer;
        }

        #endregion

        #region メソッド

        #region 等値比較子

        /// <summary>
        /// 指定したオブジェクトが等しいかを判断します。
        /// </summary>
        /// <param name="x">比較対象のオブジェクト。</param>
        /// <param name="y">比較対象のオブジェクト。</param>
        /// <returns>指定したオブジェクトが等しいかを示す値。</returns>
        public bool Equals(T x, T y)
        {
            var typeInfo = typeof(T).GetTypeInfo();

            // Nullable である場合、null 比較による判断を試みます。
            if (!typeInfo.IsValueType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().GetTypeInfo().IsAssignableFrom(NullableTypeInfo)))
            {
                if (object.Equals(x, default(T)))
                {
                    return object.Equals(y, default(T));
                }

                if (object.Equals(y, default(T)))
                {
                    return false;
                }
            }

            // IEquatable<T> である場合、IEquatable<T>.Equals(...) で判断します。
            if (x is IEquatable<T> equatable)
            {
                return equatable.Equals(y);
            }

            // IComparable<T> である場合、IComparable<T>.CompareTo(...) で判断します。
            if (x is IComparable<T> comparableGeneric)
            {
                try
                {
                    return comparableGeneric.CompareTo(y) == 0;
                }
                catch
                {
                    // IComparable<T>.CompareTo(...) は、状況によって例外を投げる可能性があります。
                    // 例外が投げられた場合、例外を無視して比較を続けます。
                }
            }

            // IComparable である場合、IComparable.CompareTo(...) で判断します。
            if (x is IComparable comparable)
            {
                try
                {
                    return comparable.CompareTo(y) == 0;
                }
                catch
                {
                    // IComparable.CompareTo(...) は、状況によって例外を投げる可能性があります。
                    // 例外が投げられた場合、例外を無視して比較を続けます。
                }
            }

            // IDictionary である場合、各項目が等しいかを判断します。
            var dictionariesEqual = CheckIfDictionariesAreEqual(x, y);
            if (dictionariesEqual.HasValue)
            {
                return dictionariesEqual.GetValueOrDefault();
            }

            // ISet である場合、各項目が等しいかを判断します。
            var setsEqual = CheckIfSetsAreEqual(x, y, typeInfo);
            if (setsEqual.HasValue)
            {
                return setsEqual.GetValueOrDefault();
            }

            // IEnumerable である場合、各項目が等しいかを判断します。
            var enumerablesEqual = CheckIfEnumerablesAreEqual(x, y);
            if (enumerablesEqual.HasValue)
            {
                return enumerablesEqual.GetValueOrDefault();
            }

            // IStructuralEquatable である場合、IStructuralEquatable.Equals(...) で判断します。
            if (x is IStructuralEquatable structuralEquatable && structuralEquatable.Equals(y, new StructuralEqualityComparer(ItemEqualityComparerFactory())))
            {
                return true;
            }

            // IEquatable<typeof(y)> である場合、IEquatable<typeof(y)>.Equals(...) で判断します。
            var iequatableY = typeof(IEquatable<>).MakeGenericType(y.GetType()).GetTypeInfo();
            if (iequatableY.IsAssignableFrom(x.GetType().GetTypeInfo()))
            {
                var equalsMethod = iequatableY.GetDeclaredMethod(nameof(IEquatable<T>.Equals));
                return (bool)equalsMethod.Invoke(x, new object[] { y });
            }

            // IComparable<typeof(y)> である場合、IComparable<typeof(y)>.Equals(...) で判断します。
            var icomparableY = typeof(IComparable<>).MakeGenericType(y.GetType()).GetTypeInfo();
            if (icomparableY.IsAssignableFrom(x.GetType().GetTypeInfo()))
            {
                var compareToMethod = icomparableY.GetDeclaredMethod(nameof(IComparable<T>.CompareTo));
                try
                {
                    return (int)compareToMethod.Invoke(x, new object[] { y }) == 0;
                }
                catch
                {
                    // IComparable<typeof(y)>.CompareTo は、状況によって例外を投げる可能性があります。
                    // 例外が投げられた場合、例外を無視して比較を続けます。
                }
            }

            // 上記のいずれでもない場合、object.Equals(...) で判断します。
            return object.Equals(x, y);
        }

        #region IEnumerable の比較

        /// <summary>
        /// 指定したオブジェクトが <see cref="IEnumerable"/> である場合、等しいかを判断します。
        /// </summary>
        /// <param name="x">比較対象のオブジェクト。</param>
        /// <param name="y">比較対象のオブジェクト。</param>
        /// <returns>指定したオブジェクトが等しいかを示す値 (<see cref="IEnumerable"/> ではない場合、null)。</returns>
        private bool? CheckIfEnumerablesAreEqual(T x, T y)
        {
            if (x is IEnumerable enumerableX && y is IEnumerable enumerableY)
            {
                IEnumerator enumeratorX = null;
                IEnumerator enumeratorY = null;
                try
                {
                    enumeratorX = enumerableX.GetEnumerator();
                    enumeratorY = enumerableY.GetEnumerator();
                    var equalityComparer = ItemEqualityComparerFactory();

                    while (true)
                    {
                        var hasNextX = enumeratorX.MoveNext();
                        var hasNextY = enumeratorY.MoveNext();

                        if (!hasNextX || !hasNextY)
                        {
                            if (hasNextX == hasNextY)
                            {
                                // Array.GetEnumerator() は配列を平坦化し、配列の次元と長さを無視します。
                                // 配列の次元と長さが等しいかを判断します。
                                if (enumerableX is Array xArray && enumerableY is Array yArray)
                                {
                                    // new object[2,1] != new object[2]
                                    if (xArray.Rank != yArray.Rank)
                                    {
                                        return false;
                                    }

                                    // new object[2,1] != new object[1,2]
                                    for (int i = 0; i < xArray.Rank; i++)
                                    {
                                        if (xArray.GetLength(i) != yArray.GetLength(i))
                                        {
                                            return false;
                                        }
                                    }
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (!equalityComparer.Equals(enumeratorX.Current, enumeratorY.Current))
                        {
                            return false;
                        }
                    }
                }
                finally
                {
                    if (enumeratorX is IDisposable disposableX)
                    {
                        disposableX.Dispose();
                    }

                    if (enumeratorY is IDisposable disposableY)
                    {
                        disposableY.Dispose();
                    }
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IDictionary の比較

        /// <summary>
        /// 指定したオブジェクトが <see cref="IDictionary"/> である場合、等しいかを判断します。
        /// </summary>
        /// <param name="x">比較対象のオブジェクト。</param>
        /// <param name="y">比較対象のオブジェクト。</param>
        /// <returns>指定したオブジェクトが等しいかを示す値 (<see cref="IDictionary"/> ではない場合、null)。</returns>
        private bool? CheckIfDictionariesAreEqual(T x, T y)
        {
            if (x is IDictionary dictionaryX && y is IDictionary dictionaryY)
            {
                if (dictionaryX.Count != dictionaryY.Count)
                {
                    return false;
                }

                var equalityComparer = ItemEqualityComparerFactory();
                var dictionaryYKeys = new HashSet<object>(dictionaryY.Keys.Cast<object>());

                foreach (var key in dictionaryX.Keys)
                {
                    if (!dictionaryYKeys.Contains(key))
                    {
                        return false;
                    }

                    var valueX = dictionaryX[key];
                    var valueY = dictionaryY[key];

                    if (!equalityComparer.Equals(valueX, valueY))
                    {
                        return false;
                    }

                    dictionaryYKeys.Remove(key);
                }

                return dictionaryYKeys.Count == 0;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region ISet<T> の比較

        /// <summary>
        /// 指定したオブジェクトが <see cref="ISet<T>"/> である場合、等しいかを判断します。
        /// </summary>
        /// <param name="x">比較対象のオブジェクト。</param>
        /// <param name="y">比較対象のオブジェクト。</param>
        /// <param name="typeInfo">比較対象のオブジェクトの型情報。</param>
        /// <returns>指定したオブジェクトが等しいかを示す値 (<see cref="ISet<T>"/> ではない場合、null)。</returns>
        private bool? CheckIfSetsAreEqual(T x, T y, TypeInfo typeInfo)
        {
            if (!IsSet(typeInfo))
            {
                return null;
            }

            if (x is IEnumerable enumerableX && y is IEnumerable enumerableY)
            {
                Type elementType;
                if (typeof(T).GenericTypeArguments.Length != 1)
                {
                    elementType = typeof(object);
                }
                else
                {
                    elementType = typeof(T).GenericTypeArguments[0];
                }

                var method = CompareTypedSetsMethod.MakeGenericMethod(new Type[] { elementType });
                return (bool)method.Invoke(this, new object[] { enumerableX, enumerableY });
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 指定した型情報が <see cref="ISet<T>"/> かを判断します。
        /// </summary>
        /// <param name="typeInfo">型情報。</param>
        /// <returns>指定した型情報が <see cref="ISet<T>"/> かを示す値。</returns>
        private bool IsSet(TypeInfo typeInfo)
        {
            return typeInfo.ImplementedInterfaces
                .Select(i => i.GetTypeInfo())
                .Where(ti => ti.IsGenericType)
                .Select(ti => ti.GetGenericTypeDefinition())
                .Contains(typeof(ISet<>).GetGenericTypeDefinition());
        }

        /// <summary>
        /// 指定した <see cref="IEnumerable"/>(<see cref="ISet<T>"/>) が等しいかを判断します。
        /// </summary>
        /// <typeparam name="TItem"><see cref="IEnumerable"/> の項目の型。</typeparam>
        /// <param name="enumerableX">比較対象のオブジェクト。</param>
        /// <param name="enumerableY">比較対象のオブジェクト。</param>
        /// <returns>指定したオブジェクトが等しいかを示す値。</returns>
        private bool CompareTypedSets<TItem>(IEnumerable enumerableX, IEnumerable enumerableY)
        {
            var setX = new HashSet<TItem>(enumerableX.Cast<TItem>());
            var setY = new HashSet<TItem>(enumerableY.Cast<TItem>());
            return setX.SetEquals(setY);
        }

        #endregion

        #endregion

        #region ハッシュコード

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト。</param>
        /// <returns>ハッシュコード。</returns>
        /// <remarks>
        /// このクラスは <see cref="GetHashCode(T)"/> を実装しません。
        /// このクラスをハッシュコンテナーに使用しないでください。
        /// </remarks>
        /// <exception cref="NotImplementedException">このメソッドを呼び出された場合、投げられます。</exception>
        public int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region 内部クラス

        /// <summary>
        /// <see cref="IStructuralEquatable"/> の等値比較子。
        /// </summary>
        private class StructuralEqualityComparer : IEqualityComparer
        {
            #region プロパティ

            /// <summary>
            /// 項目の等値比較子を取得します。
            /// </summary>
            /// <remarks>
            /// 項目の等値比較子は、比較対象のオブジェクトが反復子である場合、各項目の等値比較子に使用されます。
            /// </remarks>
            private IEqualityComparer ItemEqualityComparer { get; }

            /// <summary>
            /// <see cref="EqualsGeneric{U}(U, U)"/> のメソッド情報を取得します。
            /// </summary>
            private MethodInfo EqualsGenericMethod
            {
                get
                {
                    if (equalsGenericMethod == null)
                    {
                        equalsGenericMethod = typeof(StructuralEqualityComparer).GetTypeInfo().GetDeclaredMethod(nameof(EqualsGeneric));
                    }

                    return equalsGenericMethod;
                }
            }
            private static MethodInfo equalsGenericMethod;

            #endregion

            #region コンストラクター

            /// <summary>
            /// コンストラクター。
            /// </summary>
            /// <param name="itemEqualityComparer">項目の等値比較子 (比較対象のオブジェクトが反復子である場合、各項目の等値比較子に使用されます)。</param>
            public StructuralEqualityComparer(IEqualityComparer itemEqualityComparer)
            {
                ItemEqualityComparer = itemEqualityComparer;
            }

            #endregion

            #region メソッド

            #region 等値比較子

            /// <summary>
            /// 指定したオブジェクトが等しいかを判断します。
            /// </summary>
            /// <param name="x">比較対象のオブジェクト。</param>
            /// <param name="y">比較対象のオブジェクト。</param>
            /// <returns>指定したオブジェクトが等しいかを示す値。</returns>
            public new bool Equals(object x, object y)
            {
                if (x == null)
                {
                    return y == null;
                }
                if (y == null)
                {
                    return false;
                }

                // AssertEqualityComparer から最高の結果を得るために、比較対象のオブジェクトの型を特定します。
                // 比較対象のオブジェクトの型が同じではない場合、System.Object であると想定します。
                // これはインターフェイスなどを共有しているかどうかを確認しようとする C＃ コンパイラよりも単純ですが、
                // AssertEqualityComparer<System.Object> が十分に賢いため、ここではやりすぎになる可能性があります。
                var objectType = x.GetType() == y.GetType() ? x.GetType() : typeof(object);
                return (bool)EqualsGenericMethod.MakeGenericMethod(objectType).Invoke(this, new object[] { x, y });
            }

            /// <summary>
            /// 指定したオブジェクトが等しいかを判断します。
            /// </summary>
            /// <typeparam name="U">比較対象のオブジェクトの型。</typeparam>
            /// <param name="x">比較対象のオブジェクト。</param>
            /// <param name="y">比較対象のオブジェクト。</param>
            /// <returns>指定したオブジェクトが等しいかを示す値。</returns>
            private bool EqualsGeneric<U>(U x, U y)
            {
                return new AssertEqualityComparer<U>(ItemEqualityComparer).Equals(x, y);
            }

            #endregion

            #region ハッシュコード

            /// <summary>
            /// ハッシュコードを取得します。
            /// </summary>
            /// <param name="obj">比較対象のオブジェクト。</param>
            /// <returns>ハッシュコード。</returns>
            /// <remarks>
            /// このクラスは <see cref="GetHashCode(T)"/> を実装しません。
            /// このクラスをハッシュコンテナーに使用しなでください。
            /// </remarks>
            /// <exception cref="NotImplementedException">このメソッドを呼び出された場合、投げられます。</exception>
            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }

            #endregion

            #endregion
        }

        #endregion
    }
}