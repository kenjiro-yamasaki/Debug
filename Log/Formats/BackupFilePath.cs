using System;
using System.IO;

namespace SoftCube.Log
{
    /// <summary>
    /// バックアップファイルの書式。
    /// </summary>
    public class BackupFilePath
    {
        #region プロパティ

        /// <summary>
        /// 書式。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// 文字列フォーマット。
        /// </summary>
        private string StringFormat0 { get; }

        /// <summary>
        /// 文字列フォーマット。
        /// </summary>
        private string StringFormat1 { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="format">書式。<seealso cref="Format"/></param>
        internal BackupFilePath(string format)
        {
            Format = format ?? throw new ArgumentNullException(nameof(format));

            //
            format = format.Replace("{ApplicationData}",        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            format = format.Replace("{CommonApplicationData}",  Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            format = format.Replace("{CommonDesktopDirectory}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            format = format.Replace("{CommonDocuments}",        Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            format = format.Replace("{Desktop}",                Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            format = format.Replace("{DesktopDirectory}",       Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            format = format.Replace("{LocalApplicationData}",   Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            format = format.Replace("{MyDocuments}",            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            format = format.Replace("{Personal}",               Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            format = format.Replace("{UserProfile}",            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            format = format.Replace("{Directory}", "{0}");
            format = format.Replace("{FilePath}",  "{1}");
            format = format.Replace("{FileName}",  "{2}");
            format = format.Replace("{FileBody}",  "{3}");
            format = format.Replace("{Extension}", "{4}");

            var dateTimeFormat = ParseDateTimeFormat(format);
            var indexFormat    = ParseIndexFormat(format);
            if (indexFormat == null)
            {
                throw new ArgumentException("バックアップの書式には {Index} を含めてください。", nameof(format));
            }

            format = format.Replace(dateTimeFormat, dateTimeFormat.Replace("DateTime", "5"));
            format = format.Replace(indexFormat,    indexFormat.Replace("Index", "6"));

            StringFormat0 = format.Replace(indexFormat.Replace("Index", "6"), "");
            StringFormat1 = format;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// バックアップファイルパスに変換します。
        /// </summary>
        /// <param name="dateTime">日時。</param>
        /// <returns>バックアップファイルパス。</returns>
        public string Convert(string filePath, DateTime dateTime)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName  = Path.GetFileName(filePath);
            var fileBody  = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            return string.Format(
                StringFormat0,
                directory,
                filePath,
                fileName,
                fileBody,
                extension,
                dateTime);
        }

        /// <summary>
        /// バックアップファイルパスに変換します。
        /// </summary>
        /// <param name="dateTime">日時。</param>
        /// <param name="index">インデックス。</param>
        /// <returns>バックアップファイルパス。</returns>
        public string Convert(string filePath, DateTime dateTime, int index)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName  = Path.GetFileName(filePath);
            var fileBody  = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            return string.Format(
                StringFormat1,
                directory,
                filePath,
                fileName,
                fileBody,
                extension,
                dateTime,
                index);
        }

        /// <summary>
        /// 日時の書式を解析します。
        /// </summary>
        /// <param name="format">書式。</param>
        /// <returns>日時の書式。</returns>
        private string ParseDateTimeFormat(string format)
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
        private string ParseIndexFormat(string format)
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
    }
}
