using System;

namespace SoftCube.Logger.Appenders
{
    /// <summary>
    /// 文字列の解析。
    /// </summary>
    public static class StringParser
    {
        #region 静的メソッド

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
        public static string ParseFilePath(string filePath)
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
        public static long ParseMaxFileSize(string maxFileSize)
        {
            const long Byte = 1;
            const long KB   = Byte * 1024;
            const long MB   = KB * 1024;
            const long GB   = MB * 1024;

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
    }
}
