using SoftCube.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

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
        /// 日付パターン。
        /// </summary>
        /// <remarks>
        /// ここで指定した日付パターンがログファイルの後ろに添えられることになるのですが、
        /// 日付パターンで使用する文字列は、<see cref="DateTime.ToString(string)"/> で決められたものを使用します。
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
        public string DatePattern
        {
            get => datePattern;
            set
            {
                if (datePattern != value)
                {
                    int index = value.IndexOfAny(Path.GetInvalidFileNameChars());
                    if (0 <= index)
                    {
                        throw new ArgumentException(string.Format($"ファイル名に使用できない文字[{value[index]}]が使われています。"), nameof(value));
                    }

                    datePattern = value;
                }
            }
        }
        private string datePattern = "yyyy-MM-dd";

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
        /// <param name="xparams">パラメーター名→値変換。</param>
        public DailyRollingFileAppender(IReadOnlyDictionary<string, string> xparams)
            : base(xparams)
        {
            if (xparams == null)
            {
                throw new ArgumentNullException(nameof(xparams));
            }

            DatePattern = xparams[nameof(DatePattern)];
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
            if (CreationTime.ToString(DatePattern) != SystemClock.Now.ToString(DatePattern))
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

        #endregion
    }
}
