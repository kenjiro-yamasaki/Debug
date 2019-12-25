using System;
using System.Diagnostics;
using System.Threading;

namespace SoftCube.Log
{
    /// <summary>
    /// ログ変換パターン。
    /// </summary>
    public class ConversionPattern
    {
        #region プロパティ

        /// <summary>
        /// 変換パターン。
        /// </summary>
        /// <remarks>
        /// 以下の変数を使用してログ出力の変換パターンを指定します。
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
        public string Pattern { get; }

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
        public ConversionPattern(string pattern)
        {
            Pattern = pattern;

            pattern = pattern.Replace("message", "0");
            pattern = pattern.Replace("newline", "1");
            pattern = pattern.Replace("method", "2");
            pattern = pattern.Replace("thread", "3");
            pattern = pattern.Replace("level", "4");
            pattern = pattern.Replace("date", "5");
            pattern = pattern.Replace("file", "6");
            pattern = pattern.Replace("line", "7");
            pattern = pattern.Replace("type", "8");
            Format = pattern;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 日付、レベル、ログメッセージ、スタックフレームをログに変換する。
        /// </summary>
        /// <param name="date">日付。</param>
        /// <param name="level">レベル。</param>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="stackFrame">スタックフレーム。</param>
        /// <returns>ログ。</returns>
        public string Convert(DateTime date, Level level, string message, StackFrame stackFrame)
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
