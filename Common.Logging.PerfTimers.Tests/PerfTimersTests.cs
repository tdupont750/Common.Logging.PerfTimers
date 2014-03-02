using System;
using System.Threading;
using NSubstitute;
using Xunit;

namespace Common.Logging.PerfTimers.Tests
{
    public class PerfDeviationTimer
    {
        [Fact]
        public void BasicUseCase()
        {
            var logger = Substitute.For<ILog>();
            logger.IsInfoEnabled.Returns(true);

            // The first call will never cause a log line.
            // NOTE: It will NOT cause a log line.
            using (logger.PerfDeviationTimer("A1", "1"))
                Thread.Sleep(250);

            // This has the same duration as the avareage 
            // of the previous timers.
            // NOTE: It will NOT cause a log line.
            using (logger.PerfDeviationTimer("A1", "2"))
                Thread.Sleep(250);

            // The average of the previous timers and this
            // duration deviate by more than 100 milliseconds.
            // NOTE: It will cause a log line.
            using (logger.PerfDeviationTimer("A1", "3"))
                Thread.Sleep(100);

            // Again, the average of the previous timers and this
            // duration deviate by more than 100 milliseconds.
            // NOTE: It will cause a log line.
            using (logger.PerfDeviationTimer("A1", "4"))
                Thread.Sleep(50);

            // This time the average of the previous timers
            // has shifted, and the duration no longer deviates
            // by more than 100 milliseconds.
            // NOTE: It will NOT cause a log line.
            using (logger.PerfDeviationTimer("B1", "5"))
                Thread.Sleep(50);

            logger.Received(2).Info(Arg.Any<string>());
            logger.Received(1).Info("3");
            logger.Received(1).Info("4");
        }

        [Fact]
        public void CicularBuffer()
        {
            var map = new[]
            {
                Tuple.Create(LogLevel.Info, TimeSpan.FromMilliseconds(10))
            };

            var logger = Substitute.For<ILog>();
            logger.IsInfoEnabled.Returns(true);

            for (var i = 0; i < 10; i++)
                using (logger.PerfDeviationTimer(map, "A2", i.ToString()))
                    Thread.Sleep(30);

            using (logger.PerfDeviationTimer(map, "A2", "11"))
                Thread.Sleep(15);

            for (var i = 12; i < 21; i++)
                using (logger.PerfDeviationTimer(map, "A2", i.ToString()))
                    Thread.Sleep(20);

            using (logger.PerfDeviationTimer(map, "A2", "22"))
                Thread.Sleep(35);

            logger.Received(2).Info(Arg.Any<string>());
            logger.Received(1).Info("11");
            logger.Received(1).Info("22");
        }
    }
}
