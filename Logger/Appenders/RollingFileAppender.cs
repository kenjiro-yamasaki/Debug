using SoftCube.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoftCube.Logger.Appenders
{
    /// <summary>
    /// ローリングファイルアペンダー。
    /// </summary>
    /// <remarks>
    /// <see cref="RollingFileAppender"/> は <see cref="FileAppender"/> クラスを継承したクラスです。
    /// ログファイルが一定のサイズを超えたとき、バックアップファイルを作成したい場合に使用します。
    /// </remarks>
    public class RollingFileAppender : FileAppender
    {
        #region プロパティ

        /// <summary>
        /// 最大ファイルサイズ (単位：byte)。
        /// </summary>
        /// <remarks>
        /// ローテンションするログファイルサイズを指定します。
        /// </remarks>
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024;

        /// <summary>
        /// 最大バックアップ数。
        /// </summary>
        /// <remarks>
        /// バックアップファイルをいくつ保持するか指定します。
        /// 例えば、<see cref="MaxBackupCount"/>=3 を指定すると、
        /// ログファイル.1→ログファイル.2→ログファイル.3とローテンションしていき、
        /// それ以上古くなると破棄されます。
        /// </remarks>
        public int MaxBackupCount { get; set; } = 10;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public RollingFileAppender()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public RollingFileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="params">パラメーター名→値変換。</param>
        public RollingFileAppender(IReadOnlyDictionary<string, string> @params)
            : base(@params)
        {
            if (@params == null)
            {
                throw new ArgumentNullException(nameof(@params));
            }

            MaxFileSize    = StringParser.ParseMaxFileSize(@params["MaxFileSize"]);
            MaxBackupCount = int.Parse(@params[nameof(MaxBackupCount)]);
        }

        #endregion

        #region メソッド

        #region ログ出力

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="log">ログ。</param>
        public override void Log(Level level, string log)
        {
            if (MaxFileSize <= FileSize)
            {
                RollLogAndBackupFiles();
            }

            base.Log(level, log);
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

            // 現在のログファイルを閉じます。
            Close();

            // 既に存在するバックアップファイルをローテーションします。
            for (int backupNumber = MaxBackupCount - 1; 1 <= backupNumber; backupNumber--)
            {
                var srcFilePath  = Path.Combine(directoryName, $"{baseName}{extension}.{backupNumber}");
                var destFilePath = Path.Combine(directoryName, $"{baseName}{extension}.{backupNumber + 1}");

                if (!File.Exists(srcFilePath))
                {
                    continue;
                }

                if (File.Exists(destFilePath))
                {
                    File.Delete(destFilePath);
                }

                File.Move(srcFilePath, destFilePath);
            }

            // 現在のログファイルを新規バックアップファイルとします。
            if (1 <= MaxBackupCount)
            {
                var logFilePath    = filePath;
                var backupNumber   = 1;
                var backupFilePath = Path.Combine(directoryName, $"{baseName}{extension}.{backupNumber}");

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

        #endregion
    }
}
