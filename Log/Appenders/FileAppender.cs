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
        /// ファイルパス。
        /// </summary>
        public string FilePath
        {
            get
            {
                if (!IsOpened)
                {
                    throw new InvalidOperationException("ファイルが開かれていません");
                }

                return FileStream.Name;
            }
        }

        /// <summary>
        /// ファイルサイズ（単位：byte）。
        /// </summary>
        public long FileSize
        {
            get
            {
                if (!IsOpened)
                {
                    throw new InvalidOperationException("ファイルが開かれていません");
                }

                return FileStream.Position;
            }
        }

        /// <summary>
        /// ファイル作成日時。
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                if (!IsOpened)
                {
                    throw new InvalidOperationException("ファイルが開かれていません");
                }

                return File.GetCreationTime(FilePath);
            }
        }

        /// <summary>
        /// バックアップファイルパス。
        /// </summary>
        public string BackupFilePath
        {
            get => backupFilePath;
            set
            {
                var index = value.IndexOfAny(Path.GetInvalidPathChars());
                if (0 <= index)
                {
                    throw new ArgumentException($"BackupFilePath に使用できない文字[{value[index]}]が使われています。", nameof(value));
                }

                if (value != backupFilePath)
                {
                    backupFilePath = value;

                    value = value.Replace("{ApplicationData}",        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    value = value.Replace("{CommonApplicationData}",  Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                    value = value.Replace("{CommonDesktopDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
                    value = value.Replace("{CommonDocuments}",        Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
                    value = value.Replace("{Desktop}",                Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    value = value.Replace("{DesktopDirectory}",       Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    value = value.Replace("{LocalApplicationData}",   Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                    value = value.Replace("{MyDocuments}",            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    value = value.Replace("{Personal}",               Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                    value = value.Replace("{UserProfile}",            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

                    value = value.Replace("{FilePath}",  "{0}");
                    value = value.Replace("{Directory}", "{1}");
                    value = value.Replace("{FileName}",  "{2}");
                    value = value.Replace("{FileBody}",  "{3}");
                    value = value.Replace("{Extension}", "{4}");

                    var dateTimeFormat = ParseDateTimeFormat(value);
                    var indexFormat = ParseIndexFormat(value);
                    if (indexFormat == null)
                    {
                        throw new ArgumentException("バックアップの書式には {BackupIndex} を含めてください。", nameof(backupFilePath));
                    }

                    value = value.Replace(dateTimeFormat, dateTimeFormat.Replace("DateTime", "5"));
                    value = value.Replace(indexFormat, indexFormat.Replace("Index", "6"));

                    BackupFilePathFormat0 = value.Replace(indexFormat.Replace("Index", "6"), "");
                    BackupFilePathFormat1 = value;
                }
            }
        }
        private string backupFilePath;

        /// <summary>
        /// 開かれているか。
        /// </summary>
        public bool IsOpened => FileStream != null;

        /// <summary>
        /// バックアップファイルパスの文字列フォーマット。
        /// </summary>
        private string BackupFilePathFormat0 { get; set; }

        /// <summary>
        /// バックアップファイルパスの文字列フォーマット。
        /// </summary>
        private string BackupFilePathFormat1 { get; set; }

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
            : this(new SystemClock())
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
            BackupFilePath = xappender.Property(nameof(BackupFilePath));

            var filePath   = ParseFilePath(xappender.Property(nameof(FilePath)));
            Open(filePath);
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        internal FileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
            BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}{Extension}";
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

        #region オープン

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

        #region クローズ

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
            if (!IsOpened)
            {
                return;
            }

            lock (Writer)
            {
                Writer.Write(log);
                Writer.Flush();
            }
        }

        #endregion

        #region バックアップ

        /// <summary>
        /// ログファイルをバックアップします。
        /// </summary>
        public void Backup()
        {
            Assert.NotNull(FileStream);
            var filePath = FilePath;

            // 現在のログファイルを閉じます。
            Close();

            // 現在のログファイルをバックアップファイルに名前変更します。
            Assert.True(File.Exists(filePath));
            var creationTime = File.GetCreationTime(filePath);

            for (int index = 0; true; index++)
            {
                var backupFilePath = GetBackupFilePath(filePath, creationTime, index);
                var directory = Path.GetDirectoryName(backupFilePath);
                if (Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (index == 0)
                {
                    var backupFilePath0 = GetBackupFilePath(filePath, creationTime);

                    if (!File.Exists(backupFilePath0) && !File.Exists(backupFilePath))
                    {
                        Assert.True(File.Exists(filePath));
                        Assert.False(File.Exists(backupFilePath0));
                        File.Move(filePath, backupFilePath0);
                        break;
                    }
                }
                else if (index == 1)
                {
                    if (!File.Exists(backupFilePath))
                    {
                        File.Move(GetBackupFilePath(filePath, creationTime), GetBackupFilePath(filePath, creationTime, 0));

                        Assert.True(File.Exists(filePath));
                        Assert.False(File.Exists(backupFilePath));
                        File.Move(filePath, backupFilePath);
                        break;
                    }
                }
                else
                {
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

            FileStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            FileStream.Seek(0, SeekOrigin.End);
            Writer = new StreamWriter(FileStream, Encoding);
        }

        /// <summary>
        /// バックアップファイルパスを取得します。
        /// </summary>
        /// <param name="dateTime">日時。</param>
        /// <returns>バックアップファイルパス。</returns>
        private string GetBackupFilePath(string filePath, DateTime dateTime)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName  = Path.GetFileName(filePath);
            var fileBody  = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            return string.Format(
                BackupFilePathFormat0,
                filePath,
                directory,
                fileName,
                fileBody,
                extension,
                dateTime);
        }

        /// <summary>
        /// バックアップファイルパスを取得します。
        /// </summary>
        /// <param name="dateTime">日時。</param>
        /// <param name="index">インデックス。</param>
        /// <returns>バックアップファイルパス。</returns>
        private string GetBackupFilePath(string filePath, DateTime dateTime, int index)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName  = Path.GetFileName(filePath);
            var fileBody  = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            return string.Format(
                BackupFilePathFormat1,
                filePath,
                directory,
                fileName,
                fileBody,
                extension,
                dateTime,
                index);
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
        private static string ParseFilePath(string filePath)
        {
            filePath = filePath.Replace("{ApplicationData}",        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            filePath = filePath.Replace("{CommonApplicationData}",  Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            filePath = filePath.Replace("{CommonDesktopDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            filePath = filePath.Replace("{CommonDocuments}",        Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            filePath = filePath.Replace("{Desktop}",                Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            filePath = filePath.Replace("{DesktopDirectory}",       Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            filePath = filePath.Replace("{LocalApplicationData}",   Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            filePath = filePath.Replace("{MyDocuments}",            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            filePath = filePath.Replace("{Personal}",               Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            filePath = filePath.Replace("{UserProfile}",            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            return filePath;
        }

        /// <summary>
        /// 日時の書式を解析します。
        /// </summary>
        /// <param name="format">書式。</param>
        /// <returns>日時の書式。</returns>
        private static string ParseDateTimeFormat(string format)
        {
            var startIndex = format.IndexOf("{DateTime");
            if (startIndex == -1)
            {
                return null;
            }

            var endIndex = format.IndexOf("}", startIndex);
            if (endIndex == -1)
            {
                return null;
            }

            var length = endIndex - startIndex + 1;
            return format.Substring(startIndex, length);
        }

        /// <summary>
        /// インデックスの書式を解析します。
        /// </summary>
        /// <param name="format">書式。</param>
        /// <returns>インデックスの書式。</returns>
        private static string ParseIndexFormat(string format)
        {
            var startIndex = format.IndexOf("{Index");
            if (startIndex == -1)
            {
                return null;
            }

            var endIndex = format.IndexOf("}", startIndex);
            if (endIndex == -1)
            {
                return null;
            }

            var length = endIndex - startIndex + 1;
            return format.Substring(startIndex, length);
        }

        #endregion

        #endregion
    }
}
