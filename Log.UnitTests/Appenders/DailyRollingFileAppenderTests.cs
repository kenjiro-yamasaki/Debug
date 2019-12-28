using NSubstitute;
using SoftCube.Runtime;
using SoftCube.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SoftCube.Log.Appenders.UnitTests
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
            public void yyyy_バックアップ動作が正しい()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);
                var clock = Substitute.For<ISystemClock>();
                var datePattern = "yyyy";

                using (var appender = new DateTimeRollingFileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2019, 12, 30));
                    appender.DateTimeFormat = datePattern;
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2019, 12, 31));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.Trace("C");
                }

                Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2019, 12, 31), datePattern)));
                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void yyyyMM_バックアップ動作が正しい()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);
                var clock = Substitute.For<ISystemClock>();
                var datePattern = "yyyyMM";

                using (var appender = new DateTimeRollingFileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 30));
                    appender.DateTimeFormat = datePattern;
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 31));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 2, 1));
                    appender.Trace("C");
                }

                Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2020, 1, 31), datePattern)));
                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void yyyyMMdd_バックアップ動作が正しい()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);
                var clock = Substitute.For<ISystemClock>();
                var datePattern = "yyyyMMdd";

                using (var appender = new DateTimeRollingFileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 30, 23, 59, 58));
                    appender.DateTimeFormat = datePattern;
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 30, 23, 59, 59));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 0, 0, 0));
                    appender.Trace("C");
                }

                Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2020, 1, 30, 0, 0, 0), datePattern)));
                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void yyyyMMddHH_バックアップ動作が正しい()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);
                var clock = Substitute.For<ISystemClock>();
                var datePattern = "yyyyMMddHH";

                using (var appender = new DateTimeRollingFileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 31, 22, 59, 58));
                    appender.DateTimeFormat = datePattern;
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 22, 59, 59));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 0, 0));
                    appender.Trace("C");
                }

                Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2020, 1, 31, 22, 0, 0), datePattern)));
                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void yyyyMMddHHmm_バックアップ動作が正しい()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);
                var clock = Substitute.For<ISystemClock>();
                var datePattern = "yyyyMMddHHmm";

                using (var appender = new DateTimeRollingFileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 58, 58));
                    appender.DateTimeFormat = datePattern;
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 58, 59));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 59, 0));
                    appender.Trace("C");
                }

                Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2020, 1, 31, 23, 58, 0), datePattern)));
                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void yyyyMMddHHmmss_バックアップ動作が正しい()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                ClearLogAndBackupFiles(logFilePath);
                var clock = Substitute.For<ISystemClock>();
                var datePattern = "yyyyMMddHHmmss";

                using (var appender = new DateTimeRollingFileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 59, 58, 998));
                    appender.DateTimeFormat = datePattern;
                    appender.Open(logFilePath, append: false, Encoding.ASCII);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 59, 58, 999));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 23, 59, 59, 000));
                    appender.Trace("C");
                }

                Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(logFilePath, new DateTime(2020, 1, 31, 23, 59, 58, 000), datePattern)));
                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void 不正な書式_を投げる()
            {
                using (var appender = new DateTimeRollingFileAppender())
                {
                    var ex = Record.Exception(() => appender.DateTimeFormat = "?");
                    
                    Assert.IsType<ArgumentException>(ex);
                }
            }
        }

        [Fact]
        public void 作成日時が変わらない_バックアップファイルが作成されない()
        {
            var logFilePath = TestFile.GetFilePath(".log");
            ClearLogAndBackupFiles(logFilePath);
            var clock = Substitute.For<ISystemClock>();
            clock.Now.Returns(new DateTime(2019, 12, 21));

            using (var appender = new DateTimeRollingFileAppender(clock))
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
            var datePattern = "yyyyMMdd";

            using (var appender = new DateTimeRollingFileAppender(clock))
            {
                clock.Now.Returns(new DateTime(2019, 12, 21));
                appender.DateTimeFormat = datePattern;
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
