using SoftCube.Test;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SoftCube.Log.Appenders.UnitTests
{
    public class RollingFileAppenderTests
    {
        #region テスト用ユーティリティ

        /// <summary>
        /// ログファイルとバックアップファイルをクリアします。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        private static void ClearLogAndBackupFiles(string logFilePath)
        {
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            foreach (var path in GetBackupFilePathes(logFilePath))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// バックアップファイルパスを取得します。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        /// <param name="backupIndex">バックアップインデックス。</param>
        /// <returns>バックアップファイルパス。</returns>
        private static string GetBackupFilePath(string logFilePath, int backupIndex)
        {
            var directoryName = Path.GetDirectoryName(logFilePath);
            var baseFileName  = Path.GetFileNameWithoutExtension(logFilePath);
            var extension     = Path.GetExtension(logFilePath);

            return Path.Combine(directoryName, $"{baseFileName}{extension}.{backupIndex + 1}");
        }

        /// <summary>
        /// バックアップファイルパスコレクションを取得します。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        /// <returns>バックアップファイルパスコレクション。</returns>
        private static IEnumerable<string> GetBackupFilePathes(string logFilePath)
        {
            for (int backupIndex = 0; true; backupIndex++)
            {
                var backupFilePath = GetBackupFilePath(logFilePath, backupIndex);

                if (File.Exists(backupFilePath))
                {
                    yield return backupFilePath;
                }
                else
                {
                    yield break;
                }
            }
        }

        #endregion

        public class MaxFileSize
        {
            [Fact]
            public void MaxFileSizeを超える_バックアップファイルを作成する()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);

                using (var appender = new FileSizeRollingFileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.MaxFileSize = 1;

                    appender.Trace("A");
                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
                Assert.Equal("A", File.ReadAllText(GetBackupFilePath(logFilePath, 0)));
            }

            [Fact]
            public void MaxFileSizeを超えない_バックアップファイルを作成しない()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);

                using (var appender = new FileSizeRollingFileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.MaxFileSize = 2;

                    appender.Trace("A");
                    appender.Trace("B");
                }

                Assert.Equal("AB", File.ReadAllText(logFilePath));
            }
        }

        public class MaxBackupCount
        {
            [Fact]
            public void MaxBackupCountが0_バックアップファイルが作成されない()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);

                using (var appender = new FileSizeRollingFileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.MaxFileSize    = 1;
                    appender.MaxBackupCount = 0;

                    appender.Trace("A");
                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
                Assert.False(File.Exists(GetBackupFilePath(logFilePath, 0)));
            }

            [Fact]
            public void MaxBackupCountが1_バックアップファイルが作成される()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);

                using (var appender = new FileSizeRollingFileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.MaxFileSize    = 1;
                    appender.MaxBackupCount = 1;

                    appender.Trace("A");
                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
                Assert.Equal("A", File.ReadAllText(GetBackupFilePath(logFilePath, 0)));
            }

            [Fact]
            public void MaxBackupCountが2_バックアップファイルがローテーションされる()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);

                using (var appender = new FileSizeRollingFileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.MaxFileSize    = 1;
                    appender.MaxBackupCount = 2;

                    appender.Trace("A");
                    appender.Trace("B");
                    appender.Trace("C");
                }

                Assert.Equal("C", File.ReadAllText(logFilePath));
                Assert.Equal("B", File.ReadAllText(GetBackupFilePath(logFilePath, 0)));
                Assert.Equal("A", File.ReadAllText(GetBackupFilePath(logFilePath, 1)));
            }

            [Fact]
            public void MaxBackupCount以上のバックアップファイルは作成されない()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);

                using (var appender = new FileSizeRollingFileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.MaxFileSize    = 1;
                    appender.MaxBackupCount = 2;

                    appender.Trace("A");
                    appender.Trace("B");
                    appender.Trace("C");
                    appender.Trace("D");
                }

                Assert.Equal("D", File.ReadAllText(logFilePath));
                Assert.Equal("C", File.ReadAllText(GetBackupFilePath(logFilePath, 0)));
                Assert.Equal("B", File.ReadAllText(GetBackupFilePath(logFilePath, 1)));
                Assert.False(File.Exists(GetBackupFilePath(logFilePath, 2)));
            }
        }
    }
}
