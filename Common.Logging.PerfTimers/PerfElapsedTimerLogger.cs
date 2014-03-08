using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Logging.PerfTimers
{
    public class PerfElapsedTimerLogger : PerfTimerLogger
    {
        private static IList<Tuple<LogLevel, TimeSpan>> _defaultMap = new[]
        {
            Tuple.Create(LogLevel.Debug, TimeSpan.Zero),
            Tuple.Create(LogLevel.Info, TimeSpan.FromMilliseconds(100)),
            Tuple.Create(LogLevel.Warn, TimeSpan.FromSeconds(1)),
            Tuple.Create(LogLevel.Error, TimeSpan.FromSeconds(10))
        };

        public static void SetDefaultMap(IList<Tuple<LogLevel, TimeSpan>> defaultMap)
        {
            _defaultMap = defaultMap
                .OrderBy(p => p.Item2.Ticks)
                .ToArray();
        }

        public PerfElapsedTimerLogger(
            ILog logger,
            IList<Tuple<LogLevel, TimeSpan>> map,
            string format,
            params object[] args)
            : base(logger, map ?? _defaultMap, format, args)
        {
        }

        protected override bool IsPastThreshold(TimeSpan timeSpan)
        {
            return Stopwatch.Elapsed >= timeSpan;
        }
    }
}