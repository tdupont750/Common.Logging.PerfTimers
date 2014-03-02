using System;
using System.Collections.Generic;
using Common.Logging.PerfTimers;

// ReSharper disable CheckNamespace
namespace Common.Logging
// ReSharper restore CheckNamespace
{
    public static class LogExtensions
    {
        public static PerfTimerLogger PerfDeviationTimer(
            this ILog logger,
            string key,
            string format,
            params object[] args)
        {
            return new PerfDeviationTimerLogger(logger, null, key, format, args);
        }

        public static PerfTimerLogger PerfDeviationTimer(
            this ILog logger,
            IList<Tuple<LogLevel, TimeSpan>> map,
            string key,
            string format,
            params object[] args)
        {
            return new PerfDeviationTimerLogger(logger, map, key, format, args);
        }

        public static PerfTimerLogger PerfEllapsedTimer(
            this ILog logger,
            string format,
            params object[] args)
        {
            return new PerfEllapsedTimerLogger(logger, null, format, args);
        }

        public static PerfTimerLogger PerfEllapsedTimer(
            this ILog logger,
            IList<Tuple<LogLevel, TimeSpan>> map,
            string format,
            params object[] args)
        {
            return new PerfEllapsedTimerLogger(logger, map, format, args);
        }
    }
}