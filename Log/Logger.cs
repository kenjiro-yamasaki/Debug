﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftCube.Log
{
    /// <summary>
    /// ロガー。
    /// </summary>
    public static class Logger
    {
        #region 静的イベント

        /// <summary>
        /// 終了イベント。
        /// </summary>
        public static event EventHandler Exiting;

        /// <summary>
        /// 終了完了イベント。
        /// </summary>
        public static event EventHandler Exited;

        #endregion

        #region 静的プロパティ

        /// <summary>
        /// アペンダーコレクション。
        /// </summary>
        public static IReadOnlyList<Appender> Appenders => appenders;
        private static readonly List<Appender> appenders = new List<Appender>();

        #endregion

        #region 静的コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        static Logger()
        {
            var configurator = Assembly.GetEntryAssembly().GetCustomAttribute<LoggerConfigurator>();
            configurator?.Configurate();

            // プロセス終了時にアペンダーを破棄します。
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Exit();
        }

        #endregion

        #region 静的メソッド

        #region イベント発生

        /// <summary>
        /// 終了イベントを発生させます。
        /// </summary>
        private static void OnExiting()
        {
            Exiting?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// 終了完了イベントを発生させます。
        /// </summary>
        private static void OnEixted()
        {
            Exited?.Invoke(null, EventArgs.Empty);
        }

        #endregion

        #region アペンダーコレクション

        /// <summary>
        /// アペンダーを追加します。
        /// </summary>
        /// <param name="appender">アペンダー。</param>
        public static void Add(Appender appender)
        {
            lock (appenders)
            {
                appenders.Add(appender);
            }
        }

        /// <summary>
        /// アペンダーをクリアし、破棄します。
        /// </summary>
        public static void ClearAndDisposeAppenders()
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Dispose();
                }

                appenders.Clear();
            }
        }

        #endregion

        #region ログ出力

        /// <summary>
        /// トレースログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public static void Trace(string message)
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Trace(message);
                }
            }
        }

        /// <summary>
        /// デバッグログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public static void Debug(string message)
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Debug(message);
                }
            }
        }

        /// <summary>
        /// 情報ログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public static void Info(string message)
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Info(message);
                }
            }
        }

        /// <summary>
        /// 警告ログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public static void Warning(string message)
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Warning(message);
                }
            }
        }

        /// <summary>
        /// エラーログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public static void Error(string message)
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Error(message);
                }
            }
        }

        /// <summary>
        /// 致命的なエラーログを出力します。
        /// </summary>
        /// <param name="message">ログメッセージ。</param>
        public static void Fatal(string message)
        {
            lock (appenders)
            {
                foreach (var appender in appenders)
                {
                    appender.Fatal(message);
                }
            }
        }

        #endregion

        /// <summary>
        /// 終了します。
        /// </summary>
        private static void Exit()
        {
            OnExiting();

            ClearAndDisposeAppenders();

            OnEixted();
        }

        #endregion
    }
}