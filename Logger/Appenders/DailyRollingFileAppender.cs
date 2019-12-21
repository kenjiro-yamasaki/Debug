using SoftCube.System;
using System.IO;
using System.Text;

namespace SoftCube.Logger
{
    /// <summary>
    /// 日付ローリングファイルアペンダー。
    /// </summary>
    /// <remarks>
    /// <see cref="DailyRollingFileAppender"/> は <see cref="FileAppender"/> クラスを継承したクラスです。
    /// 日付や時間によりログファイルのバックアップを作成したい場合に使用します。
    /// </remarks>
    public class DailyRollingFileAppender : FileAppender
    {
        #region プロパティ

        /// <summary>
        /// 日付パターン。
        /// </summary>
        /// <remarks>
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </remarks>
        public string DatePattern { get; set; } = "yyyyMMdd";

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public DailyRollingFileAppender()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public DailyRollingFileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="append">ファイルにログを追加するかを示す値。</param>
        /// <param name="encoding">エンコーディング。</param>
        /// <seealso cref="Open(string, bool, Encoding)"/>
        public DailyRollingFileAppender(string filePath, bool append, Encoding encoding)
            : base(filePath, append, encoding)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="append">ファイルにログを追加するかを示す値。</param>
        /// <param name="encoding">エンコーディング。</param>
        /// <seealso cref="Open(string, bool, Encoding)"/>
        public DailyRollingFileAppender(ISystemClock systemClock, string filePath, bool append, Encoding encoding)
            : base(systemClock, filePath, append, encoding)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public override void Log(string log)
        {
            if (CreationTime.ToString(DatePattern) != SystemClock.Now.ToString(DatePattern))
            {
                RollLogAndBackupFiles();
            }

            base.Log(log);
        }

        /// <summary>
        /// ログファイルとバックアップファイルをローテンションします。
        /// </summary>
        private void RollLogAndBackupFiles()
        {
            var filePath      = FilePath;
            var directoryName = Path.GetDirectoryName(filePath);
            var fileName      = Path.GetFileName(filePath);
            var baseName      = Path.GetFileNameWithoutExtension(fileName);
            var extension     = Path.GetExtension(fileName);
            var encoding      = Encoding;
            var date          = CreationTime;

            // 現在のログファイルを閉じます。
            Close();

            // 現在のログファイルを新規バックアップファイルとします。
            {
                var logFilePath = filePath;

                var backupFileName = string.Format($"{{0}}{{1}}.{{2:{DatePattern}}}", baseName, extension, date);
                var backupFilePath = Path.Combine(directoryName, backupFileName);

                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }

                File.Move(logFilePath, backupFilePath);
            }

            // ログファイルを新規作成します。
            Open(filePath, append: false, encoding);
        }

        #endregion
    }
}
