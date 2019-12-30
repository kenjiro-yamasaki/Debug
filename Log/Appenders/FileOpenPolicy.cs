using System;

namespace SoftCube.Log
{
    /// <summary>
    /// ファイルオープン方針。
    /// </summary>
    /// <remarks>
    /// ファイルオープン時の既存ログファイルの取り扱い方針を表現します。
    /// </remarks>
    public enum FileOpenPolicy
    {
        /// <summary>
        /// 既存ログファイルの末尾に追加します。
        /// </summary>
        Append = 0,

        /// <summary>
        /// 既存ログファイルをバックアップします。
        /// </summary>
        Backup = 1,

        /// <summary>
        /// 既存ログファイルを上書きします。
        /// </summary>
        Overwrite = 2,
    }

    /// <summary>
    /// <see cref="FileOpenPolicy"/> の拡張メソッド。
    /// </summary>
    public static class FileOpenPolicyExtensions
    {
        #region メソッド

        /// <summary>
        /// ファイルオープン方針を表示名に変換します。
        /// </summary>
        /// <param name="fileOpenPolicy">ファイルオープン方針。</param>
        /// <returns>表示名。</returns>
        public static string ToDisplayName(this FileOpenPolicy fileOpenPolicy)
        {
            switch (fileOpenPolicy)
            {
                case FileOpenPolicy.Append:
                    return nameof(FileOpenPolicy.Append);

                case FileOpenPolicy.Backup:
                    return nameof(FileOpenPolicy.Backup);

                case FileOpenPolicy.Overwrite:
                    return nameof(FileOpenPolicy.Overwrite);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// ファイルオープン方針の表示名をファイルオープン方針に変換します。
        /// </summary>
        /// <param name="displayName">ファイルオープン方針の表示名。</param>
        /// <returns>ファイルオープン方針。</returns>
        public static FileOpenPolicy ToFileOpenPolicy(this string displayName)
        {
            switch (displayName)
            {
                case nameof(FileOpenPolicy.Append):
                    return FileOpenPolicy.Append;

                case nameof(FileOpenPolicy.Backup):
                    return FileOpenPolicy.Backup;

                case nameof(FileOpenPolicy.Overwrite):
                    return FileOpenPolicy.Overwrite;

                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}
