using SoftCube.Asserts;
using SoftCube.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoftCube.Log
{
    /// <summary>
    /// ファイルアペンダー。
    /// </summary>
    public class FileAppender : Appender
    {
        #region プロパティ

        /// <summary>
        /// ファイルパス。
        /// </summary>
        public string FilePath => FileStream.Name;

        /// <summary>
        /// ファイルサイズ（単位：byte）。
        /// </summary>
        public long FileSize => FileStream.Position;

        /// <summary>
        /// ファイル作成日時。
        /// </summary>
        public DateTime CreationTime => File.GetCreationTime(FilePath);

        /// <summary>
        /// エンコーディング。
        /// </summary>
        public Encoding Encoding => Writer.Encoding;

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
        public FileAppender()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public FileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xparams">パラメーター名→値変換。</param>
        public FileAppender(IReadOnlyDictionary<string, string> xparams)
            : base(xparams)
        {
            if (xparams == null)
            {
                throw new ArgumentNullException(nameof(xparams));
            }

            var filePath = ParseFilePath(xparams["FilePath"]);
            var append   = bool.Parse(xparams["Append"]);
            var encoding = Encoding.GetEncoding(xparams["Encoding"]);

            Open(filePath, append, encoding);
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
                Writer.Write(log);
                Writer.Flush();
            }
        }

        #endregion

        #region ログファイルを開く

        /// <summary>
        /// ログファイルを開きます。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="append">
        /// ファイルにログを追加するか。
        /// <c>true</c> の場合、ファイルにログを追加します。
        /// <c>false</c> の場合、ファイルのログを上書きします。
        /// </param>
        /// <param name="encoding">エンコーディング。</param>
        public void Open(string filePath, bool append, Encoding encoding)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Close();

            if (!File.Exists(filePath) || !append)
            {
                File.Create(filePath).Dispose();
                File.SetCreationTime(filePath, SystemClock.Now);
            }
            FileStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            FileStream.Seek(0, SeekOrigin.End);

            Writer = new StreamWriter(FileStream, encoding);
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

        #endregion
    }
}
