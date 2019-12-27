using SoftCube.Configuration;
using SoftCube.Runtime;
using System;
using System.Diagnostics;
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
            SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            LogFormat = "{Message}";
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public Appender(XElement xappender)
            : this(new SystemClock())
        {
            if (xappender == null)
            {
                throw new ArgumentNullException(nameof(xappender));
            }

            LogFormat = xappender.Property(nameof(LogFormat));
            MinLevel  = xappender.Property(nameof(MinLevel)).ToLevel();
            MaxLevel  = xappender.Property(nameof(MaxLevel)).ToLevel();
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
        public void Trace(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var level = Level.Trace;
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
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
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
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
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
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
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
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
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
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
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
            }
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="message">ログメッセージ。</param>
        public void Log(Level level, string message)
        {
            if (MinLevel <= level && level <= MaxLevel)
            {
                Log(logFormat.Convert(SystemClock.Now, level, message, new StackFrame(1, true)));
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
