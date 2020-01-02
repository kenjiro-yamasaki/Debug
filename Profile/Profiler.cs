using SoftCube.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイラー。
    /// </summary>
    public static class Profiler
    {
        #region プロパティ

        #region エントリーコレクション

        /// <summary>
        /// エントリーコレクション。
        /// </summary>
        private static IEnumerable<Entry> Entries => NameToEntry.Values;

        /// <summary>
        /// プロファイル名→エントリー変換。
        /// </summary>
        private static Dictionary<string, Entry> NameToEntry { get; } = new Dictionary<string, Entry>();

        #endregion

        #region ログ

        /// <summary>
        /// ログレベル。
        /// </summary>
        internal static Level LogLevel { get; set; }

        /// <summary>
        /// タイトル書式。
        /// </summary>
        internal static string TitleFormat { get; set; }

        /// <summary>
        /// エントリー書式コレクション。
        /// </summary>
        internal static IEnumerable<EntryFormat> EntryFormats => entryFormats;
        private readonly static List<EntryFormat> entryFormats = new List<EntryFormat>();

        #endregion

        #endregion

        #region コンストラクター

        /// <summary>
        /// 静的コンストラクター。
        /// </summary>
        static Profiler()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null)
                {
                    System.Diagnostics.Debug.WriteLine("エントリーアセンブリの取得に失敗しました。");
                    return;
                }

                var configurator = assembly.GetCustomAttribute<Configuration>();
                if (configurator == null)
                {
                    System.Diagnostics.Debug.WriteLine($"エントリーアセンブリ {assembly.FullName} に {typeof(Configuration).FullName } 属性が定義されていません。");
                    return;
                }

                configurator.Configurate();
            }
            finally
            {
                // ロガー終了時 (プロセス終了時) に計測結果のログ出力をおこないます。
                Logger.Exiting += (s, e) => Log();
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 計測を開始します。
        /// </summary>
        /// <param name="name">プロファイル名。</param>
        /// <returns>トランザクション。</returns>
        /// <remarks>
        /// 計測を開始して、トランザクションを返します。
        /// トランザクションの <see cref="IDisposable.Dispose()"/> を呼び出すと計測が終了します。
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

        #region エントリー書式コレクション

        /// <summary>
        /// エントリー書式を追加します。
        /// </summary>
        /// <param name="entryFormat">エントリー書式。</param>
        internal static void Add(EntryFormat entryFormat)
        {
            entryFormats.Add(entryFormat);
        }

        /// <summary>
        /// エントリー書式を挿入します。
        /// </summary>
        /// <param name="index">インデックス。</param>
        /// <param name="entryFormat">エントリー書式。</param>
        internal static void Insert(int index, EntryFormat entryFormat)
        {
            entryFormats.Insert(index, entryFormat);
        }

        /// <summary>
        /// エントリー書式を削除します。
        /// </summary>
        /// <param name="entryFormat">エントリー書式。</param>
        internal static void Remove(EntryFormat entryFormat)
        {
            entryFormats.Remove(entryFormat);
        }

        /// <summary>
        /// エントリー書式コレクションをクリアします。
        /// </summary>
        internal static void ClearEntryFormats()
        {
            entryFormats.Clear();
        }

        #endregion

        /// <summary>
        /// プロファイル結果をログ出力します。
        /// </summary>
        private static void Log()
        {
            Logger.Log(LogLevel, TitleFormat);

            foreach (var entry in Entries.OrderByDescending(p => p.TotalSeconds))
            {
                foreach (var format in EntryFormats)
                {
                    Logger.Log(LogLevel, format.Convert(entry));
                }
            }
        }

        #endregion
    }
}
