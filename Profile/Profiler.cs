using SoftCube.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイラー。
    /// </summary>
    public static class Profiler
    {
        #region 静的プロパティ

        /// <summary>
        /// エントリーコレクション。
        /// </summary>
        private static IEnumerable<Entry> Entries => NameToEntry.Values;

        /// <summary>
        /// エントリー名→エントリー変換。
        /// </summary>
        private static Dictionary<string, Entry> NameToEntry { get; } = new Dictionary<string, Entry>();

        #endregion

        #region 静的コンストラクター

        /// <summary>
        /// 静的コンストラクター。
        /// </summary>
        static Profiler()
        {
            Logger.Exiting += (s, e) => Log();
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 計測を開始します。
        /// </summary>
        /// <param name="name">プロファイル名。</param>
        /// <returns>計測トランザクション。</returns>
        /// <remarks>
        /// 計測を開始して、計測トランザクションを返します。
        /// 計測トランザクションは <see cref="IDisposable"/> を実装しています。
        /// 計測トランザクションの <see cref="IDisposable.Dispose()"/> を呼び出すと計測が終了します。
        /// </remarks>
        /// <example>
        /// このメソッドは以下の例のように使用します。
        /// <code>
        /// using (var transaction = Profiler.Start("A"))
        /// {
        ///     ...計測したい処理...
        /// }
        /// </code>
        /// </example>
        public static Transaction Start(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (NameToEntry.TryGetValue(name, out var entry))
            {
                return new Transaction(entry);
            }
            else
            {
                var transaction = new Transaction(new Entry(name));
                NameToEntry.Add(name, transaction.Entry);
                return transaction;
            }
        }

        /// <summary>
        /// プロファイル結果をログ出力します。
        /// </summary>
        public static void Log()
        {
            Logger.Trace($"----- プロファイル結果 -----{Environment.NewLine}");
            foreach (var entry in Entries.OrderByDescending(p => p.TotalSeconds))
            {
                Logger.Trace($"{entry.Name} **************************");

                Logger.Trace($"合計プロファイル時間={entry.TotalSeconds:F10}s");
                Logger.Trace($"平均プロファイル時間={entry.AverageSeconds:F10}s");
                Logger.Trace($"最大プロファイル時間={entry.MaxSeconds:F10}s");
                Logger.Trace($"最小プロファイル時間={entry.MinSeconds:F10}s");
                Logger.Trace($"最大プロファイル順序={entry.MaxIndex + 1}");
                Logger.Trace($"最小プロファイル順序={entry.MinIndex + 1}");
                Logger.Trace($"プロファイル回数={entry.Count}");
            }
        }

        #endregion
    }
}
