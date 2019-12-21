using System;
using System.Globalization;
using System.Linq;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Collection アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.Collection"/> の失敗時に投げられます。
    /// </remarks>
    public class CollectionException : AssertException
    {
        #region プロパティ

        /// <summary>
        /// テストに失敗したコレクション。
        /// </summary>
        public object Collection { get; set; }

        /// <summary>
        /// 期待値の数 (コレクションの項目数)。
        /// </summary>
        public int ExpectedCount { get; set; }

        /// <summary>
        /// 実測値の数 (コレクションの項目数)。
        /// </summary>
        public int ActualCount { get; set; }

        /// <summary>
        /// 最初の比較失敗が起きたインデックス。
        /// 比較が起きなかった場合 (期待値と実測値の数が異なる場合)、-1。
        /// </summary>
        public int IndexFailurePoint { get; set; }

        /// <summary>
        /// メッセージ。
        /// </summary>
        public override string Message
        {
            get
            {
                if (0 <= IndexFailurePoint)
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}{4}Collection: {1}{4}Error during comparison of item at index {2}{4}Inner exception: {3}",
                        base.Message,
                        ArgumentFormatter.Format(Collection),
                        IndexFailurePoint,
                        FormatInnerException(InnerException),
                        Environment.NewLine);
                }

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{4}Collection: {1}{4}Expected item count: {2}{4}Actual item count:   {3}",
                    base.Message,
                    ArgumentFormatter.Format(Collection),
                    ExpectedCount,
                    ActualCount,
                    Environment.NewLine);
            }
        }

        /// <summary>
        /// スタックトレース。
        /// </summary>
        public override string StackTrace
        {
            get
            {
                if (InnerStackTrace == null)
                {
                    return base.StackTrace;
                }

                return InnerStackTrace + Environment.NewLine + base.StackTrace;
            }
        }

        /// <summary>
        /// 内部スタックトレース。
        /// </summary>
        private string InnerStackTrace { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="collection">テストに失敗したコレクション。</param>
        /// <param name="expectedCount">期待値の数 (コレクションの項目数)。</param>
        /// <param name="actualCount">実測値の数 (コレクションの項目数)。</param>
        /// <param name="indexFailurePoint">最初の比較失敗が起きたインデックス。</param>
        /// <param name="innerException">内部例外。</param>
        public CollectionException(object collection, int expectedCount, int actualCount, int indexFailurePoint = -1, Exception innerException = null)
            : base("Assert.Collection() Failure", innerException)
        {
            Collection        = collection;
            ExpectedCount     = expectedCount;
            ActualCount       = actualCount;
            IndexFailurePoint = indexFailurePoint;
            InnerStackTrace   = innerException == null ? null : innerException.StackTrace;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 内部例外をフォーマットします。
        /// </summary>
        /// <param name="innerException">内部例外。</param>
        /// <returns>フォーマットされた内部例外。</returns>
        static string FormatInnerException(Exception innerException)
        {
            if (innerException == null)
            {
                return null;
            }

            var lines = innerException.Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select((value, idx) => idx > 0 ? "        " + value : value);
            return string.Join(Environment.NewLine, lines);
        }

        #endregion
    }
}
