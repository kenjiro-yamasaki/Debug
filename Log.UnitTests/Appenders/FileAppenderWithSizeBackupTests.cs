using NSubstitute;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace SoftCube.Log
{
    public class FileAppenderWithSizeBackupTests
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
            var stackFrame = new StackFrame(1, true);
            var type       = stackFrame.GetMethod().DeclaringType.FullName;
            var filePath   = Path.Combine(Environment.CurrentDirectory, $"{type}_{callerMemberName}{callerLineNumber}.log");

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

        #endregion

        public class Log
        {
            [Fact]
            public void 容量超過_バックアップする()
            {
                var filePath = GetFilePath();

                var clock = Substitute.For<IClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppenderWithSizeBackup(clock))
                {
                    appender.MaxFileSize = 1;
                    appender.Open(filePath);
                    appender.Trace("A");
                    appender.Trace("B");
                    Assert.Equal("A", File.ReadAllText(appender.GetBackupFilePath(clock.Now)));

                    appender.Trace("C");
                    Assert.Equal("A", File.ReadAllText(appender.GetBackupFilePath(clock.Now, 0)));
                    Assert.Equal("B", File.ReadAllText(appender.GetBackupFilePath(clock.Now, 1)));
                }

                Assert.Equal("C", File.ReadAllText(filePath));
            }

            [Fact]
            public void 容量超過しない_バックアップしない()
            {
                var filePath = GetFilePath();

                var clock = Substitute.For<IClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppenderWithSizeBackup(clock))
                {
                    appender.Open(filePath);
                    appender.Trace("A");
                    appender.Trace("B");
                    appender.Trace("C");
                }

                Assert.Equal("ABC", File.ReadAllText(filePath));
            }
        }
    }
}
