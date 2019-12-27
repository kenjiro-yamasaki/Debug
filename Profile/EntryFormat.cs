using System;

namespace SoftCube.Profile
{
    /// <summary>
    /// エントリーのログ書式。
    /// </summary>
    internal class EntryFormat
    {
        #region プロパティ

        /// <summary>
        /// ログ書式。
        /// </summary>
        /// <remarks>
        /// 以下の変数を使用してエントリーのログ書式を定義します。
        /// ・Name           : プロファイル名。
        /// ・Count          : 計測回数。
        /// ・TotalSeconds   : 合計計測時間 (秒)。
        /// ・AverageSeconds : 平均計測時間 (秒)。
        /// ・MinSeconds     : 最小計測時間 (秒)。
        /// ・MaxSeconds     : 最大計測時間 (秒)。
        /// ・MinIndex       : 最小計測インデックス (0～)。
        /// ・MaxIndex       : 最大計測インデックス (0～)。
        /// ・MinNumber      : 最小計測番号 (= MinIndex + 1)。
        /// ・MaxNumber      : 最大計測番号 (= MaxIndex + 1)。
        /// </remarks>
        /// <example>
        /// 変換パターンは、以下の例のように指定します。
        /// ・"合計計測時間 = {TotalSeconds:F5} (秒)" → ""
        /// </example>
        public string Format { get; }

        /// <summary>
        /// 内部ログ書式。
        /// </summary>
        private string InnerFormat { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="format">ログ書式。<seealso cref="Format"/></param>
        public EntryFormat(string format)
        {
            Format = format;

            // 変換パターンを文字列フォーマットに置換します。
            // このとき部分置換を避けるために、文字数の多い変数から置換します。
            format = format.Replace("AverageSeconds", "3");
            format = format.Replace("TotalSeconds",   "2");
            format = format.Replace("MinSeconds",     "4");
            format = format.Replace("MaxSeconds",     "5");
            format = format.Replace("MinNumber",      "8");
            format = format.Replace("MaxNumber",      "9");
            format = format.Replace("MinIndex",       "6");
            format = format.Replace("MaxIndex",       "7");
            format = format.Replace("Count",          "1");
            format = format.Replace("Name",           "0");

            InnerFormat = format;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// エントリーをログに変換します。
        /// </summary>
        /// <param name="entry">エントリー。</param>
        /// <returns>ログ。</returns>
        internal string Convert(Entry entry)
        {
            try
            {
                return string.Format(
                    InnerFormat,
                    entry.Name,
                    entry.Count,
                    entry.TotalSeconds,
                    entry.AverageSeconds,
                    entry.MinSeconds,
                    entry.MaxSeconds,
                    entry.MinIndex,
                    entry.MaxIndex,
                    entry.MinIndex + 1,
                    entry.MaxIndex + 1);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException($"ConversionPattern[{Format}]が不正です。");
            }
        }

        #endregion
    }
}
