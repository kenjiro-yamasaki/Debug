using System;

namespace SoftCube.Profile
{
    /// <summary>
    /// 計測トランザクション。
    /// </summary>
    public class Transaction : IDisposable
    {
        #region プロパティ

        /// <summary>
        /// 計測エントリー。
        /// </summary>
        internal Entry Entry { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="entry">計測エントリー。</param>
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
