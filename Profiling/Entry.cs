using SoftCube.Asserts;
using System;
using System.Diagnostics;

namespace SoftCube.Profiling
{
    /// <summary>
    /// エントリー。
    /// </summary>
    /// <remarks>
    /// エントリーは、計測結果を管理します。
    /// Start を呼びだすと計測を開始します。
    /// Stop を呼びだすと計測を終了し、計測結果をプロパティに反映します。
    /// Start と Stop は、<see cref="Transaction"/> が呼びだします。
    /// </remarks>
    internal class Entry
    {
        #region プロパティ

        /// <summary>
        /// プロファイル名。
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// 計測回数。
        /// </summary>
        internal int Count { get; private set; }

        /// <summary>
        /// 合計計測時間 (秒)。
        /// </summary>
        internal double TotalSeconds => totalTicks / (double)Stopwatch.Frequency;

        /// <summary>
        /// 平均計測時間 (秒)。
        /// </summary>
        internal double AverageSeconds => TotalSeconds / Count;

        /// <summary>
        /// 最小計測時間 (秒)。
        /// </summary>
        internal double MinSeconds => minTicks / (double)Stopwatch.Frequency;

        /// <summary>
        /// 最大計測時間 (秒)。
        /// </summary>
        internal double MaxSeconds => maxTicks / (double)Stopwatch.Frequency;

        /// <summary>
        /// 最小計測インデックス (0～)。
        /// </summary>
        internal int MinIndex { get; private set; }

        /// <summary>
        /// 最大計測インデックス (0～)。
        /// </summary>
        internal int MaxIndex { get; private set; }

        /// <summary>
        /// 合計計測チック (タイマー刻み)。
        /// </summary>
        private long totalTicks;

        /// <summary>
        /// 最小計測チック (タイマー刻み)。
        /// </summary>
        private long minTicks;

        /// <summary>
        /// 最大計測チック (タイマー刻み)。
        /// </summary>
        private long maxTicks;

        /// <summary>
        /// 何回 Stop をスキップするか。
        /// </summary>
        /// <remarks>
        /// 以下のコードのような場合、transaction2 と transaction3 の Start と Stop 処理は無視する必要があります。
        /// そこで Start 時に Stopwatch.IsRunning が true の場合、 ignoreStopCount をカウントアップし、計測開始処理をスキップします。
        /// Stop 時に ignoreStopCount が 1 以上の場合、ignoreStopCount をカウントダウンし、計測終了処理をスキップします。
        /// <code>
        /// using (var transaction1 = Profiler.Start("A"))
        /// {
        ///     Thread.Sleep(500);
        ///     using (var transaction2 = Profiler.Start("A"))
        ///     {
        ///         Thread.Sleep(500);
        ///         using (var transaction3 = Profiler.Start("A"))
        ///         {
        ///             Thread.Sleep(500);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </remarks>
        private int skipStopCount = 0;

        /// <summary>
        /// ストップウォッチ。
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="name">プロファイル名。</param>
        public Entry(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 計測を開始します。
        /// </summary>
        public void Start()
        {
            if (stopwatch.IsRunning)
            {
                skipStopCount++;
                return;
            }

            stopwatch.Restart();
        }

        /// <summary>
        /// 計測を終了します。
        /// </summary>
        public void Stop()
        {
            if (1 <= skipStopCount)
            {
                skipStopCount--;
                return;
            }

            Assert.True(stopwatch.IsRunning);
            stopwatch.Stop();

            var elapsedTicks = stopwatch.ElapsedTicks;
            totalTicks += elapsedTicks;

            if (Count == 0)
            {
                minTicks = elapsedTicks;
                MinIndex = Count;

                maxTicks = elapsedTicks;
                MaxIndex = Count;
            }
            else
            {
                if (elapsedTicks < minTicks)
                {
                    minTicks = elapsedTicks;
                    MinIndex = Count;
                }
                if (maxTicks < elapsedTicks)
                {
                    maxTicks = elapsedTicks;
                    MaxIndex = Count;
                }
            }

            Count++;
        }

        #endregion
    }
}
