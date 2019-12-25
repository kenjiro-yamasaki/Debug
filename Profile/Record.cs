using System;
using System.Diagnostics;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイル記録。
    /// </summary>
    public class Record
    {
        #region プロパティ

        /// <summary>
        /// プロファイル名。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 計測回数。
        /// </summary>
        public int Count { get; private set; }

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
        public double TotalSeconds => TotalTicks / Stopwatch.Frequency;

        /// <summary>
        /// 最小計測時間 (秒)。
        /// </summary>
        public double MinSeconds => MinTicks / Stopwatch.Frequency;

        /// <summary>
        /// 最大計測時間 (秒)。
        /// </summary>
        public double MaxSeconds => MaxTicks / Stopwatch.Frequency;

        /// <summary>
        /// 最小計測インデックス (0～)。
        /// </summary>
        public int MinIndex { get; private set; }

        /// <summary>
        /// 最大計測インデックス (1～)。
        /// </summary>
        public int MaxIndex { get; private set; }

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
        public void OutputLog()
        {
            //Assert.True(1 <= Count);

            //var totalTime   = $"{TotalTicks / (double)Stopwatch.Frequency:0.0000000000}";
            //var averageTime = $"{TotalTicks / Count / (double)Stopwatch.Frequency:0.0000000000}";
            //var maxTime     = $"{MaxTicks / (double)Stopwatch.Frequency:0.0000000000}";
            //var minTime     = $"{MinTicks / (double)Stopwatch.Frequency:0.0000000000}";
            //var maxSequence = $"{MaxIndex}";
            //var minSequence = $"{MinIndex}";
            //var count       = $"{Count}";

            //Logger.Trace($"{Name} **************************");
            //Logger.Trace($"合計プロファイル時間={totalTime}s");
            //Logger.Trace($"平均プロファイル時間={averageTime}s");
            //Logger.Trace($"最大プロファイル時間={maxTime}s");
            //Logger.Trace($"最小プロファイル時間={minTime}s");
            //Logger.Trace($"最大プロファイル順序={maxSequence}");
            //Logger.Trace($"最小プロファイル順序={minSequence}");
            //Logger.Trace($"プロファイル回数={count}");
        }

        #endregion
    }
}
