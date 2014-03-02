using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common.Logging.PerfTimers
{
    public abstract class PerfTimerLogger : IDisposable
    {
        private readonly ILog _logger;
        private readonly IList<Tuple<LogLevel, TimeSpan>> _map;
        private readonly string _format;
        private readonly object[] _args;

        protected readonly Stopwatch Stopwatch;

        protected PerfTimerLogger(
            ILog logger,
            IList<Tuple<LogLevel, TimeSpan>> map,
            string format,
            params object[] args)
        {
            _logger = logger;
            _map = map;
            _format = format;
            _args = args;
            Stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            if (!Stopwatch.IsRunning)
                return;

            Stopwatch.Stop();

            WriteLog();
        }

        private void WriteLog()
        {
            for (var i = _map.Count - 1; i >= 0; i--)
            {
                var pair = _map[i];
                var isPastThreshold = IsPastThreshold(pair.Item2);

                if (!isPastThreshold)
                    continue;

                var hasArgs = _args != null && _args.Length > 0;

                switch (pair.Item1)
                {
                    case LogLevel.Error:
                        if (!_logger.IsErrorEnabled)
                            continue;

                        if (hasArgs)
                            _logger.ErrorFormat(_format, _args);
                        else
                            _logger.Error(_format);
                        return;

                    case LogLevel.Fatal:
                        if (!_logger.IsFatalEnabled)
                            continue;

                        if (hasArgs)
                            _logger.FatalFormat(_format, _args);
                        else
                            _logger.Fatal(_format);
                        return;

                    case LogLevel.Info:
                        if (!_logger.IsInfoEnabled)
                            continue;

                        if (hasArgs)
                            _logger.InfoFormat(_format, _args);
                        else
                            _logger.Info(_format);
                        return;

                    case LogLevel.Trace:
                        if (!_logger.IsTraceEnabled)
                            continue;

                        if (hasArgs)
                            _logger.TraceFormat(_format, _args);
                        else
                            _logger.Trace(_format);
                        return;

                    case LogLevel.Warn:
                        if (!_logger.IsWarnEnabled)
                            continue;

                        if (hasArgs)
                            _logger.WarnFormat(_format, _args);
                        else
                            _logger.Warn(_format);
                        return;

                    default:
                        if (!_logger.IsDebugEnabled)
                            continue;

                        if (hasArgs)
                            _logger.DebugFormat(_format, _args);
                        else
                            _logger.Debug(_format);
                        return;
                }
            }
        }

        protected abstract bool IsPastThreshold(TimeSpan timeSpan);
    }
}
