using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common.Logging.PerfTimers
{
    public class PerfDeviationTimerLogger : PerfTimerLogger
    {
        private static readonly ConcurrentDictionary<string, DeviationContext> Contexts =
            new ConcurrentDictionary<string, DeviationContext>();

        private static IList<Tuple<LogLevel, TimeSpan>> _defaultMap = new[]
        {
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

        private readonly string _key;

        public PerfDeviationTimerLogger(
            ILog logger,
            IList<Tuple<LogLevel, TimeSpan>> map,
            string key,
            string format,
            params object[] args)
            : base(logger, map ?? _defaultMap, format, args)
        {
            _key = key;
        }

        protected override bool IsPastThreshold(TimeSpan timeSpan)
        {
            var context = Contexts.GetOrAdd(_key, k => new DeviationContext());

            long average;
            lock (context)
            {
                average = context.GetAverage();
                context.Add(Stopwatch.ElapsedMilliseconds);
            }

            if (average == 0)
                return false;

            var diff = Math.Abs(average - Stopwatch.ElapsedMilliseconds);
            return timeSpan.TotalMilliseconds <= diff;
        }

        private class DeviationContext
        {
            private int _nextIndex;
            private int _usedCount;
            private readonly long[] _entries = new long[10];

            public DeviationContext()
            {
                _nextIndex = 0;
            }

            public long GetAverage()
            {
                long sum = 0;

                for (var i = 0; i < _usedCount; i++)
                    sum += _entries[i];

                return _usedCount == 0
                           ? 0
                           : sum / _usedCount;
            }

            public void Add(long entry)
            {
                _entries[_nextIndex] = entry;

                _nextIndex++;
                if (_nextIndex == _entries.Length)
                    _nextIndex = 0;

                if (_usedCount != _entries.Length)
                    _usedCount++;
            }
        }
    }
}