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
        private static string GetFilePath([CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var filePath = TestFile.GetFilePath(".log", 1, callerMemberName, callerLineNumber);

            foreach (var path in Directory.GetFiles(Path.GetDirectoryName(filePath), "*", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(path);
                if (fileName.StartsWith(Path.GetFileNameWithoutExtension(filePath)))
                {
                    File.Delete(path);
                }
            }

            return filePath;
        }

        /// <summary>
        /// ログファイルを生成します。
        /// </summary>
        /// <param name="filePath">ログファイルパス。</param>
        /// <param name="log">ログ。</param>
        private static void CreateFile(string filePath, string log)
        {
            using (var appender = new FileAppender())
            {
                appender.Open(filePath);
                appender.Trace(log);
            }
        }

        /// <summary>
        /// ログファイルを生成します。
        /// </summary>
        /// <param name="filePath">ログファイルパス。</param>
        /// <param name="log">ログ。</param>
        /// <param name="dateTime">ログファイルの作成日時。</param>
        private static void CreateFile(string filePath, string log, DateTime dateTime)
        {
            var clock = Substitute.For<ISystemClock>();
            clock.Now.Returns(dateTime);

            using (var appender = new FileAppender(clock))
            {
                appender.Open(filePath);
                appender.Trace(log);
            }
        }

        #endregion

        public class FileOpenPolicy
        {
            [Fact]
            public void Append_既存ログファイルの末尾に追加する()
            {
                var filePath = GetFilePath();
                CreateFile(filePath, "A");

                using (var appender = new FileAppender())
                {
                    appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Append;
                    appender.Open(filePath);
                    appender.Trace("B");
                }

                var actual = File.ReadAllText(filePath);
                Assert.Equal("AB", actual);
            }

            [Fact]
            public void Backup_既存ログファイルをバックアップする()
            {
                var filePath = GetFilePath();
                CreateFile(filePath, "A", new DateTime(2020, 1, 1));

                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppender(clock))
                {
                    appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Backup;
                    appender.BackupFilePath = @"{Directory}/{FileBody}.{DateTime:yyyy-MM-dd}{Index:\.000}.log";
                    appender.Open(filePath);
                    appender.Trace("B");
                    Assert.Equal("A", File.ReadAllText(appender.GetBackupFilePath(new DateTime(2020, 1, 1))));

                    appender.Open(filePath);
                    appender.Trace("C");
                    Assert.Equal("A", File.ReadAllText(appender.GetBackupFilePath(new DateTime(2020, 1, 1), 0)));
                    Assert.Equal("B", File.ReadAllText(appender.GetBackupFilePath(new DateTime(2020, 1, 1), 1)));
                }

                Assert.Equal("C", File.ReadAllText(filePath));
            }

            [Fact]
            public void Overwrite_既存ログファイルを上書きする()
            {
                var filePath = GetFilePath();
                CreateFile(filePath, "A");

                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppender(clock))
                {
                    appender.FileOpenPolicy = SoftCube.Log.FileOpenPolicy.Overwrite;
                    appender.Open(filePath);

                    Assert.False(File.Exists(appender.GetBackupFilePath(new DateTime(2020, 1, 1))));
                    Assert.False(File.Exists(appender.GetBackupFilePath(new DateTime(2020, 1, 1), 0)));

                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(filePath));
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

        public class FilePath
        {
            [Fact]
            public void Openしていない_InvalidOperationExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.FilePath);

                    Assert.IsType<InvalidOperationException>(ex);
                }
            }

            [Fact]
            public void Openしている_ファイルパスを返す()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);

                    var actual = appender.FilePath;

                    Assert.Equal(filePath, actual);
                }
            }
        }

        public class FileSize
        {
            [Fact]
            public void Openしていない_InvalidOperationExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.FileSize);

                    Assert.IsType<InvalidOperationException>(ex);
                }
            }

            [Fact]
            public void Openしている_ファイルサイズを返す()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Encoding = System.Text.Encoding.ASCII;
                    appender.Open(filePath);
                    appender.Log("A");

                    var actual = appender.FileSize;

                    Assert.Equal(1, actual);
                }
            }
        }

        public class CreationTime
        {
            [Fact]
            public void Openしていない_InvalidOperationExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.CreationTime);

                    Assert.IsType<InvalidOperationException>(ex);
                }
            }

            [Fact]
            public void Openしている_ファイル作成日時を返す()
            {
                var filePath = GetFilePath();

                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppender(clock))
                {
                    appender.Open(filePath);

                    var actual = appender.CreationTime;

                    Assert.Equal(new DateTime(2020, 1, 1), actual);
                }
            }
        }

        public class BackupFilePath
        {
            [Fact]
            public void null_ArgumentNullExceptionを投げる()
            {
                var appender = new FileAppender();

                var ex = Record.Exception(() => appender.BackupFilePath = null);

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 禁止文字_ArgumentExceptionを投げる()
            {
                var appender = new FileAppender();

                var ex = Record.Exception(() => appender.BackupFilePath = "?");

                Assert.IsType<ArgumentException>(ex);
            }

            [Fact]
            public void Indexを含めない_ArgumentExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.BackupFilePath = "");

                    Assert.IsType<ArgumentException>(ex);
                }
            }

            [Fact]
            public void Indexを含める_許容する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = "{Index}";
                    appender.Open(filePath);

                    Assert.Equal("", appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                    Assert.Equal("7", appender.GetBackupFilePath(new DateTime(2020, 1, 1), 7));
                }
            }

            [Theory]
            [InlineData("{ApplicationData}", Environment.SpecialFolder.ApplicationData)]
            [InlineData("{CommonApplicationData}", Environment.SpecialFolder.CommonApplicationData)]
            [InlineData("{CommonDesktopDirectory}", Environment.SpecialFolder.CommonDesktopDirectory)]
            [InlineData("{CommonDocuments}", Environment.SpecialFolder.CommonDocuments)]
            [InlineData("{Desktop}", Environment.SpecialFolder.Desktop)]
            [InlineData("{DesktopDirectory}", Environment.SpecialFolder.DesktopDirectory)]
            [InlineData("{LocalApplicationData}", Environment.SpecialFolder.LocalApplicationData)]
            [InlineData("{MyDocuments}", Environment.SpecialFolder.MyDocuments)]
            [InlineData("{Personal}", Environment.SpecialFolder.Personal)]
            [InlineData("{UserProfile}", Environment.SpecialFolder.UserProfile)]
            public void 特殊フォルダ_正しいバックアップファイルパスを生成する(string specialFolderTag, Environment.SpecialFolder specialFolder)
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = $"{specialFolderTag}{{Index}}";
                    appender.Open(filePath);

                    Assert.Equal(Environment.GetFolderPath(specialFolder), appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                }
            }

            [Fact]
            public void FilePath_正しいバックアップファイルパスを生成する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = "{FilePath}{Index}";
                    appender.Open(filePath);

                    Assert.Equal(filePath, appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                }
            }

            [Fact]
            public void Directory_正しいバックアップファイルパスを生成する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = "{Directory}{Index}";
                    appender.Open(filePath);

                    Assert.Equal(Path.GetDirectoryName(filePath), appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                }
            }

            [Fact]
            public void FileName_正しいバックアップファイルパスを生成する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = "{FileName}{Index}";
                    appender.Open(filePath);

                    Assert.Equal(Path.GetFileName(filePath), appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                }
            }

            [Fact]
            public void FileBody_正しいバックアップファイルパスを生成する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = "{FileBody}{Index}";
                    appender.Open(filePath);

                    Assert.Equal(Path.GetFileNameWithoutExtension(filePath), appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                }
            }

            [Fact]
            public void Extension_正しいバックアップファイルパスを生成する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.BackupFilePath = "{Extension}{Index}";
                    appender.Open(filePath);

                    Assert.Equal(Path.GetExtension(filePath), appender.GetBackupFilePath(new DateTime(2020, 1, 1)));
                }
            }
        }

        public class IsOpened
        {
            [Fact]
            public void Openしていない_falseを返す()
            {
                using (var appender = new FileAppender())
                {
                    Assert.False(appender.IsOpened);
                }
            }

            [Fact]
            public void Openしている_trueを返す()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);

                    Assert.True(appender.IsOpened);
                }
            }
        }

        public class Open
        {
            [Fact]
            public void filePath_null_ArgumentNullExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.Open(filePath: null));

                    Assert.IsType<ArgumentNullException>(ex);
                }
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

            [Fact]
            public void filePath_正しいファイルパス_ログファイルを開く()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);
                    appender.Trace("A");
                }

                Assert.Equal("A", File.ReadAllText(filePath));
            }

            [Fact]
            public void 連続してOpenする_CloseしたあとにOpenする()
            {
                var filePathA = GetFilePath();
                var filePathB = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePathA);
                    appender.Trace("A");

                    appender.Open(filePathB);
                    appender.Trace("B");
                }

                Assert.Equal("A", File.ReadAllText(filePathA));
                Assert.Equal("B", File.ReadAllText(filePathB));
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
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);

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
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);
                    appender.Close();
                    var ex = Record.Exception(() => appender.Close());

                    Assert.Null(ex);
                }
            }
        }

        public class Log
        {
            [Fact]
            public void log_null_ArgumentNullExceptionを投げる()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);

                    var ex = Record.Exception(() => appender.Log(null));
                    
                    Assert.IsType<ArgumentNullException>(ex);
                }
            }

            [Fact]
            public void log_A_成功する()
            {
                var filePath = GetFilePath();

                using (var appender = new FileAppender())
                {
                    appender.Open(filePath);
                    appender.Log("A");
                }

                Assert.Equal("A", File.ReadAllText(filePath));
            }

            [Fact]
            public void Openせずに呼び出す_許容する()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.Log("A"));
                }
            }
        }
    }
}
