using System;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイラー。
    /// </summary>
    public class Profiler : IDisposable
    {
        #region プロパティ

        /// <summary>
        /// プロファイル名。
        /// </summary>
        private string Name { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="name">プロファイル名。</param>
        public Profiler(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (RecordManager.ContainsRecord(Name) == false)
            {
                RecordManager.AddRecord(new Record(Name));
            }
            RecordManager.GetRecord(Name).Start();
        }

        #endregion

        #region メソッド

        #region IDisposable

        /// <summary>
        /// ファイナライザー。
        /// </summary>
        ~Profiler()
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
            if (disposing)
            {
                RecordManager.GetRecord(Name).Stop();
            }
        }

        #endregion

        #endregion
    }
}
