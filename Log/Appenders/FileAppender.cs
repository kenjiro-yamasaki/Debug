using SoftCube.Asserts;
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
        /// ファイルパス。
        /// </summary>
        public string FilePath => FileStream.Name;

        /// <summary>
        /// バックアップファイルパス。
        /// </summary>
        public string BackupFilePath
        {
            get => backupFilePath.Format;
            set => backupFilePath = new BackupFilePath(value);
        }
        private BackupFilePath backupFilePath = new BackupFilePath(@"{DateTime:yyyy-DD-mm}{Index:\.000}.log");

        /// <summary>
        /// ファイルサイズ（単位：byte）。
        /// </summary>
        public long FileSize => FileStream.Position;

        /// <summary>
        /// ファイル作成日時。
        /// </summary>
        public DateTime CreationTime => File.GetCreationTime(FilePath);

        /// <summary>
        /// ファイルストリーム。
        /// </summary>
        private FileStream FileStream { get; set; }

        /// <summary>
        /// ストリームライター。
        /// </summary>
        private StreamWriter Writer { get; set; }

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
            BackupFilePath = xappender.Property(nameof(BackupFilePath));

            var filePath = ParseFilePath(xappender.Property("FilePath"));
            Open(filePath);
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
        public void Open(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            // 出力先ディレクトリが存在しない場合、新規作成します。
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ログファイルを閉じます。
            Close();

            // ログファイルを開きます。
            if (FileOpenPolicy == FileOpenPolicy.Overwrite || !File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
                File.SetCreationTime(filePath, SystemClock.Now);
            }

            FileStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            FileStream.Seek(0, SeekOrigin.End);
            Writer = new StreamWriter(FileStream, Encoding);

            // バックアップ条件に適合している場合、現在のログファイルをバックアップします。
            if (FileOpenPolicy == FileOpenPolicy.Backup && FileSize != 0)
            {
                Backup();
            }
        }

        #endregion

        #region ログファイルを閉じる

        /// <summary>
        /// ログファイルを閉じます。
        /// </summary>
        public void Close()
        {
            if (FileStream != null && Writer != null)
            {
                lock (Writer)
                {
                    Writer.Dispose();
                    Writer = null;

                    FileStream.Dispose();
                    FileStream = null;
                }
            }
            else
            {
                Assert.Null(FileStream);
                Assert.Null(Writer);
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
            if (Writer == null)
            {
                return;
            }

            lock (Writer)
            {
                // バックアップ条件に適合している場合、現在のログファイルをバックアップします。
                // その後、新たにログファイルを開きます。
                var isDateTimeChanged = backupFilePath.Convert(FilePath, CreationTime) != backupFilePath.Convert(FilePath, SystemClock.Now);
                var isOverCapacity    = MaxFileSize <= FileSize;
                if (isDateTimeChanged || isOverCapacity)
                {
                    Backup();
                }

                //
                Writer.Write(log);
                Writer.Flush();
            }
        }

        #endregion

        #region バックアップ

        /// <summary>
        /// 現在のログファイルをバックアップします。
        /// </summary>
        public void Backup()
        {
            Assert.NotNull(FileStream);
            var filePath = FilePath;

            // 現在のログファイルを閉じます。
            Close();

            // 現在のログファイルをバックアップファイルに名前変更します。
            Assert.True(File.Exists(filePath));
            var fileName       = Path.GetFileName(filePath);
            var backupDateTime = File.GetCreationTime(filePath);

            for (int backupIndex = 0; true; backupIndex++)
            {
                if (backupIndex == 0)
                {
                    var backupFilePath0 = backupFilePath.Convert(filePath, backupDateTime);
                    var backupFilePath1 = backupFilePath.Convert(filePath, backupDateTime, backupIndex);

                    if (!File.Exists(backupFilePath0) && !File.Exists(backupFilePath1))
                    {
                        Assert.True(File.Exists(filePath));
                        Assert.False(File.Exists(backupFilePath0));
                        File.Move(filePath, backupFilePath0);
                        break;
                    }
                }
                else if (backupIndex == 1)
                {
                    var backupFilePath = this.backupFilePath.Convert(filePath, backupDateTime, backupIndex);

                    if (!File.Exists(backupFilePath))
                    {
                        var srcBackupFilePath0  = this.backupFilePath.Convert(filePath, backupDateTime);
                        var destBackupFilePath0 = this.backupFilePath.Convert(filePath, backupDateTime, 0);
                        Assert.True(File.Exists(srcBackupFilePath0));
                        Assert.False(File.Exists(destBackupFilePath0));
                        File.Move(srcBackupFilePath0, destBackupFilePath0);

                        Assert.True(File.Exists(filePath));
                        Assert.False(File.Exists(backupFilePath));
                        File.Move(filePath, backupFilePath);
                        break;
                    }
                }
                else
                {
                    var backupFilePath = this.backupFilePath.Convert(filePath, backupDateTime, backupIndex);

                    if (!File.Exists(backupFilePath))
                    {
                        Assert.True(File.Exists(filePath));
                        Assert.False(File.Exists(backupFilePath));
                        File.Move(filePath, backupFilePath);
                        break;
                    }
                }
            }

            // 新たにログファイルを開きます。
            File.Create(filePath).Dispose();
            File.SetCreationTime(filePath, SystemClock.Now);

            FileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            Writer = new StreamWriter(FileStream, Encoding);
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
