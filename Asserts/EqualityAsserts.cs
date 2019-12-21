using System;
using System.Collections.Generic;
using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        #region Equal

        /// <summary>
        /// オブジェクトが等しいことを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="EqualException">オブジェクトが等しくない場合、投げられます。</exception>
        public static void Equal<T>(T expected, T actual)
        {
            Equal(expected, actual, GetEqualityComparer<T>());
        }

        /// <summary>
        /// オブジェクトが等しいことを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="comparer">等値比較子。</param>
        /// <exception cref="EqualException">オブジェクトが等しくない場合、投げられます。</exception>
        public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            GuardArgumentNotNull(nameof(comparer), comparer);

            if (!comparer.Equals(expected, actual))
            {
                throw new EqualException(expected, actual);
            }
        }

        /// <summary>
        /// <see cref="double"/>型の値が等しいことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="precision">比較精度の小数点以下の桁数 (有効な値：0～15)。</param>
        /// <exception cref="EqualException"><see cref="double"/>型の値が等しくない場合、投げられます。</exception>
        public static void Equal(double expected, double actual, int precision)
        {
            var expectedRounded = Math.Round(expected, precision);
            var actualRounded   = Math.Round(actual, precision);

            if (!GetEqualityComparer<double>().Equals(expectedRounded, actualRounded))
            {
                throw new EqualException(
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                );
            }
        }

        /// <summary>
        /// <see cref="decimal"/>型の値が等しいことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="precision">比較精度の小数点以下の桁数 (有効な値：0～28)。</param>
        /// <exception cref="EqualException"><see cref="decimal"/>型の値が等しくない場合、投げられます。</exception>
        public static void Equal(decimal expected, decimal actual, int precision)
        {
            var expectedRounded = Math.Round(expected, precision);
            var actualRounded   = Math.Round(actual, precision);

            if (!GetEqualityComparer<decimal>().Equals(expectedRounded, actualRounded))
            {
                throw new EqualException(
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                );
            }
        }

        /// <summary>
        /// <see cref="DateTime"/>型の値が等しいことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="precision">比較精度。</param>
        /// <exception cref="EqualException"><see cref="DateTime"/>型の値が等しくない場合、投げられます。</exception>
        public static void Equal(DateTime expected, DateTime actual, TimeSpan precision)
        {
            var difference = (expected - actual).Duration();
            if (precision < difference)
            {
                throw new EqualException(
                    string.Format(CultureInfo.CurrentCulture, "{0} ", expected),
                    string.Format(CultureInfo.CurrentCulture, "{0} difference {1} is larger than {2}",
                        actual,
                        difference.ToString(),
                        precision.ToString()
                    ));
            }
        }

        /// <summary>
        /// 型のデフォルトの等値比較子を使用し、オブジェクトが等しいことを正確に検証します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="EqualException">オブジェクトが等しくない場合、投げられます。</exception>
        public static void StrictEqual<T>(T expected, T actual)
        {
            Equal(expected, actual, EqualityComparer<T>.Default);
        }

        #endregion

        #region NotEqual

        /// <summary>
        /// オブジェクトが等しくないことを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="NotEqualException">オブジェクトが等しい場合、投げられます。</exception>
        public static void NotEqual<T>(T expected, T actual)
        {
            NotEqual(expected, actual, GetEqualityComparer<T>());
        }

        /// <summary>
        /// オブジェクトが等しくないことを検証します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="comparer">等値比較子</param>
        /// <exception cref="NotEqualException">オブジェクトが等しい場合、投げられます。</exception>
        public static void NotEqual<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            GuardArgumentNotNull("comparer", comparer);

            if (comparer.Equals(expected, actual))
            {
                throw new NotEqualException(ArgumentFormatter.Format(expected), ArgumentFormatter.Format(actual));
            }
        }

        /// <summary>
        /// <see cref="double"/>型の値が等しくないことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="precision">比較精度の小数点以下の桁数 (有効な値：0～15)。</param>
        /// <exception cref="NotEqualException"><see cref="double"/>型の値が等しい場合、投げられます。</exception>
        public static void NotEqual(double expected, double actual, int precision)
        {
            var expectedRounded = Math.Round(expected, precision);
            var actualRounded   = Math.Round(actual, precision);

            if (GetEqualityComparer<double>().Equals(expectedRounded, actualRounded))
            {
                throw new NotEqualException(
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                );
            }
        }

        /// <summary>
        /// <see cref="decimal"/>型の値が等しくないことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="precision">比較精度の小数点以下の桁数 (有効な値：0～28)。</param>
        /// <exception cref="NotEqualException"><see cref="decimal"/>型の値が等しい場合、投げられます。</exception>
        public static void NotEqual(decimal expected, decimal actual, int precision)
        {
            var expectedRounded = Math.Round(expected, precision);
            var actualRounded   = Math.Round(actual, precision);

            if (GetEqualityComparer<decimal>().Equals(expectedRounded, actualRounded))
            {
                throw new NotEqualException(
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", expectedRounded, expected),
                    string.Format(CultureInfo.CurrentCulture, "{0} (rounded from {1})", actualRounded, actual)
                );
            }
        }

        /// <summary>
        /// <see cref="DateTime"/>型の値が等しくないことを検証します。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="precision">比較精度。</param>
        /// <exception cref="NotEqualException"><see cref="DateTime"/>型の値が等しい場合、投げられます。</exception>
        public static void NotEqual(DateTime expected, DateTime actual, TimeSpan precision)
        {
            var difference = (expected - actual).Duration();
            if (difference <= precision)
            {
                throw new NotEqualException(
                    string.Format(CultureInfo.CurrentCulture, "{0} ", expected),
                    string.Format(CultureInfo.CurrentCulture, "{0} difference {1} is not larger than {2}",
                        actual,
                        difference.ToString(),
                        precision.ToString()
                    ));
            }
        }

        /// <summary>
        /// 型のデフォルトの等値比較子を使用し、オブジェクトが等しくないことを正確に検証します。
        /// </summary>
        /// <typeparam name="T">比較対象のオブジェクトの型。</typeparam>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <exception cref="NotEqualException">オブジェクトが等しい場合、投げられます。</exception>
        public static void NotStrictEqual<T>(T expected, T actual)
        {
            NotEqual(expected, actual, EqualityComparer<T>.Default);
        }

        #endregion

        #endregion
    }
}
