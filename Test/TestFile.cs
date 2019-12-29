using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace SoftCube.Test
{
    /// <summary>
    /// テストファイル。
    /// </summary>
    public static class TestFile
    {
        #region 静的メソッド

        /// <summary>
        /// ファイルパスを取得します。
        /// </summary>
        /// <param name="extension">拡張子。</param>
        /// <param name="callingMemberName">呼び出し元のメソッド名。</param>
        /// <param name="callingLineNumber">呼び出し元の行番号。</param>
        /// <returns>ファイルパス。</returns>
        public static string GetFilePath(string extension, int skipFrames = 0, [CallerMemberName] string callingMemberName = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            var stackFrame = new StackFrame(skipFrames + 1, true);
            var type       = stackFrame.GetMethod().DeclaringType.FullName;

            var filePath = Path.Combine(Environment.CurrentDirectory, $"{type}_{callingMemberName}{callingLineNumber}{extension}");
            DeleteIfExists(filePath);

            return filePath;
        }

        /// <summary>
        /// ファイルが存在する場合、削除します。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        public static void DeleteIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        #endregion
    }
}
