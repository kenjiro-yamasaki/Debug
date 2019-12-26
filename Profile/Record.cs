using SoftCube.Log;
using System;
using System.Diagnostics;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイル記録。
    /// </summary>
    internal class Record
    {
        #region プロパティ

        /// <summary>
        /// プロファイル名。
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// 計測回数。
        /// </summary>
        internal int Count { get; set; }

        /// <summary>
        /// 合計計測チック (タイマー刻み)。
        /// </summary>
        private long TotalTicks { get;  set; }

        /// <summary>
        /// 最小計測チック (タイマー刻み)。
        /// </summary>
        private long MinTicks { get; set; }

        /// <summary>
        /// 最大計測チック (タイマー刻み)。
        /// </summary>
        private long MaxTicks { get; set; }

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
        internal int MinIndex { get; set; }

        /// <summary>
        /// 最大計測インデックス (0～)。
        /// </summary>
        internal int MaxIndex { get; set; }

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
        public Record(string name)
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

            long elapsedTicks = Stopwatch.ElapsedTicks;
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

        /// <summary>
        /// プロファイル結果をログ出力する。
        /// </summary>
        internal void Log()
        {
            Logger.Trace($"{Name} **************************{Environment.NewLine}");
            Logger.Trace($"合計プロファイル時間={TotalSeconds:F10}s{Environment.NewLine}");
            Logger.Trace($"平均プロファイル時間={AverageSeconds:F10}s{Environment.NewLine}");
            Logger.Trace($"最大プロファイル時間={MaxSeconds:F10}s{Environment.NewLine}");
            Logger.Trace($"最小プロファイル時間={MinSeconds:F10}s{Environment.NewLine}");
            Logger.Trace($"最大プロファイル順序={MaxIndex + 1}{Environment.NewLine}");
            Logger.Trace($"最小プロファイル順序={MinIndex + 1}{Environment.NewLine}");
            Logger.Trace($"プロファイル回数={Count}{Environment.NewLine}");
        }

        #endregion
    }
}
