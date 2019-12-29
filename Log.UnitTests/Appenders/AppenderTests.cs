using NSubstitute;
using SoftCube.Runtime;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace SoftCube.Log.Appenders.UnitTests
{
    public class AppenderTests
    {
        public class MinLevel
        {
            [Fact]
            public void Trace_Trace以上を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Trace;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Debug_Debug以上を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Debug;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.DidNotReceive().Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Info_Info以上を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Info;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.DidNotReceive().Log(nameof(Level.Trace));
                appender.DidNotReceive().Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Warning_Warning以上を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Warning;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.DidNotReceive().Log(nameof(Level.Trace));
                appender.DidNotReceive().Log(nameof(Level.Debug));
                appender.DidNotReceive().Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Error_Error以上を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Error;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.DidNotReceive().Log(nameof(Level.Trace));
                appender.DidNotReceive().Log(nameof(Level.Debug));
                appender.DidNotReceive().Log(nameof(Level.Info));
                appender.DidNotReceive().Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Fatal_Fatal以上を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Fatal;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.DidNotReceive().Log(nameof(Level.Trace));
                appender.DidNotReceive().Log(nameof(Level.Debug));
                appender.DidNotReceive().Log(nameof(Level.Info));
                appender.DidNotReceive().Log(nameof(Level.Warning));
                appender.DidNotReceive().Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }
        }

        public class MaxLevel
        {
            [Fact]
            public void Fatal_Fatal以下を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Fatal;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.Received(1).Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Error_Error以下を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Error;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.Received(1).Log(nameof(Level.Error));
                appender.DidNotReceive().Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Warning_Warning以下を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Warning;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.Received(1).Log(nameof(Level.Warning));
                appender.DidNotReceive().Log(nameof(Level.Error));
                appender.DidNotReceive().Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Info_Info以下を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Info;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.Received(1).Log(nameof(Level.Info));
                appender.DidNotReceive().Log(nameof(Level.Warning));
                appender.DidNotReceive().Log(nameof(Level.Error));
                appender.DidNotReceive().Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Debug_Debug以下を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Debug;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.Received(1).Log(nameof(Level.Debug));
                appender.DidNotReceive().Log(nameof(Level.Info));
                appender.DidNotReceive().Log(nameof(Level.Warning));
                appender.DidNotReceive().Log(nameof(Level.Error));
                appender.DidNotReceive().Log(nameof(Level.Fatal));
            }

            [Fact]
            public void Trace_Trace以下を出力する()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Trace;

                appender.Trace(nameof(Level.Trace));
                appender.Debug(nameof(Level.Debug));
                appender.Info(nameof(Level.Info));
                appender.Warning(nameof(Level.Warning));
                appender.Error(nameof(Level.Error));
                appender.Fatal(nameof(Level.Fatal));

                appender.Received(1).Log(nameof(Level.Trace));
                appender.DidNotReceive().Log(nameof(Level.Debug));
                appender.DidNotReceive().Log(nameof(Level.Info));
                appender.DidNotReceive().Log(nameof(Level.Warning));
                appender.DidNotReceive().Log(nameof(Level.Error));
                appender.DidNotReceive().Log(nameof(Level.Fatal));
            }
        }

        public class LogFormat
        {
            [Fact]
            public void Date_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{DateTime}";

                appender.Trace("A");

                var actual = appender.ToString();
                Assert.True(global::System.DateTime.TryParse(actual, out _));
            }

            [Fact]
            public void Date書式指定_正しく出力する()
            {
                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2019, 12, 19, 22, 54, 19, 777));

                var appender = new StringAppender(clock);
                appender.LogFormat = "{DateTime:yyyy-MM-dd HH:mm:ss,fff}";

                appender.Trace("A");

                var expected = clock.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void File_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{File}";

                appender.Trace("A");

                var expected = new StackFrame(true).GetFileName();
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Level_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Level}";

                appender.Info("A");

                var expected = "INFO";
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Level左詰め_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Level,-5}";

                appender.Info("A");

                var expected = "INFO ";
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Level右詰め_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Level,5}";

                appender.Info("A");

                var log = appender.ToString();
                Assert.Equal(" INFO", log);
            }

            [Fact]
            public void Line_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Line}";

                appender.Trace("A");

                var expected = (new StackFrame(true).GetFileLineNumber() - 2).ToString();
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Message_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Message}";

                appender.Info("A");

                var log = appender.ToString();
                Assert.Equal("A", log);
            }

            [Fact]
            public void Method_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Method}";

                appender.Trace("A");

                var expected = nameof(Method_正しく出力する);
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void NewLine_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{NewLine}";

                appender.Info("A");

                var log = appender.ToString();
                Assert.Equal($"{Environment.NewLine}", log);
            }

            [Fact]
            public void Thread_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Thread}";

                appender.Trace("A");

                var expected = Thread.CurrentThread.ManagedThreadId.ToString();
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Type_正しく出力する()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Type}";

                appender.Trace("A");

                var expected = new StackFrame(true).GetMethod().DeclaringType.FullName;
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void 推奨書式_正しく出力する()
            {
                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2019, 12, 19, 22, 54, 19, 777));

                var appender = new StringAppender(clock);
                appender.LogFormat = "{DateTime:yyyy-MM-dd HH:mm:ss,fff} [{Level,-5}] - {Message}{NewLine}";

                appender.Info("A");

                var expected = $@"{clock.Now:yyyy-MM-dd HH:mm:ss,fff} [INFO ] - A{Environment.NewLine}";
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void 不正な書式_を投げる()
            {
                var appender = new StringAppender();
                appender.LogFormat = "{Date}";

                var ex = Record.Exception(() => appender.Trace("A"));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("LogFormat[{Date}]が不正です。", ex.Message);
            }
        }

        public class Trace
        {
            [Fact]
            public void null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Trace(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 空白_許容する()
            {
                var appender = Substitute.For<Appender>();

                appender.Trace("");

                appender.Received().Log(Level.Trace, "");
            }

            [Fact]
            public void A_成功する()
            {
                var appender = Substitute.For<Appender>();

                appender.Trace("A");

                appender.Received().Log(Level.Trace, "A");
            }
        }

        public class Debug
        {
            [Fact]
            public void null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Debug(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 空白_許容する()
            {
                var appender = Substitute.For<Appender>();

                appender.Debug("");

                appender.Received().Log(Level.Debug, "");
            }

            [Fact]
            public void A_成功する()
            {
                var appender = Substitute.For<Appender>();

                appender.Debug("A");

                appender.Received().Log(Level.Debug, "A");
            }
        }

        public class Info
        {
            [Fact]
            public void null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Info(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 空白_許容する()
            {
                var appender = Substitute.For<Appender>();

                appender.Info("");

                appender.Received().Log(Level.Info, "");
            }

            [Fact]
            public void A_成功する()
            {
                var appender = Substitute.For<Appender>();

                appender.Info("A");

                appender.Received().Log(Level.Info, "A");
            }
        }

        public class Warning
        {
            [Fact]
            public void null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Warning(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 空白_許容する()
            {
                var appender = Substitute.For<Appender>();

                appender.Warning("");

                appender.Received().Log(Level.Warning, "");
            }

            [Fact]
            public void A_成功する()
            {
                var appender = Substitute.For<Appender>();

                appender.Warning("A");

                appender.Received().Log(Level.Warning, "A");
            }
        }

        public class Error
        {
            [Fact]
            public void null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Error(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 空白_許容する()
            {
                var appender = Substitute.For<Appender>();

                appender.Error("");

                appender.Received().Log(Level.Error, "");
            }

            [Fact]
            public void A_成功する()
            {
                var appender = Substitute.For<Appender>();

                appender.Error("A");

                appender.Received().Log(Level.Error, "A");
            }
        }

        public class Fatal
        {
            [Fact]
            public void null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Fatal(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void 空白_許容する()
            {
                var appender = Substitute.For<Appender>();

                appender.Fatal("");

                appender.Received().Log(Level.Fatal, "");
            }

            [Fact]
            public void A_成功する()
            {
                var appender = Substitute.For<Appender>();

                appender.Fatal("A");

                appender.Received().Log(Level.Fatal, "A");
            }
        }

        public class Log
        {
            [Fact]
            public void level_Trace_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Trace;
                appender.MaxLevel = Level.Trace;

                appender.Log(Level.Trace, "A");

                SubstituteExtensions.Received<Appender>(appender).Log("A");
            }

            [Fact]
            public void level_Debug_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Debug;
                appender.MaxLevel = Level.Debug;

                appender.Log(Level.Debug, "A");

                SubstituteExtensions.Received<Appender>(appender).Log("A");
            }

            [Fact]
            public void level_Info_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Info;
                appender.MaxLevel = Level.Info;

                appender.Log(Level.Info, "A");

                SubstituteExtensions.Received<Appender>(appender).Log("A");
            }

            [Fact]
            public void level_Warning_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Warning;
                appender.MaxLevel = Level.Warning;

                appender.Log(Level.Warning, "A");

                SubstituteExtensions.Received<Appender>(appender).Log("A");
            }

            [Fact]
            public void level_Error_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Error;
                appender.MaxLevel = Level.Error;

                appender.Log(Level.Error, "A");

                SubstituteExtensions.Received<Appender>(appender).Log("A");
            }

            [Fact]
            public void level_Fatal_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Fatal;
                appender.MaxLevel = Level.Fatal;

                appender.Log(Level.Fatal, "A");

                SubstituteExtensions.Received<Appender>(appender).Log("A");
            }

            [Fact]
            public void message_null_ArgumentNullExceptionが投げられる()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Log(Level.Trace, null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void message_空白_許容する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Trace;
                appender.MaxLevel = Level.Trace;

                appender.Log(Level.Trace, "");

                appender.Received().Log("");
            }

            [Fact]
            public void message_A_成功する()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Trace;
                appender.MaxLevel = Level.Trace;

                appender.Log(Level.Trace, "A");

                appender.Received().Log("A");
            }
        }
    }
}
