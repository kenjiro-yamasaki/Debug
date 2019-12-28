using SoftCube.Runtime;
using System;
using System.IO;
using System.Xml.Linq;
using SoftCube.Configuration;

namespace SoftCube.Log
{
    /// <summary>
    /// ファイル容量ローリングファイルアペンダー。
    /// </summary>
    /// <remarks>
    /// <see cref="FileSizeRollingFileAppender"/> は <see cref="FileAppender"/> クラスを継承したクラスです。
    /// ログファイルが一定のサイズを超えたとき、バックアップファイルを作成したい場合に使用します。
    /// </remarks>
    public class FileSizeRollingFileAppender : FileAppender
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
        public FileSizeRollingFileAppender()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public FileSizeRollingFileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public FileSizeRollingFileAppender(XElement xappender)
            : base(xappender)
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            MaxFileSize    = ParseMaxFileSize(xappender.Property("MaxFileSize"));
            MaxBackupCount = int.Parse(xappender.Property(nameof(MaxBackupCount)));
        }

        #endregion

        #region メソッド

        #region ログ出力

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public override void Log(string log)
        {
            if (MaxFileSize <= FileSize)
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
                var backupNumber   = 1;
                var backupFilePath = Path.Combine(directoryName, $"{baseName}{extension}.{backupNumber}");

                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }

                File.Move(filePath, backupFilePath);
            }

            // ログファイルを新規作成します。
            Open(filePath, append: false, encoding);
        }

        #endregion

        /// <summary>
        /// 最大ファイルサイズを解析します。
        /// </summary>
        /// <param name="maxFileSize">最大ファイルサイズを示す文字列。</param>
        /// <returns>最大ファイルサイズ。</returns>
        /// <remarks>
        /// 単位を示す以下のプレースフォルダは、このメソッド内で単位変換されます。
        /// ・KB : キロバイト。
        /// ・MB : メガバイト。
        /// ・GB : ギガバイト。
        /// </remarks>
        private long ParseMaxFileSize(string maxFileSize)
        {
            const long Byte = 1;
            const long KB = Byte * 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (maxFileSize.EndsWith("KB"))
            {
                return long.Parse(maxFileSize.Replace("KB", string.Empty)) * KB;
            }
            if (maxFileSize.EndsWith("MB"))
            {
                return long.Parse(maxFileSize.Replace("MB", string.Empty)) * MB;
            }
            if (maxFileSize.EndsWith("GB"))
            {
                return long.Parse(maxFileSize.Replace("GB", string.Empty)) * GB;
            }
            return long.Parse(maxFileSize) * Byte;
        }

        #endregion
    }
}
