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
            public void Trace_Trace�ȏ���o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Trace;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Debug_Debug�ȏ���o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Debug;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Info_Info�ȏ���o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Info;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Warning_Warning�ȏ���o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Warning;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Error_Error�ȏ���o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Error;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Fatal_Fatal�ȏ���o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MinLevel = Level.Fatal;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }
        }

        public class MaxLevel
        {
            [Fact]
            public void Fatal_Fatal�ȉ����o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Fatal;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Error_Error�ȉ����o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Error;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Warning_Warning�ȉ����o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Warning;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Info_Info�ȉ����o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Info;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Debug_Debug�ȉ����o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Debug;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Fatal, Arg.Any<string>());
            }

            [Fact]
            public void Trace_Trace�ȉ����o�͂���()
            {
                var appender = Substitute.For<Appender>();
                appender.MaxLevel = Level.Trace;

                appender.Trace("");
                appender.Debug("");
                appender.Info("");
                appender.Warning("");
                appender.Error("");
                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender, (int)1).Log(Level.Trace, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Debug, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Info, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Warning, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Error, Arg.Any<string>());
                SubstituteExtensions.DidNotReceive<Appender>(appender).Log(Level.Fatal, Arg.Any<string>());
            }
        }

        public class ConversionPattern
        {
            [Fact]
            public void Date_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{date}";

                appender.Trace("A");

                var actual = appender.ToString();
                Assert.True(global::System.DateTime.TryParse(actual, out _));
            }

            [Fact]
            public void Date�����w��_�������o�͂���()
            {
                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2019, 12, 19, 22, 54, 19, 777));

                var appender = new StringAppender(clock);
                appender.ConversionPattern = "{date:yyyy-MM-dd HH:mm:ss,fff}";

                appender.Trace("A");

                var expected = clock.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void File_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{file}";

                appender.Trace("A");

                var expected = new StackFrame(true).GetFileName();
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Level_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{level}";

                appender.Info("A");

                var expected = "INFO";
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Level���l��_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{level,-5}";

                appender.Info("A");

                var expected = "INFO ";
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Level�E�l��_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{level,5}";

                appender.Info("A");

                var log = appender.ToString();
                Assert.Equal(" INFO", log);
            }

            [Fact]
            public void Line_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{line}";

                appender.Trace("A");

                var expected = (new StackFrame(true).GetFileLineNumber() - 2).ToString();
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Message_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{message}";

                appender.Info("A");

                var log = appender.ToString();
                Assert.Equal("A", log);
            }

            [Fact]
            public void Method_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{method}";

                appender.Trace("A");

                var expected = nameof(Method_�������o�͂���);
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void NewLine_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{newline}";

                appender.Info("A");

                var log = appender.ToString();
                Assert.Equal($"{Environment.NewLine}", log);
            }

            [Fact]
            public void Thread_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{thread}";

                appender.Trace("A");

                var expected = Thread.CurrentThread.ManagedThreadId.ToString();
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Type_�������o�͂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{type}";

                appender.Trace("A");

                var expected = new StackFrame(true).GetMethod().DeclaringType.FullName;
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void ��������_�������o�͂���()
            {
                var clock = Substitute.For<ISystemClock>();
                clock.Now.Returns(new DateTime(2019, 12, 19, 22, 54, 19, 777));

                var appender = new StringAppender(clock);
                appender.ConversionPattern = "{date:yyyy-MM-dd HH:mm:ss,fff} [{level,-5}] - {message}{newline}";

                appender.Info("A");

                var expected = $@"{clock.Now:yyyy-MM-dd HH:mm:ss,fff} [INFO ] - A{Environment.NewLine}";
                var actual   = appender.ToString();
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void �s���ȏ���_�𓊂���()
            {
                var appender = new StringAppender();
                appender.ConversionPattern = "{datetime}";

                var ex = Record.Exception(() => appender.Trace("A"));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("ConversionPattern[{datetime}]���s���ł��B", ex.Message);
            }
        }

        public class Trace
        {
            [Fact]
            public void null_ArgumentNullException����������()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Trace(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void ��_���e����()
            {
                var appender = Substitute.For<Appender>();

                appender.Trace("");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Trace, "");
            }

            [Fact]
            public void A_��������()
            {
                var appender = Substitute.For<Appender>();

                appender.Trace("A");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Trace, "A");
            }
        }

        public class Debug
        {
            [Fact]
            public void null_ArgumentNullException����������()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Debug(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void ��_���e����()
            {
                var appender = Substitute.For<Appender>();

                appender.Debug("");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Debug, "");
            }

            [Fact]
            public void A_��������()
            {
                var appender = Substitute.For<Appender>();

                appender.Debug("A");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Debug, "A");
            }
        }

        public class Info
        {
            [Fact]
            public void null_ArgumentNullException����������()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Info(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void ��_���e����()
            {
                var appender = Substitute.For<Appender>();

                appender.Info("");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Info, "");
            }

            [Fact]
            public void A_��������()
            {
                var appender = Substitute.For<Appender>();

                appender.Info("A");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Info, "A");
            }
        }

        public class Warning
        {
            [Fact]
            public void null_ArgumentNullException����������()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Warning(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void ��_���e����()
            {
                var appender = Substitute.For<Appender>();

                appender.Warning("");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Warning, "");
            }

            [Fact]
            public void A_��������()
            {
                var appender = Substitute.For<Appender>();

                appender.Warning("A");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Warning, "A");
            }
        }

        public class Error
        {
            [Fact]
            public void null_ArgumentNullException����������()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Error(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void ��_���e����()
            {
                var appender = Substitute.For<Appender>();

                appender.Error("");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Error, "");
            }

            [Fact]
            public void A_��������()
            {
                var appender = Substitute.For<Appender>();

                appender.Error("A");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Error, "A");
            }
        }

        public class Fatal
        {
            [Fact]
            public void null_ArgumentNullException����������()
            {
                var appender = Substitute.For<Appender>();

                var ex = Record.Exception(() => appender.Fatal(null));

                Assert.IsType<ArgumentNullException>(ex);
            }

            [Fact]
            public void ��_���e����()
            {
                var appender = Substitute.For<Appender>();

                appender.Fatal("");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Fatal, "");
            }

            [Fact]
            public void A_��������()
            {
                var appender = Substitute.For<Appender>();

                appender.Fatal("A");

                SubstituteExtensions.Received<Appender>(appender).Log(Level.Fatal, "A");
            }
        }
    }
}
