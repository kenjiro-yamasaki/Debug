using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SoftCube.Asserts
{
    /// <summary>
    /// All アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.All"/> の失敗時に投げられます。
    /// </remarks>
    public class AllException : AssertException
    {
        #region プロパティ

        /// <summary>
        /// メッセージ。
        /// </summary>
        public override string Message
        {
            get
            {
                var formattedErrors = Errors.Select(error =>
                {
                    var indexString = string.Format(CultureInfo.CurrentCulture, "[{0}]: ", error.Index);
                    var spaces = Environment.NewLine + "".PadRight(indexString.Length);

                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}Item: {1}{2}{3}",
                        indexString, error.Item?.ToString()?.Replace(Environment.NewLine, spaces),
                        spaces,
                        error.Exception.ToString().Replace(Environment.NewLine, spaces));
                });

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}: {1} out of {2} items in the collection did not pass.{3}{4}",
                    base.Message,
                    Errors.Count(),
                    ItemCount,
                    Environment.NewLine,
                    string.Join(Environment.NewLine, formattedErrors));
            }
        }

        /// <summary>
        /// 検証中に起きたエラーコレクション。
        /// </summary>
        public IReadOnlyList<Exception> Failures => Errors.Select(e => e.Exception).ToList();

        /// <summary>
        /// コレクションの項目数。
        /// </summary>
        private int ItemCount { get; }

        /// <summary>
        /// エラーコレクション。
        /// </summary>
        private IReadOnlyList<(int Index, object Item, Exception Exception)> Errors { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="itemCount">項目数。</param>
        /// <param name="errors">エラーコレクション。</param>
        public AllException(int itemCount, (int Index, object Item, Exception Exception)[] errors)
            : base("Assert.All() Failure")
        {
            ItemCount = itemCount;
            Errors    = errors;
        }

        #endregion
    }
}