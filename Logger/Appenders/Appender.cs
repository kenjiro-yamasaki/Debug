using SoftCube.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SoftCube.Logger.Appenders
{
    /// <summary>
    /// アペンダー。
    /// </summary>
    public abstract class Appender : IDisposable
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
        public string ConversionPattern
        {
            get => conversionPattern;
            set
            {
                if (conversionPattern != value)
                {
                    conversionPattern = value;

                    // 置換が成功するように、長い文字列から置換します。
                    // たとえば、newlineより先にlineを置換すると正しく置換されません。
                    replacedConversionPattern = value;
                    replacedConversionPattern = replacedConversionPattern.Replace("message", "0");
                    replacedConversionPattern = replacedConversionPattern.Replace("newline", "1");
                    replacedConversionPattern = replacedConversionPattern.Replace("method", "2");
                    replacedConversionPattern = replacedConversionPattern.Replace("thread", "3");
                    replacedConversionPattern = replacedConversionPattern.Replace("level", "4");
                    replacedConversionPattern = replacedConversionPattern.Replace("date", "5");
                    replacedConversionPattern = replacedConversionPattern.Replace("file", "6");
                    replacedConversionPattern = replacedConversionPattern.Replace("line", "7");
                    replacedConversionPattern = replacedConversionPattern.Replace("type", "8");
                }
            }
        }
        private string conversionPattern;
        private string replacedConversionPattern;

        /// <summary>
        /// 最小レベル。
        /// </summary>
        /// <remarks>
        /// 最小レベル以上のログのみを出力します。
        /// </remarks>
        public Level MinLevel { get; set; } = Level.Trace;

        /// <summary>
        /// 最大レベル。
        /// </summary>
        /// <remarks>
        /// 最大レベル以下のログのみを出力します。
        /// </remarks>
        public Level MaxLevel { get; set; } = Level.Fatal;

        /// <summary>
        /// システムクロック。
        /// </summary>
        protected ISystemClock SystemClock { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public Appender()
            : this(new SystemClock())
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public Appender(ISystemClock systemClock)
        {
            ConversionPattern = "{message}";
            SystemClock       = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="params">パラメーター名→値変換。</param>
        public Appender(IReadOnlyDictionary<string, string> @params)
            : this(new SystemClock())
        {
            if (@params == null)
            {
                throw new ArgumentNullException(nameof(@params));
            }

            ConversionPattern = @params[nameof(ConversionPattern)];
            MinLevel          = @params[nameof(MinLevel)].ToLevel();
            MaxLevel          = @params[nameof(MaxLevel)].ToLevel();
        }

        #endregion

        #region メソッド

        #region 破棄

        /// <summary>
        /// ファイナライザー。
        /// </summary>
        ~Appender()
        {
            Dispose(false);
        }

        /// <summary>
        /// 破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 破棄します。
        /// </summary>
        /// <param name="disposing"><see cref="IDisposable.Dispose"/> から呼び出されたか。</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        #region ログ出力

        /// <summary>
        /// トレースログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public void Trace(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Trace;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(level, Format(level, message));
            }
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public void Debug(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Debug;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(level, Format(level, message));
            }
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public void Info(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Info;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(level, Format(level, message));
            }
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public void Warning(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Warning;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(level, Format(level, message));
            }
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public void Error(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Error;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(level, Format(level, message));
            }
        }

        /// <summary>
        /// 致命的なエラーログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public void Fatal(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Fatal;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(level, Format(level, message));
            }
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="log">ログ。</param>
        public abstract void Log(Level level, string log);

        /// <summary>
        /// 変換パターンを使用して、ログをフォーマットします。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="message">ログメッセージ。</param>
        /// <returns>ログ。</returns>
        private string Format(Level level, string message)
        {
            try
            {
                var stackFrame = new StackFrame(2, true);
                var type       = stackFrame.GetMethod().DeclaringType.FullName;
                var method     = stackFrame.GetMethod().Name;
                var file       = stackFrame.GetFileName();
                var line       = stackFrame.GetFileLineNumber();
                var date       = SystemClock.Now;
                var newline    = Environment.NewLine;
                var thread     = Thread.CurrentThread.ManagedThreadId;

                return string.Format(
                    replacedConversionPattern,
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
                throw new InvalidOperationException($"ConversionPattern[{ConversionPattern}]が不正です。");
            }
        }

        #endregion

        #endregion
    }
}
