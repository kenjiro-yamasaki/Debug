using SoftCube.Asserts;
using SoftCube.Logger.Appenders;
using SoftCube.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SoftCube.Logger
{
    /// <summary>
    /// ロガー。
    /// </summary>
    public static class Logger
    {
        #region 静的プロパティ

        /// <summary>
        /// アペンダーコレクション。
        /// </summary>
        public static IReadOnlyList<Appender> Appenders => appenders;
        private static readonly List<Appender> appenders = new List<Appender>();

        #endregion

        #region 静的コンストラクター

        /// <summary>
        /// 静的コンストラクター。
        /// </summary>
        static Logger()
        {
            ReadConfigulation();
        }

        #endregion

        #region 静的メソッド

        #region 構成

        /// <summary>
        /// 構成する。
        /// </summary>
        private static void ReadConfigulation()
        {
            var configFilePath = Path.Combine(SystemConstants.ExecutableDirectoryPath, "Logger.config");

            var logger = XElement.Load(configFilePath).Element("logger");

            foreach (var xappender in logger.Elements("appender"))
            {
                var name = (string)xappender.Attribute("name");
                var type = (string)xappender.Attribute("type");

                var @params  = xappender.Elements("param").ToDictionary(e => (string)e.Attribute("name"), e => (string)e.Attribute("value"));
                var appender = Activator.CreateInstance(Type.GetType(type), @params) as Appender;
                Assert.NotNull(appender);

                Add(appender);
            }
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

        #endregion
    }
}
