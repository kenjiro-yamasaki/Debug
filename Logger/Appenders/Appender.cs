using SoftCube.System;
using System;
using System.Diagnostics;
using System.Threading;

namespace SoftCube.Logger
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
                    replacedConversionPattern = new Lazy<string>(() => {

                        // 置換が成功するように、長い文字列から置換します。
                        // たとえば、newlineより先にlineを置換すると正しく置換されません。
                        value = value.Replace("message", "0");
                        value = value.Replace("newline", "1");
                        value = value.Replace("method",  "2");
                        value = value.Replace("thread",  "3");
                        value = value.Replace("level",   "4");
                        value = value.Replace("date",    "5");
                        value = value.Replace("file",    "6");
                        value = value.Replace("line",    "7");
                        value = value.Replace("type",    "8");

                        return value;
                    });
                }
            }
        }
        private string conversionPattern;
        private Lazy<string> replacedConversionPattern = null;

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
        /// <param name="disposing"><see cref="IDisposable.Dispose"/> から呼び出されたかを示す値。</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        #region ログ

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

            Log(Level.Trace, message);
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

            Log(Level.Debug, message);
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

            Log(Level.Info, message);
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

            Log(Level.Warning, message);
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

            Log(Level.Error, message);
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

            Log(Level.Fatal, message);
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public abstract void Log(string log);

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="message">ログメッセージ。</param>
        private void Log(Level level, string message)
        {
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(Format(level, message));
            }
        }

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
                var stackFrame = new StackFrame(3, true);
                var type       = stackFrame.GetMethod().DeclaringType.FullName;
                var method     = stackFrame.GetMethod().Name;
                var file       = stackFrame.GetFileName();
                var line       = stackFrame.GetFileLineNumber();
                var date       = SystemClock.Now;
                var newline    = Environment.NewLine;
                var thread     = Thread.CurrentThread.ManagedThreadId;

                return string.Format(
                    replacedConversionPattern?.Value,
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
