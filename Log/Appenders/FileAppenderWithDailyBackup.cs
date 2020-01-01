using SoftCube.Xml;
using System;
using System.Xml.Linq;

namespace SoftCube.Log
{
    /// <summary>
    /// ファイルアペンダー (日付変化によるバックアップ付き)。
    /// </summary>
    public class FileAppenderWithDailyBackup : FileAppender
    {
        #region プロパティ

        /// <summary>
        /// 日付パターン。
        /// </summary>
        public string DatePattern
        {
            get => datePattern;
            set => datePattern = value ?? throw new ArgumentNullException(nameof(value));
        }
        private string datePattern;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public FileAppenderWithDailyBackup()
            : this(new Clock())
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public FileAppenderWithDailyBackup(XElement xappender)
            : base(xappender)
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            DatePattern = xappender.Property(nameof(DatePattern));
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="clock">クロック。</param>
        internal FileAppenderWithDailyBackup(IClock clock)
            : base(clock)
        {
            DatePattern = "yyyyMMdd";
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public override void Log(string log)
        {
            // バックアップ条件に適合している場合、現在のログファイルをバックアップします。
            if (CreationTime.ToString(DatePattern) != Clock.Now.ToString(DatePattern))
            {
                Backup();
            }

            base.Log(log);
        }

        #endregion
    }
}
