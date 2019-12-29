using NSubstitute;
using SoftCube.Runtime;
using SoftCube.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace SoftCube.Log.Appenders.UnitTests
{
    public class FileAppenderTests
    {
        #region テスト用ユーティリティ

        /// <summary>
        /// ログファイルパスを取得します。
        /// </summary>
        /// <param name="callingMemberName"></param>
        /// <param name="callingLineNumber"></param>
        /// <returns>ログファイルパス。</returns>
        private static string GetLogFilePath([CallerMemberName] string callingMemberName = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            var logFilePath = TestFile.GetFilePath(".log", 1, callingMemberName, callingLineNumber);
            ClearLogAndBackupFiles(logFilePath);
            return logFilePath;
        }

        /// <summary>
        /// ログファイルを生成します。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        /// <param name="log">ログ。</param>
        private static void CreateLogFile(string logFilePath, string log)
        {
            using (var appender = new FileAppender())
            {
                appender.Open(logFilePath);
                appender.Trace(log);
            }
        }

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
        /// <param name="appender">ファイルアペンダー。</param>
        /// <param name="backupDateTime">バックアップ日時。</param>
        /// <returns>バックアップファイルパス。</returns>
        private static string GetBackupFilePath(FileAppender appender, DateTime backupDateTime)
        {
            var filePath = appender.FilePath;
            Assert.True(File.Exists(filePath));
            var directoryName = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(filePath);
            var backupFileName = string.Format("{0}.{1}{2}", baseName, backupDateTime.ToString(appender.DateTimeFormat), extension);

            return Path.Combine(directoryName, backupFileName);
        }

        /// <summary>
        /// バックアップファイルパスを取得します。
        /// </summary>
        /// <param name="appender">ファイルアペンダー。</param>
        /// <param name="backupDateTime">バックアップ日時。</param>
        /// <param name="backupIndex">バックアップインデックス。</param>
        /// <returns>バックアップファイルパス。</returns>
        private static string GetBackupFilePath(FileAppender appender, DateTime backupDateTime, int backupIndex)
        {
            var filePath = appender.FilePath;
            Assert.True(File.Exists(filePath));
            var directoryName = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(filePath);
            var backupFileName = string.Format("{0}.{1}.{2}{3}", baseName, backupDateTime.ToString(appender.DateTimeFormat), backupIndex.ToString(appender.IndexFormat), extension);

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
            var extension = Path.GetExtension(logFilePath);

            string[] files = Directory.GetFiles(directoryName, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                if (fileName.StartsWith(Path.GetFileNameWithoutExtension(logFilePath)))
                {
                    yield return file;
                }
            }
        }

        #endregion

        public class Open
        {
            #region filePath

            [Fact]
            public void filePath_null_ArgumentNullExceptionを投げる()
            {
                Assert.Throws<ArgumentNullException>(() => new FileAppender().Open(filePath: null));
            }

            [Fact]
            public void filePath_正しいファイルパス_ログファイルを開く()
            {
                var logFilePath = GetLogFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePath);
                    appender.Trace("A");
                }

                Assert.Equal("A", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void filePath_不正なファイルパス_ArgumentExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.Open(filePath: "?"));

                    Assert.IsType<ArgumentException>(ex);
                }
            }

            #endregion

            #region append

            [Fact]
            public void append_false_ログを上書きする()
            {
                var logFilePath = GetLogFilePath();
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender())
                {
                    appender.FileOpenPolicy = FileOpenPolicy.Overwrite;
                    appender.Open(logFilePath);
                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void append_true_ログを追加する()
            {
                var logFilePath = GetLogFilePath();
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePath);
                    appender.Trace("B");
                }

                var actual = File.ReadAllText(logFilePath);
                Assert.Equal("AB", actual);
            }

            #endregion

            #region encoding

            //[Fact]
            //public void encoding_null_ArgumentNullExceptionを投げる()
            //{
            //    var logFilePath = GetLogFilePath();

            //    using (var appender = new FileAppender())
            //    {
            //        var ex = Record.Exception(() => appender.Open(logFilePath, append: true, encoding: null));

            //        Assert.IsType<ArgumentNullException>(ex);
            //    }
            //}

            #endregion

            [Fact]
            public void 連続してOpenする_CloseしたあとにOpenする()
            {
                var logFilePathA = GetLogFilePath();
                var logFilePathB = GetLogFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePathA);
                    appender.Trace("A");

                    appender.Open(logFilePathB);
                    appender.Trace("B");
                }

                Assert.Equal("A", File.ReadAllText(logFilePathA));
                Assert.Equal("B", File.ReadAllText(logFilePathB));
            }

            [Fact]
            public void Openしないでログ出力する_許容する()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.Trace("B"));

                    Assert.Null(ex);
                }
            }
        }

        public class Close
        {
            [Fact]
            public void OpenしてCloseする_成功する()
            {
                var logFilePath = GetLogFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePath);

                    var ex = Record.Exception(() => appender.Close());

                    Assert.Null(ex);
                }
            }

            [Fact]
            public void OpenしないでCloseする_許容する()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.Close());

                    Assert.Null(ex);
                }
            }

            [Fact]
            public void 連続してCloseする_許容する()
            {
                var logFilePath = GetLogFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePath);
                    appender.Close();
                    var ex = Record.Exception(() => appender.Close());

                    Assert.Null(ex);
                }
            }
        }

        public class Log
        {
            [Fact]
            public void 容量超過_正しくバックアップする()
            {
                var logFilePath = GetLogFilePath();
                var clock = Substitute.For<ISystemClock>();

                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.DateTimeFormat = "yyyyMMdd";
                    appender.MaxFileSize = 1;
                    appender.Open(logFilePath);
                    appender.Trace("A");
                    appender.Trace("B");
                    Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));

                    clock.Now.Returns(new DateTime(2020, 1, 2));
                    appender.Trace("C");

                    Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 0)));
                    Assert.Equal("B", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 1)));
                }

                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void 日付変化_正しくバックアップする()
            {
                var logFilePath = GetLogFilePath();
                var clock = Substitute.For<ISystemClock>();

                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.DateTimeFormat = "yyyyMMdd";
                    appender.Open(logFilePath);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 2));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 3));
                    appender.Trace("C");

                    Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));
                    Assert.Equal("B", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 2))));
                }

                Assert.Equal("C", File.ReadAllText(logFilePath));
            }
        }




        public class DateTimeFormat
        {
            [Fact]
            public void yyyyMMdd_バックアップ動作が正しい()
            {
                var logFilePath = GetLogFilePath();
                var clock = Substitute.For<ISystemClock>();

                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 30, 23, 59, 58));
                    appender.DateTimeFormat = "yyyyMMdd";
                    appender.Open(logFilePath);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 30, 23, 59, 59));
                    appender.Trace("B");

                    clock.Now.Returns(new DateTime(2020, 1, 31, 0, 0, 0));
                    appender.Trace("C");

                    Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 30, 0, 0, 0))));
                }

                Assert.Equal("C", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void 不正な書式_を投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.DateTimeFormat = "?");

                    Assert.IsType<ArgumentException>(ex);
                }
            }
        }

        public class MaxFileSize
        {
            [Fact]
            public void MaxFileSizeを超える_バックアップファイルを作成する()
            {
                var logFilePath = GetLogFilePath();
                var clock = Substitute.For<ISystemClock>();

                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.DateTimeFormat = "yyyyMMdd";
                    appender.MaxFileSize = 1;
                    appender.Open(logFilePath);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 2));
                    appender.Trace("D");

                    Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));
                }

                Assert.Equal("D", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void MaxFileSizeを超えない_バックアップファイルを作成しない()
            {
                var logFilePath = GetLogFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Encoding = Encoding.ASCII;
                    appender.MaxFileSize = 2;
                    appender.Open(logFilePath);

                    appender.Trace("A");
                    appender.Trace("B");
                }

                Assert.Equal("AB", File.ReadAllText(logFilePath));
            }
        }
    }
}
