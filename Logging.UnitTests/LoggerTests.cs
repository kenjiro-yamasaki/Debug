using Xunit;

namespace SoftCube.Logging
{
    public class LoggerTests
    {
        public class Regression
        {
            [Fact]
            public void 構成ファイルのないLogger_許容する()
            {
                Logger.Add(new StringAppender());
            }
        }
    }
}
