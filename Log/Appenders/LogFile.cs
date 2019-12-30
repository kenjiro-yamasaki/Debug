using SoftCube.Asserts;
using SoftCube.Runtime;
using System;
using System.IO;
using System.Text;

namespace SoftCube.Log
{
    /// <summary>
    /// ログファイル。
    /// </summary>
    public class LogFile
    {
        #region プロパティ

        /// <summary>
        /// ログファイルパス。
        /// </summary>
        public string LogFilePath { get; }

        /// <summary>
        /// バックアップファイルパス。
        /// </summary>
        public string BackupFilePath { get; }

        /// <summary>
        /// エンコーディング。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// ログファイルサイズ（単位：byte）。
        /// </summary>
        public long FileSize => FileStream.Position;

        /// <summary>
        /// ログファイル作成日時。
        /// </summary>
        public DateTime CreationTime => File.GetCreationTime(LogFilePath);

        /// <summary>
        /// システムクロック。
        /// </summary>
        private ISystemClock SystemClock { get; }

        /// <summary>
        /// バックアップファイルパスの文字列フォーマット。
        /// </summary>
        private string BackupFilePathFormat0 { get; }

        /// <summary>
        /// バックアップファイルパスの文字列フォーマット。
        /// </summary>
        private string BackupFilePathFormat1 { get; }

        /// <summary>
        /// ファイルストリーム。
        /// </summary>
        private FileStream FileStream { get; set; }

        /// <summary>
        /// ストリームライター。
        /// </summary>
        private StreamWriter Writer { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="logFilePath"></param>
        /// <param name="backupFilePath"></param>
        /// <param name="encoding"></param>
        /// <param name="systemClock"></param>
        public LogFile(string logFilePath, string backupFilePath, Encoding encoding, ISystemClock systemClock)
        {
            LogFilePath    = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
            BackupFilePath = backupFilePath ?? throw new ArgumentNullException(nameof(backupFilePath));
            Encoding       = encoding ?? throw new ArgumentNullException(nameof(encoding));
            SystemClock    = systemClock ?? throw new ArgumentNullException(nameof(systemClock));

            //
            backupFilePath = backupFilePath.Replace("{ApplicationData}",        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            backupFilePath = backupFilePath.Replace("{CommonApplicationData}",  Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            backupFilePath = backupFilePath.Replace("{CommonDesktopDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            backupFilePath = backupFilePath.Replace("{CommonDocuments}",        Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            backupFilePath = backupFilePath.Replace("{Desktop}",                Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            backupFilePath = backupFilePath.Replace("{DesktopDirectory}",       Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            backupFilePath = backupFilePath.Replace("{LocalApplicationData}",   Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            backupFilePath = backupFilePath.Replace("{MyDocuments}",            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            backupFilePath = backupFilePath.Replace("{Personal}",               Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            backupFilePath = backupFilePath.Replace("{UserProfile}",            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            backupFilePath = backupFilePath.Replace("{FilePath}",  "{0}");
            backupFilePath = backupFilePath.Replace("{Directory}", "{1}");
            backupFilePath = backupFilePath.Replace("{FileName}",  "{2}");
            backupFilePath = backupFilePath.Replace("{FileBody}",  "{3}");
            backupFilePath = backupFilePath.Replace("{Extension}", "{4}");

            var dateTimeFormat = ParseDateTimeFormat(backupFilePath);
            var indexFormat = ParseIndexFormat(backupFilePath);
            if (indexFormat == null)
            {
                throw new ArgumentException("バックアップの書式には {BackupIndex} を含めてください。", nameof(backupFilePath));
            }

            backupFilePath = backupFilePath.Replace(dateTimeFormat, dateTimeFormat.Replace("DateTime", "5"));
            backupFilePath = backupFilePath.Replace(indexFormat, indexFormat.Replace("Index", "6"));

            BackupFilePathFormat0 = backupFilePath.Replace(indexFormat.Replace("Index", "6"), "");
            BackupFilePathFormat1 = backupFilePath;

            //
            Open();
        }

        #endregion

        #region メソッド

        #region オープン

        /// <summary>
        /// ログファイルを開きます。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        public void Open()
        {
            // 出力先ディレクトリが存在しない場合、新規作成します。
            var directory = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ログファイルを閉じます。
            Close();

            // ログファイルを開きます。
            if (!File.Exists(LogFilePath))
            {
                File.Create(LogFilePath).Dispose();
                File.SetCreationTime(LogFilePath, SystemClock.Now);
            }

            FileStream = System.IO.File.Open(LogFilePath, FileMode.Open, FileAccess.Write);
            FileStream.Seek(0, SeekOrigin.End);
            Writer = new StreamWriter(FileStream, Encoding);
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

        #region バックアップ

        /// <summary>
        /// ログファイルをバックアップします。
        /// </summary>
        public void Backup()
        {
            Assert.NotNull(FileStream);
            var filePath = LogFilePath;

            // 現在のログファイルを閉じます。
            Close();

            // 現在のログファイルをバックアップファイルに名前変更します。
            Assert.True(System.IO.File.Exists(filePath));
            var creationTime = System.IO.File.GetCreationTime(filePath);

            for (int index = 0; true; index++)
            {
                var backupFilePath = GetBackupFilePath(filePath, creationTime, index);

                if (index == 0)
                {
                    var backupFilePath0 = GetBackupFilePath(filePath, creationTime);

                    if (!System.IO.File.Exists(backupFilePath0) && !System.IO.File.Exists(backupFilePath))
                    {
                        Assert.True(System.IO.File.Exists(filePath));
                        Assert.False(System.IO.File.Exists(backupFilePath0));
                        System.IO.File.Move(filePath, backupFilePath0);
                        break;
                    }
                }
                else if (index == 1)
                {
                    if (!System.IO.File.Exists(backupFilePath))
                    {
                        System.IO.File.Move(GetBackupFilePath(filePath, creationTime), GetBackupFilePath(filePath, creationTime, 0));

                        Assert.True(System.IO.File.Exists(filePath));
                        Assert.False(System.IO.File.Exists(backupFilePath));
                        System.IO.File.Move(filePath, backupFilePath);
                        break;
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(backupFilePath))
                    {
                        Assert.True(System.IO.File.Exists(filePath));
                        Assert.False(System.IO.File.Exists(backupFilePath));
                        System.IO.File.Move(filePath, backupFilePath);
                        break;
                    }
                }
            }

            // 新たにログファイルを開きます。
            Open();
        }

        /// <summary>
        /// バックアップファイルパスを取得します。
        /// </summary>
        /// <param name="dateTime">日時。</param>
        /// <returns>バックアップファイルパス。</returns>
        public string GetBackupFilePath(string filePath, DateTime dateTime)
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

        #region ログ出力

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public void Write(string log)
        {
            Writer.Write(log);
            Writer.Flush();
        }

        #endregion

        #region 解析

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
