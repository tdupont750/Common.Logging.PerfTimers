using System.Threading;
using NSubstitute;
using Xunit;

namespace Common.Logging.PerfTimers.Tests
{
    public class PerfElapsedTimerTests
    {
        [Fact]
        public void BasicUseCase()
        {
            var logger = Substitute.For<ILog>();
            logger.IsInfoEnabled.Returns(true);
            logger.IsWarnEnabled.Returns(true);

            // This matches the Debug timer limit,
            // but debug is not enabled on the logger.
            // NOTE: It will NOT cause a log line.
            using (logger.PerfElapsedTimer("1"))
                Thread.Sleep(5);

            // This will match the Warn timer limit.
            // NOTE: It will cause a log line.
            using (logger.PerfElapsedTimer("2"))
                Thread.Sleep(105);

            // This will match the Info timer limit.
            // NOTE: It will cause a log line.
            using (logger.PerfElapsedTimer("{0}", 3))
                Thread.Sleep(1005);

            logger.Received(0).Debug("1");
            logger.Received(1).Info("2");
            logger.Received(1).WarnFormat("{0}", 3);
        }
    }
}