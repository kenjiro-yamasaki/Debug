using NSubstitute;
using SoftCube.Runtime;
using SoftCube.Test;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace SoftCube.Log.Appenders.UnitTests
{
    public class FileAppenderTests
    {
        #region テストユーティリティ

        /// <summary>
        /// ログファイルパスを取得します。
        /// </summary>
        /// <param name="callerMemberName">呼び出し元のメソッド名。</param>
        /// <param name="callerLineNumber">呼び出し元の行番号。</param>
        /// <returns>ログファイルパス。</returns>
        private static string GetLogFilePath([CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var logFilePath = TestFile.GetFilePath(".log", 1, callerMemberName, callerLineNumber);
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
        /// ログファイルを生成します。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        /// <param name="log">ログ。</param>
        private static void CreateLogFile(string logFilePath, string log, DateTime dateTime)
        {
            var clock = Substitute.For<ISystemClock>();
            using (var appender = new FileAppender(clock))
            {
                clock.Now.Returns(dateTime);
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

            foreach (var filePath in Directory.GetFiles(Path.GetDirectoryName(logFilePath), "*", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(filePath);
                if (fileName.StartsWith(Path.GetFileNameWithoutExtension(logFilePath)))
                {
                    File.Delete(filePath);
                }
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
            var backupFileName = string.Format("{0}.{1}{2}", baseName, backupDateTime.ToString("yyyy-MM-dd"), extension);

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
            var backupFileName = string.Format("{0}.{1}.{2}{3}", baseName, backupDateTime.ToString("yyyy-MM-dd"), backupIndex.ToString("000"), extension);

            return Path.Combine(directoryName, backupFileName);
        }

        #endregion

        public class FileOpenPolicy
        {
            [Fact]
            public void Append_既存ログファイルの末尾に追加する()
            {
                var logFilePath = GetLogFilePath();
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender())
                {
                    appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Append;
                    appender.Open(logFilePath);
                    appender.Trace("B");
                }

                var actual = File.ReadAllText(logFilePath);
                Assert.Equal("AB", actual);
            }

            [Fact]
            public void Backup_既存ログファイルをバックアップする()
            {
                var logFilePath = GetLogFilePath();
                CreateLogFile(logFilePath, "A", new DateTime(2020, 1, 1));

                var clock = Substitute.For<ISystemClock>();
                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Backup;
                    appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
                    appender.Open(logFilePath);

                    Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));

                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void Overwrite_既存ログファイルを上書きする()
            {
                var logFilePath = GetLogFilePath();
                CreateLogFile(logFilePath, "A");

                var clock = Substitute.For<ISystemClock>();
                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Overwrite;
                    appender.Open(logFilePath);

                    Assert.False(File.Exists(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));
                    Assert.False(File.Exists(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 0)));

                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
            }
        }

        public class Encoding
        {
            [Fact]
            public void null_ArgumentNullExceptionを投げる()
            {
                var appender = new FileAppender();

                var ex = Record.Exception(() => appender.Encoding = null);

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void UTF8_許容する()
            {
                var appender = new FileAppender();

                appender.Encoding = System.Text.Encoding.UTF8;

                Assert.Equal(System.Text.Encoding.UTF8, appender.Encoding);
            }
        }

        //public class MaxFileSize
        //{
        //    [Fact]
        //    public void MaxFileSizeを超える_バックアップファイルを作成する()
        //    {
        //        var logFilePath = GetLogFilePath();
        //        var clock = Substitute.For<ISystemClock>();

        //        using (var appender = new FileAppender(clock))
        //        {
        //            clock.Now.Returns(new DateTime(2020, 1, 1));
        //            appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
        //            appender.MaxFileSize = 1;
        //            appender.Open(logFilePath);
        //            appender.Trace("A");

        //            clock.Now.Returns(new DateTime(2020, 1, 2));
        //            appender.Trace("D");

        //            Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));
        //        }

        //        Assert.Equal("D", File.ReadAllText(logFilePath));
        //    }

        //    [Fact]
        //    public void MaxFileSizeを超えない_バックアップファイルを作成しない()
        //    {
        //        var logFilePath = GetLogFilePath();

        //        using (var appender = new FileAppender())
        //        {
        //            appender.Encoding = System.Text.Encoding.ASCII;
        //            appender.MaxFileSize = 2;
        //            appender.Open(logFilePath);

        //            appender.Trace("A");
        //            appender.Trace("B");
        //        }

        //        Assert.Equal("AB", File.ReadAllText(logFilePath));
        //    }
        //}

        //public class DateTimeFormat
        //{
        //    [Fact]
        //    public void yyyyMMdd_バックアップ動作が正しい()
        //    {
        //        var logFilePath = GetLogFilePath();
        //        var clock = Substitute.For<ISystemClock>();

        //        using (var appender = new FileAppender(clock))
        //        {
        //            clock.Now.Returns(new DateTime(2020, 1, 30, 23, 59, 58));
        //            appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
        //            appender.Open(logFilePath);
        //            appender.Trace("A");

        //            clock.Now.Returns(new DateTime(2020, 1, 30, 23, 59, 59));
        //            appender.Trace("B");

        //            clock.Now.Returns(new DateTime(2020, 1, 31, 0, 0, 0));
        //            appender.Trace("C");

        //            Assert.Equal("AB", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 30, 0, 0, 0))));
        //        }

        //        Assert.Equal("C", File.ReadAllText(logFilePath));
        //    }

        //    //[Fact]
        //    //public void null_ArgumentNullExceptionを投げる()
        //    //{
        //    //    using (var appender = new FileAppender())
        //    //    {
        //    //        var ex = Record.Exception(() => appender.DateTimeFormat = null);

        //    //        Assert.IsType<ArgumentNullException>(ex);
        //    //    }
        //    //}

        //    //[Fact]
        //    //public void 不正な書式_ArgumentExceptionを投げる()
        //    //{
        //    //    using (var appender = new FileAppender())
        //    //    {
        //    //        var ex = Record.Exception(() => appender.DateTimeFormat = "?");

        //    //        Assert.IsType<ArgumentException>(ex);
        //    //    }
        //    //}
        //}

        //public class IndexFormat
        //{
        //    [Fact]
        //    public void Log_000_バックアップ動作が正しい()
        //    {
        //        var logFilePath = GetLogFilePath();
        //        var clock = Substitute.For<ISystemClock>();
        //        clock.Now.Returns(new DateTime(2020, 1, 1));

        //        using (var appender = new FileAppender(clock))
        //        {
        //            appender.MaxFileSize = 1;
        //            appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
        //            appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Backup;
        //            appender.Open(logFilePath);

        //            appender.Log("A");
        //            appender.Log("B");
        //            Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));

        //            appender.Log("C");
        //            Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 0)));
        //            Assert.Equal("B", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 1)));
        //        }

        //        Assert.Equal("C", File.ReadAllText(logFilePath));
        //    }

        //    [Fact]
        //    public void Open_000_バックアップ動作が正しい()
        //    {
        //        var logFilePath = GetLogFilePath();
        //        var clock = Substitute.For<ISystemClock>();
        //        clock.Now.Returns(new DateTime(2020, 1, 1));

        //        using (var appender = new FileAppender(clock))
        //        {
        //            appender.MaxFileSize = 1;
        //            appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
        //            appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Backup;

        //            appender.Open(logFilePath);
        //            appender.Log("A");

        //            appender.Open(logFilePath);
        //            appender.Log("B");
        //            Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));

        //            appender.Open(logFilePath);
        //            appender.Log("C");
        //            Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 0)));
        //            Assert.Equal("B", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 1)));
        //        }

        //        Assert.Equal("C", File.ReadAllText(logFilePath));
        //    }

        //    //[Fact]
        //    //public void null_ArgumentNullExceptionを投げる()
        //    //{
        //    //    using (var appender = new FileAppender())
        //    //    {
        //    //        var ex = Record.Exception(() => appender.IndexFormat = null);

        //    //        Assert.IsType<ArgumentNullException>(ex);
        //    //    }
        //    //}

        //    //[Fact]
        //    //public void 不正な書式_ArgumentExceptionを投げる()
        //    //{
        //    //    using (var appender = new FileAppender())
        //    //    {
        //    //        var ex = Record.Exception(() => appender.IndexFormat = "?");

        //    //        Assert.IsType<ArgumentException>(ex);
        //    //    }
        //    //}
        //}

        public class Open
        {
            #region filePath

            //[Fact]
            //public void filePath_null_ArgumentNullExceptionを投げる()
            //{
            //    Assert.Throws<ArgumentNullException>(() => new FileAppender().Open(filePath: null));
            //}

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

            //[Fact]
            //public void filePath_不正なファイルパス_ArgumentExceptionを投げる()
            //{
            //    using (var appender = new FileAppender())
            //    {
            //        var ex = Record.Exception(() => appender.Open(filePath: "?"));

            //        Assert.IsType<ArgumentException>(ex);
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
            //[Fact]
            //public void 容量超過_正しくバックアップする()
            //{
            //    var logFilePath = GetLogFilePath();
            //    var clock = Substitute.For<ISystemClock>();

            //    using (var appender = new FileAppender(clock))
            //    {
            //        clock.Now.Returns(new DateTime(2020, 1, 1));
            //        appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
            //        appender.MaxFileSize = 1;
            //        appender.Open(logFilePath);
            //        appender.Trace("A");
            //        appender.Trace("B");
            //        Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1))));

            //        clock.Now.Returns(new DateTime(2020, 1, 2));
            //        appender.Trace("C");

            //        Assert.Equal("A", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 0)));
            //        Assert.Equal("B", File.ReadAllText(GetBackupFilePath(appender, new DateTime(2020, 1, 1), 1)));
            //    }

            //    Assert.Equal("C", File.ReadAllText(logFilePath));
            //}

            [Fact]
            public void 日付変化_正しくバックアップする()
            {
                var logFilePath = GetLogFilePath();
                var clock = Substitute.For<ISystemClock>();

                using (var appender = new FileAppender(clock))
                {
                    clock.Now.Returns(new DateTime(2020, 1, 1));
                    appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
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
    }
}
