using System;
using System.Diagnostics;
using System.Threading;

namespace SoftCube.Log
{
    /// <summary>
    /// ログの書式。
    /// </summary>
    internal class LogFormat
    {
        #region プロパティ

        /// <summary>
        /// 書式。
        /// </summary>
        /// <remarks>
        /// 以下の変数を使用してログの書式を定義します。
        /// ・DateTime : ログを出力した時刻 (ローカルタイムゾーン)。
        /// ・File     : ログを出力したファイル名。
        /// ・Level    : ログレベル。
        /// ・Line     : ログを出力したファイル行番号。
        /// ・Message  : ログメッセージ。
        /// ・Method   : ログを出力したメソッド名。
        /// ・NewLine  : 改行文字。
        /// ・Thread   : ログを出力したスレッド番号。
        /// ・Type     : ログを出力した型名。
        /// </remarks>
        /// <example>
        /// 書式文字列は、以下の例のように指定します。
        /// ・"{DateTime:yyyy-MM-dd HH:mm:ss,fff} [{Level,-5}] - {Message}{NewLine}" → "2019-12-17 20:51:29,565 [INFO ] - message\r\n"
        /// </example>
        internal string Format { get; }

        /// <summary>
        /// 文字列フォーマット。
        /// </summary>
        private string StringFormat { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="format">変換パターン。<seealso cref="Format"/></param>
        internal LogFormat(string format)
        {
            Format = format;

            // 変換パターンを文字列フォーマットに置換します。
            // このとき部分置換を避けるために、文字数の多い変数から置換します。
            // 例えば、line を newline より先に置換してしまうと正しい文字列フォーマットに置換できません。
            format = format.Replace("DateTime", "5");
            format = format.Replace("Message", "0");
            format = format.Replace("NewLine", "1");
            format = format.Replace("Method",  "2");
            format = format.Replace("Thread",  "3");
            format = format.Replace("Level",   "4");
            format = format.Replace("File",    "6");
            format = format.Replace("Line",    "7");
            format = format.Replace("Type",    "8");
            StringFormat = format;
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
                    StringFormat,
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
                throw new InvalidOperationException($"ConversionPattern[{Format}]が不正です。");
            }
        }

        #endregion
    }
}
