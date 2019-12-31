using NSubstitute;
using SoftCube.Runtime;
using SoftCube.Test;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace SoftCube.Log.Appenders.UnitTests
{
    public class FileAppenderWithDalyBackupTests
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

        #endregion

        public class Log
        {
            [Fact]
            public void 日付変化_バックアップする()
            {
                var filePath = GetFilePath();

                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppenderWithDailyBackup(clock))
                {
                    appender.Open(filePath);
                    appender.Trace("A");

                    clock.Now.Returns(new DateTime(2020, 1, 2));
                    appender.Trace("B");

                    Assert.Equal("A", File.ReadAllText(appender.GetBackupFilePath(new DateTime(2020, 1, 1))));

                    clock.Now.Returns(new DateTime(2020, 1, 3));
                    appender.Trace("C");

                    Assert.Equal("A", File.ReadAllText(appender.GetBackupFilePath(new DateTime(2020, 1, 1))));
                    Assert.Equal("B", File.ReadAllText(appender.GetBackupFilePath(new DateTime(2020, 1, 2))));
                }

                Assert.Equal("C", File.ReadAllText(filePath));
            }

            [Fact]
            public void 日付変化しない_バックアップしない()
            {
                var filePath = GetFilePath();

                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2020, 1, 1));

                using (var appender = new FileAppenderWithDailyBackup(clock))
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
