using SoftCube.Configuration;
using SoftCube.Runtime;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SoftCube.Log
{
    /// <summary>
    /// ファイルアペンダー。
    /// </summary>
    public class FileAppender : Appender
    {
        #region プロパティ

        #region ファイル

        /// <summary>
        /// ファイルオープン方針。
        /// </summary>
        /// <remarks>
        /// ファイルオープン時の既存ログファイルの取り扱い方針を指定します。
        ///・<see cref="FileOpenPolicy.Append"/> : 既存ログファイルの末尾に追加します。
        ///・<see cref="FileOpenPolicy.Backup"/> : 既存ログファイルをバックアップします。
        ///・<see cref="FileOpenPolicy.Overwrite"/> : 既存ログファイルを上書きします (既存のログは消失するので注意してください)。
        /// </remarks>
        public FileOpenPolicy FileOpenPolicy { get; set; } = FileOpenPolicy.Append;

        /// <summary>
        /// エンコーディング。
        /// </summary>
        public Encoding Encoding
        {
            get => encoding;
            set => encoding = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 最大ファイルサイズ (単位：byte)。
        /// </summary>
        /// <remarks>
        /// 現在のログファイルの容量が最大ファイルサイズを超過した場合、
        /// 現在のログファイルをバックアップします。
        /// </remarks>
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024;

        /// <summary>
        /// ログファイルパス。
        /// </summary>
        public string LogFilePath { get; set; }

        /// <summary>
        /// バックアップファイルパス。
        /// </summary>
        public string BackupFilePath { get; set; }

        /// <summary>
        /// ログファイル。
        /// </summary>
        private LogFile LogFile { get; set; }

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public FileAppender()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public FileAppender(XElement xappender)
            : base(xappender)
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            FileOpenPolicy = xappender.Property(nameof(FileOpenPolicy)).ToFileOpenPolicy();
            Encoding       = Encoding.GetEncoding(xappender.Property("Encoding"));
            MaxFileSize    = ParseMaxFileSize(xappender.Property("MaxFileSize"));
            LogFilePath    = ParseFilePath(xappender.Property(nameof(LogFilePath)));
            BackupFilePath = xappender.Property(nameof(BackupFilePath));

            Open();
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        internal FileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        #endregion

        #region メソッド

        #region 破棄

        /// <summary>
        /// 破棄します。
        /// </summary>
        /// <param name="disposing">
        /// <see cref="IDisposable.Dispose"/> から呼び出されたか。
        /// <c>true</c> の場合、マネージリソースを破棄します。
        /// <c>false</c> の場合、マネージリソースを破棄しないでください。
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion

        #region ログファイルを開く

        /// <summary>
        /// ログファイルを開きます。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        public void Open()
        {
            //if (filePath == null)
            //{
            //    throw new ArgumentNullException(nameof(filePath));
            //}

            //LogFilePath = filePath;

            // 出力先ディレクトリが存在しない場合、新規作成します。
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ログファイルを閉じます。
            Close();

            // ログファイルを開きます。
            if (FileOpenPolicy == FileOpenPolicy.Overwrite || !File.Exists(LogFilePath))
            {
                File.Create(LogFilePath).Dispose();
                File.SetCreationTime(LogFilePath, SystemClock.Now);
            }

            LogFile = new LogFile(LogFilePath, BackupFilePath, Encoding, SystemClock);

            // バックアップ条件に適合している場合、現在のログファイルをバックアップします。
            if (FileOpenPolicy == FileOpenPolicy.Backup && LogFile.FileSize != 0)
            {
                LogFile.Backup();
            }
        }

        #endregion

        #region ログファイルを閉じる

        /// <summary>
        /// ログファイルを閉じます。
        /// </summary>
        public void Close()
        {
            if (LogFile != null)
            {
                LogFile.Close();
                LogFile = null;
            }
        }

        #endregion

        #region ログ出力

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public override void Log(string log)
        {
            if (LogFile == null)
            {
                return;
            }

            lock (LogFile)
            {
                //// バックアップ条件に適合している場合、現在のログファイルをバックアップします。
                //// その後、新たにログファイルを開きます。
                //var isDateTimeChanged = backupFile.BackupFilePath(FilePath, CreationTime) != backupFile.BackupFilePath(FilePath, SystemClock.Now);
                //var isOverCapacity    = MaxFileSize <= FileSize;
                //if (isDateTimeChanged || isOverCapacity)
                //{
                //    Backup();
                //}

                //
                LogFile.Write(log);
            }
        }

        #endregion

        #region 解析

        /// <summary>
        /// ファイルパスを解析します。
        /// </summary>
        /// <param name="filePath">ファイルパスを示す文字列。</param>
        /// <returns>実際のファイルパス。</returns>
        /// <remarks>
        /// 以下の特殊ディレクトリへのプレースフォルダを、このメソッド内で実際のディレクトリパスに置換します。
        /// ・{ApplicationData}        : 現在のローミングユーザーの Application Data フォルダ (例、C:\Users\UserName\AppData\Roaming)。
        /// ・{CommonApplicationData}  : すべてのユーザーの Application Data フォルダ (例、C:\ProgramData)。
        /// ・{CommonDesktopDirectory} : パブリックのデスクトップフォルダ (例、C:\Users\Public\Desktop)。
        /// ・{CommonDocuments}        : パブリックのドキュメントフォルダ (例、C:\Users\Public\Documents)。
        /// ・{Desktop}                : デスクトップ (名前空間のルート) を示す仮想フォル (例、C:\Users\UserName\Desktop)。
        /// ・{DesktopDirectory}       : 物理的なデスクトップ (例、C:\Users\UserName\Desktop)。
        /// ・{LocalApplicationData}   : ローカル Application Data フォルダ (例、C:\Users\UserName\AppData\Local)。
        /// ・{MyDocuments}            : マイドキュメント (例、C:\Users\UserName\Documents)。
        /// ・{Personal}               : マイドキュメント (例、C:\Users\UserName\Documents)。
        /// ・{UserProfile}            : ユーザーのプロファイルフォルダ (例、C:\Users\UserName)。
        /// </remarks>
        private string ParseFilePath(string filePath)
        {
            if (filePath.StartsWith("{ApplicationData}"))
            {
                return filePath.Replace("{ApplicationData}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{CommonApplicationData}"))
            {
                return filePath.Replace("{CommonApplicationData}", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{CommonDesktopDirectory}"))
            {
                return filePath.Replace("{CommonDesktopDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{CommonDocuments}"))
            {
                return filePath.Replace("{CommonDocuments}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{Desktop}"))
            {
                return filePath.Replace("{Desktop}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{DesktopDirectory}"))
            {
                return filePath.Replace("{DesktopDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{LocalApplicationData}"))
            {
                return filePath.Replace("{LocalApplicationData}", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{MyDocuments}"))
            {
                return filePath.Replace("{MyDocuments}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{Personal}"))
            {
                return filePath.Replace("{Personal}", Environment.GetFolderPath(Environment.SpecialFolder.Personal, Environment.SpecialFolderOption.Create));
            }
            if (filePath.StartsWith("{UserProfile}"))
            {
                return filePath.Replace("{UserProfile}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create));
            }

            return filePath;
        }

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

        #endregion
    }
}
