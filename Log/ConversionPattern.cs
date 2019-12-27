using System;
using System.Diagnostics;
using System.Threading;

namespace SoftCube.Log
{
    /// <summary>
    /// ログ変換パターン。
    /// </summary>
    internal class ConversionPattern
    {
        #region プロパティ

        /// <summary>
        /// 変換パターン。
        /// </summary>
        /// <remarks>
        /// 以下の変数を使用してログ出力の変換パターンを定義します。
        /// ・date    : ログを出力した時刻 (ローカルタイムゾーン)。
        /// ・file    : ログを出力したファイル名。
        /// ・level   : ログレベル。
        /// ・line    : ログを出力したファイル行番号。
        /// ・message : ログメッセージ。
        /// ・method  : ログを出力したメソッド名。
        /// ・newline : 改行文字。
        /// ・thread  : ログを出力したスレッド番号。
        /// ・type    : ログを出力した型名。
        /// </remarks>
        /// <example>
        /// 変換パターンは、以下の例のように指定します。
        /// ・"{date:yyyy-MM-dd HH:mm:ss,fff} [{level,-5}] - {message}{newline}" → "2019-12-17 20:51:29,565 [INFO ] - message\r\n"
        /// </example>
        internal string Pattern { get; }

        /// <summary>
        /// 文字列フォーマット。
        /// </summary>
        private string Format { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="pattern">変換パターン。<seealso cref="Pattern"/></param>
        internal ConversionPattern(string pattern)
        {
            Pattern = pattern;

            // 変換パターンを文字列フォーマットに置換します。
            // このとき部分置換を避けるために、文字数の多い変数から置換します。
            // 例えば、line を newline より先に置換してしまうと正しい文字列フォーマットに置換できません。
            pattern = pattern.Replace("Message", "0");
            pattern = pattern.Replace("NewLine", "1");
            pattern = pattern.Replace("Method",  "2");
            pattern = pattern.Replace("Thread",  "3");
            pattern = pattern.Replace("Level",   "4");
            pattern = pattern.Replace("Date",    "5");
            pattern = pattern.Replace("File",    "6");
            pattern = pattern.Replace("Line",    "7");
            pattern = pattern.Replace("Type",    "8");
            Format = pattern;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 日付、レベル、ログメッセージ、スタックフレームをログに変換します。
        /// </summary>
        /// <param name="date">日付。</param>
        /// <param name="level">レベル。</param>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="stackFrame">スタックフレーム。</param>
        /// <returns>ログ。</returns>
        internal string Convert(DateTime date, Level level, string message, StackFrame stackFrame)
        {
            try
            {
                var type    = stackFrame.GetMethod().DeclaringType.FullName;
                var method  = stackFrame.GetMethod().Name;
                var file    = stackFrame.GetFileName();
                var line    = stackFrame.GetFileLineNumber();
                var newline = Environment.NewLine;
                var thread  = Thread.CurrentThread.ManagedThreadId;

                return string.Format(
                    Format,
                    message,
                    newline,
                    method,
                    thread,
                    level.ToDisplayName(),
                    date,
                    file,
                    line,
                    type);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException($"ConversionPattern[{Pattern}]が不正です。");
            }
        }

        #endregion
    }
}
