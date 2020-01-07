using System;

namespace SoftCube.Profiling
{
    /// <summary>
    /// トランザクション。
    /// </summary>
    /// <remarks>
    /// トランザクションは自分が担当するエントリーに対して、計測の開始と終了を指示します。
    /// 具体的には、<see cref="Transaction(Entry)"/> で計測開始を指示し、<see cref="Dispose(bool)"/> で計測終了を指示します。
    /// </remarks>
    public class Transaction : IDisposable
    {
        #region プロパティ

        /// <summary>
        /// エントリー。
        /// </summary>
        internal Entry Entry { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="entry">エントリー。</param>
        internal Transaction(Entry entry)
        {
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            Entry.Start();
        }

        #endregion

        #region メソッド

        #region IDisposable

        /// <summary>
        /// ファイナライザー。
        /// </summary>
        ~Transaction()
        {
            Dispose(false);
        }

        /// <summary>
        /// 破棄する。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 破棄します。
        /// </summary>
        /// <param name="disposing">
        /// <see cref="IDisposable.Dispose"/> から呼び出されたか。
        /// <c>true</c> の場合、マネージリソースを破棄します。
        /// <c>false</c> の場合、マネージリソースを破棄しないでください。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            Entry.Stop();
        }

        #endregion

        #endregion
    }
}
