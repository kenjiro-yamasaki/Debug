using SoftCube.Configuration;
using SoftCube.Runtime;
using System;
using System.Xml.Linq;

namespace SoftCube.Log
{
    /// <summary>
    /// ファイルアペンダー (容量超過によるバックアップ付き)。
    /// </summary>
    public class FileAppenderWithSizeBackup : FileAppender
    {
        #region プロパティ

        /// <summary>
        /// 最大ファイルサイズ (単位：byte)。
        /// </summary>
        /// <remarks>
        /// 現在のログファイルの容量が最大ファイルサイズを超過した場合、
        /// 現在のログファイルをバックアップします。
        /// </remarks>
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public FileAppenderWithSizeBackup()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public FileAppenderWithSizeBackup(XElement xappender)
            : base(xappender)
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            MaxFileSize = ParseMaxFileSize(xappender.Property("MaxFileSize"));
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        internal FileAppenderWithSizeBackup(ISystemClock systemClock)
            : base(systemClock)
        {
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
            // バックアップ条件に適合している場合、現在のログファイルをバックアップします。
            if (MaxFileSize <= FileSize)
            {
                Backup();
            }

            base.Log(log);
        }

        #endregion

        #region 解析

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
        private static long ParseMaxFileSize(string maxFileSize)
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

        #endregion
    }
}
