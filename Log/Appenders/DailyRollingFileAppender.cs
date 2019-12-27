using SoftCube.Configuration;
using SoftCube.Runtime;
using System;
using System.IO;
using System.Xml.Linq;

namespace SoftCube.Log
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
        /// 日付の書式。
        /// </summary>
        /// <remarks>
        /// 指定した書式にしたっがて、日付をフォーマットした文字列がログファイルの後ろに添えられます。
        /// 日付の書式に指定する文字列は、<see cref="DateTime.ToString(string)"/> で決められたものを使用します。
        /// 使用できる文字列のうち、主なものを紹介します。
        /// ・yyyyy : 年の下 5 桁。
        /// ・M     : 月 (1～12)。
        /// ・MM    : 月 (01～12)。
        /// ・d     : 日にち (1～31)。
        /// ・dd    : 日にち (01～31)。
        /// ・H     : 時間 (0～23)。
        /// ・HH    : 時間 (00～23)。
        /// ・h     : 時間 (1～12)。
        /// ・hh    : 時間 (01～12)。
        /// ・m     : 分 (0～59)。
        /// ・mm    : 分 (00～59)。
        /// ・s     : 秒 (0～59)。
        /// ・ss    : 秒 (00～59)。
        /// ・fff   : 1/1000秒。
        /// </remarks>
        /// <example>
        /// 変換パターンは、以下の例のように指定します。
        /// ・"yyyy-MM-dd" → "2019-12-17"
        /// </example>
        public string DateTimeFormat
        {
            get => dateTimeFormat;
            set
            {
                if (dateTimeFormat != value)
                {
                    int index = value.IndexOfAny(Path.GetInvalidFileNameChars());
                    if (0 <= index)
                    {
                        throw new ArgumentException(string.Format($"ファイル名に使用できない文字[{value[index]}]が使われています。"), nameof(value));
                    }

                    dateTimeFormat = value;
                }
            }
        }
        private string dateTimeFormat = "yyyy-MM-dd";

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
        /// <param name="xappender">XML の appender 要素。</param>
        public DailyRollingFileAppender(XElement xappender)
            : base(xappender)
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            DateTimeFormat = xappender.Property(nameof(DateTimeFormat));
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
            if (CreationTime.ToString(DateTimeFormat) != SystemClock.Now.ToString(DateTimeFormat))
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

                var backupFileName = string.Format($"{{0}}{{1}}.{{2:{DateTimeFormat}}}", baseName, extension, date);
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

        #endregion
    }
}
