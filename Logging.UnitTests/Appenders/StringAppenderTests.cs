using Xunit;

namespace SoftCube.Logging
{
    public class StringAppenderTests
    {
        [Fact]
        public void ToString_A_正しく変換する()
        {
            var appender = new StringAppender();
            appender.Trace("A");

            var actual = appender.ToString();

            Assert.Equal("A", actual);
        }

        [Fact]
        public void ToString_AB_正しく変換する()
        {
            var appender = new StringAppender();
            appender.Trace("A");
            appender.Trace("B");

            var actual = appender.ToString();

            Assert.Equal("AB", actual);
        }
    }
}
