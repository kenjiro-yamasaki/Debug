using SoftCube.IO;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SoftCube.Log
{
    /// <summary>
    /// アペンダー。
    /// </summary>
    public abstract class Appender : IDisposable
    {
        #region プロパティ

        /// <summary>
        /// ログの書式。
        /// </summary>
        /// <seealso cref="Log.LogFormat.Format"/>
        public string LogFormat
        {
            get => logFormat.Format;
            set => logFormat = new LogFormat(value);
        }
        private LogFormat logFormat;

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
        protected IClock SystemClock { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public Appender()
            : this(new Clock())
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public Appender(XElement xappender)
            : this(new Clock())
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            LogFormat = xappender.Property(nameof(LogFormat));
            MinLevel  = xappender.Property(nameof(MinLevel)).ToLevel();
            MaxLevel  = xappender.Property(nameof(MaxLevel)).ToLevel();
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        internal Appender(IClock systemClock)
        {
            SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            LogFormat = "{Message}";
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
        /// <param name="disposing">
        /// <see cref="IDisposable.Dispose"/> から呼び出されたか。
        /// <c>true</c> の場合、マネージリソースを破棄します。
        /// <c>false</c> の場合、マネージリソースを破棄しないでください。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        #region ログ出力

        /// <summary>
        /// トレースログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Trace(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Trace;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Debug;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Info;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Warning(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Warning;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Error;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// 致命的なエラーログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Fatal(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Fatal;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="message">ログメッセージ。</param>
        /// <param name="file">ログを出力したファイル名。</param>
        /// <param name="line">ログを出力したファイル行番号。</param>
        /// <param name="method">ログを出力したメソッド名。</param>
        public void Log(Level level, string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, file, level, line, message, method));
            }
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public abstract void Log(string log);

        #endregion

        #endregion
    }
}
