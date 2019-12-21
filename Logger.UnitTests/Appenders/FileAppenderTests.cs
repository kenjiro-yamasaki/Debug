using SoftCube.Test;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace SoftCube.Logger.UnitTests
{
    public class FileAppenderTests
    {
        #region テスト用ユーティリティ

        /// <summary>
        /// ログファイルを生成します。
        /// </summary>
        /// <param name="logFilePath">ログファイルパス。</param>
        /// <param name="log">ログ。</param>
        private static void CreateLogFile(string logFilePath, string log)
        {
            using (var appender = new FileAppender(logFilePath, append: false, Encoding.UTF8))
            {
                appender.Trace(log);
            }
        }

        #endregion

        public class Constructor
        {
            #region filePath

            [Fact]
            public void filePath_null_ArgumentNullExceptionを投げる()
            {
                Assert.Throws<ArgumentNullException>(() => new FileAppender(filePath: null, append: true, Encoding.UTF8));
            }

            [Fact]
            public void filePath_正しいファイルパス_ログファイルを開く()
            {
                var logFilePath = TestFile.GetFilePath(".log");

                using (var appender = new FileAppender(logFilePath, append: false, Encoding.UTF8))
                {
                    appender.Trace("A");
                }

                Assert.Equal("A", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void filePath_不正なファイルパス_ArgumentExceptionを投げる()
            {
                var ex = Record.Exception(() => new FileAppender(filePath: "?", append: true, Encoding.UTF8));

                Assert.IsType<ArgumentException>(ex);
            }

            #endregion

            #region append

            [Fact]
            public void append_false_ログを上書きする()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender(logFilePath, append: false, Encoding.UTF8))
                {
                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void append_true_ログを追加する()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender(logFilePath, append: true, Encoding.UTF8))
                {
                    appender.Trace("B");
                }

                Assert.Equal("AB", File.ReadAllText(logFilePath));
            }

            #endregion

            #region encoding

            [Fact]
            public void encoding_null_ArgumentNullExceptionを投げる()
            {
                var logFilePath = TestFile.GetFilePath(".log");

                var ex = Record.Exception(() => new FileAppender(logFilePath, append: true, encoding: null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            #endregion
        }

        public class Open
        {
            #region filePath

            [Fact]
            public void filePath_null_ArgumentNullExceptionを投げる()
            {
                Assert.Throws<ArgumentNullException>(() => new FileAppender().Open(filePath: null, append: true, Encoding.UTF8));
            }

            [Fact]
            public void filePath_正しいファイルパス_ログファイルを開く()
            {
                var logFilePath = TestFile.GetFilePath(".log");

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePath, append: false, Encoding.UTF8);
                    appender.Trace("A");
                }

                Assert.Equal("A", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void filePath_不正なファイルパス_ArgumentExceptionを投げる()
            {
                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => appender.Open(filePath: "?", append: true, Encoding.UTF8));

                    Assert.IsType<ArgumentException>(ex);
                }
            }

            #endregion

            #region append

            [Fact]
            public void append_false_ログを上書きする()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender())
                {
                    appender.ConversionPattern = "{message}";
                    appender.Open(logFilePath, append: false, Encoding.UTF8);
                    appender.Trace("B");
                }

                Assert.Equal("B", File.ReadAllText(logFilePath));
            }

            [Fact]
            public void append_true_ログを追加する()
            {
                var logFilePath = TestFile.GetFilePath(".log");
                CreateLogFile(logFilePath, "A");

                using (var appender = new FileAppender())
                {
                    appender.ConversionPattern = "{message}";
                    appender.Open(logFilePath, append: true, Encoding.UTF8);
                    appender.Trace("B");
                }

                var actual = File.ReadAllText(logFilePath);
                Assert.Equal("AB", actual);
            }

            #endregion

            #region encoding

            [Fact]
            public void encoding_null_ArgumentNullExceptionを投げる()
            {
                var logFilePath = TestFile.GetFilePath(".log");

                using (var appender = new FileAppender())
                {
                    var ex = Record.Exception(() => new FileAppender(logFilePath, append: true, encoding: null));

                    Assert.IsType<ArgumentNullException>(ex);
                }
            }

            #endregion

            [Fact]
            public void 連続してOpenする_CloseしたあとにOpenする()
            {
                var logFilePathA = TestFile.GetFilePath(".log");
                var logFilePathB = TestFile.GetFilePath(".log");

                using (var appender = new FileAppender())
                {
                    appender.Open(logFilePathA, append: false, Encoding.UTF8);
                    appender.Trace("A");

                    appender.Open(logFilePathB, append: false, Encoding.UTF8);
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
                using (var appender = new FileAppender())
                {
                    appender.Open(TestFile.GetFilePath(".log"), append: false, Encoding.UTF8);

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
                using (var appender = new FileAppender())
                {
                    appender.Open(TestFile.GetFilePath(".log"), append: false, Encoding.UTF8);
                    appender.Close();
                    var ex = Record.Exception(() => appender.Close());

                    Assert.Null(ex);
                }
            }
        }
    }
}
