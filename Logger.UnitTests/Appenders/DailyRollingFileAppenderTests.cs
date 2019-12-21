using NSubstitute;
using SoftCube.System;
using SoftCube.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace SoftCube.Logger.UnitTests
{
    public class DailyRollingFileAppenderTests
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
        private static string GetBackupFilePath(string logFilePath, DateTime backupDateTime, string backupDatePattern)
        {
            var directoryName = Path.GetDirectoryName(logFilePath);
            var fileName      = Path.GetFileName(logFilePath);
            var baseName      = Path.GetFileNameWithoutExtension(fileName);
            var extension     = Path.GetExtension(fileName);

            var backupFileName = string.Format($"{{0}}{{1}}.{{2:{backupDatePattern}}}", baseName, extension, backupDateTime);
            return Path.Combine(directoryName, backupFileName);
        }

        /// <summary>
        /// バックアップファイルパスコレクションを取得します。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        /// <returns>バックアップファイルパスコレクション。</returns>
        private static IEnumerable<string> GetBackupFilePathes(string logFilePath)
        {
            var directoryName = Path.GetDirectoryName(logFilePath);
            var extension     = Path.GetExtension(logFilePath);

            string[] files = Directory.GetFiles(directoryName, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                if (fileName.StartsWith(Path.GetFileNameWithoutExtension(logFilePath)) && fileName.EndsWith(extension))
                {
                    yield return file;
                }
            }
        }

        #endregion

        public class DatePattern
        {
            [Fact]
            public void yyyyMMdd_バックアップファイル名が正しい()
            {
                //var logFilePath = TestFile.GetFilePath(".log");
                //ClearLogAndBackupFiles(logFilePath);

                //using (var appender = new RollingFileAppender(logFilePath, append: false, Encoding.ASCII))
                //{
                //    appender.ConversionPattern = "{message}";
                //    appender.MaxFileSize       = 1;
                //    appender.MaxBackupCount    = 0;

                //    appender.Trace("A");
                //    appender.Trace("B");
                //}

                //Assert.Equal("B", File.ReadAllText(logFilePath));
                //Assert.False(File.Exists(GetBackupFilePath(logFilePath, 0)));
            }
        }

        [Fact]
        public void 作成日時が変わらない_バックアップファイルが作成されない()
        {
            var logFilePath = TestFile.GetFilePath(".log");
            ClearLogAndBackupFiles(logFilePath);
            var clock = Substitute.For<ISystemClock>();
            clock.Now.Returns(new DateTime(2019, 12, 21));

            using (var appender = new DailyRollingFileAppender(clock))
            {
                appender.Open(logFilePath, append: false, Encoding.ASCII);
                appender.Trace("A");
                appender.Trace("B");
            }

            Assert.Equal("AB", File.ReadAllText(logFilePath));
        }

        [Fact]
        public void 作成日時が変わる_バックアップファイルが作成される()
        {
            var logFilePath = TestFile.GetFilePath(".log");
            ClearLogAndBackupFiles(logFilePath);
            var clock = Substitute.For<ISystemClock>();
            clock.Now.Returns(new DateTime(2019, 12, 21));
            var datePattern = "yyyyMMdd";

            using (var appender = new DailyRollingFileAppender(clock))
            {
                appender.DatePattern = datePattern;
                appender.Open(logFilePath, append: false, Encoding.ASCII);
                appender.Trace("A");

                clock.Now.Returns(new DateTime(2019, 12, 22));
                appender.Trace("B");
            }

            Assert.Equal("A", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2019, 12, 21), datePattern)));
            Assert.Equal("B", File.ReadAllText(logFilePath));
        }
    }
}
