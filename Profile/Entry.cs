using System;
using System.Diagnostics;

namespace SoftCube.Profile
{
    /// <summary>
    /// エントリー。
    /// </summary>
    /// <remarks>
    /// プロファイルのエントリーを表現します。
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
        internal double TotalSeconds => TotalTicks / (double)Stopwatch.Frequency;

        /// <summary>
        /// 平均計測時間 (秒)。
        /// </summary>
        internal double AverageSeconds => TotalSeconds / Count;

        /// <summary>
        /// 最小計測時間 (秒)。
        /// </summary>
        internal double MinSeconds => MinTicks / (double)Stopwatch.Frequency;

        /// <summary>
        /// 最大計測時間 (秒)。
        /// </summary>
        internal double MaxSeconds => MaxTicks / (double)Stopwatch.Frequency;

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
        private long TotalTicks { get; set; }

        /// <summary>
        /// 最小計測チック (タイマー刻み)。
        /// </summary>
        private long MinTicks { get; set; }

        /// <summary>
        /// 最大計測チック (タイマー刻み)。
        /// </summary>
        private long MaxTicks { get; set; }

        /// <summary>
        /// ストップウォッチ。
        /// </summary>
        private Stopwatch Stopwatch { get; } = new Stopwatch();

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
            Stopwatch.Restart();
        }

        /// <summary>
        /// 計測を終了します。
        /// </summary>
        public void Stop()
        {
            Stopwatch.Stop();

            var elapsedTicks = Stopwatch.ElapsedTicks;
            TotalTicks += elapsedTicks;

            if (Count == 0)
            {
                MinTicks = elapsedTicks;
                MinIndex = Count;

                MaxTicks = elapsedTicks;
                MaxIndex = Count;
            }
            else
            {
                if (elapsedTicks < MinTicks)
                {
                    MinTicks = elapsedTicks;
                    MinIndex = Count;
                }
                if (MaxTicks < elapsedTicks)
                {
                    MaxTicks = elapsedTicks;
                    MaxIndex = Count;
                }
            }

            Count++;
        }

        #endregion
    }
}
