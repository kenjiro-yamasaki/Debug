using Xunit;

namespace SoftCube.Logger.UnitTests
{
    public class StringAppenderTests
    {
        [Fact]
        public void ToString_A_正しく変換する()
        {
            var appender = new StringAppender();
            appender.ConversionPattern = "{message}";
            appender.Trace("A");

            var actual = appender.ToString();

            Assert.Equal("A", actual);
        }

        [Fact]
        public void ToString_AB_正しく変換する()
        {
            var appender = new StringAppender();
            appender.ConversionPattern = "{message}";
            appender.Trace("A");
            appender.Trace("B");

            var actual = appender.ToString();

            Assert.Equal("AB", actual);
        }
    }
}
