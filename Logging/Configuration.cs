using SoftCube.Asserts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace SoftCube.Logging
{
    /// <summary>
    /// <see cref="Logger"/> の構成。
    /// </summary>
    public class Configuration : Attribute
    {
        #region プロパティ

        /// <summary>
        /// 構成ファイル名。
        /// </summary>
        public string ConfigFileName { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="Logger"/> を構成します。
        /// </summary>
        internal void Configurate()
        {
            if (ConfigFileName == null)
            {
                throw new InvalidOperationException($"ログ構成に失敗しました。{GetType().FullName}.{nameof(ConfigFileName)} が null です。");
            }

            var entryAssemblyPath = Assembly.GetEntryAssembly().Location;
            var configFilePath    = Path.Combine(Path.GetDirectoryName(entryAssemblyPath), ConfigFileName);
            if (!File.Exists(configFilePath))
            {
                throw new InvalidOperationException(
                    $"Logging 構成に失敗しました。{configFilePath} が存在しません。" +
                    $"よくある原因は、次のとおりです。" +
                    $"(1) ファイルパスの綴りが間違っている。" +
                    $"(2) {ConfigFileName} のプロパティ＞出力ディレクトリーにコピーが「コピーしない」になっている (「新しい場合はコピーする」に変更してください)。");
            }

            // 構成ファイルを読み込み、ロガーを構成します。
            Debug.WriteLine($"{configFilePath} を読み込み、ロガーを構成します。");
            var xlogger = XElement.Load(configFilePath).Element("logging");
            if (xlogger == null)
            {
                throw new IOException(
                    $"Logging 構成ファイル {ConfigFileName} の書式が不正です。" +
                    $"logging 要素を定義してください。");
            }

            var appenders = new Dictionary<string, Appender>();
            foreach (var xappender in xlogger.Elements("appender"))
            {
                var appenderName     = (string)xappender.Attribute("name");
                var appenderTypeName = (string)xappender.Attribute("type");
                var appenderType     = Type.GetType(appenderTypeName);
                if (appenderType == null)
                {
                    throw new IOException(
                        $"Logging 構成ファイル {ConfigFileName} の書式が不正です。" +
                        $"appender の type 属性に {appenderTypeName} を指定することはできません。" +
                        $"この属性には appender の正確な型名 (例：{typeof(FileAppender).FullName}) を指定してください。");
                }

                var appender = Activator.CreateInstance(appenderType, xappender) as Appender;
                Assert.NotNull(appender);

                appenders.Add(appenderName, appender);
            }

            foreach (var xuseAppender in xlogger.Elements("use-appender"))
            {
                var useAppenderName = (string)xuseAppender.Attribute("name");
                if (!appenders.ContainsKey(useAppenderName))
                {
                    throw new IOException(
                        $"Logging 構成ファイル {ConfigFileName} の書式が不正です。" +
                        $"use-appender [{useAppenderName}] の参照先の appender が存在しません。");
                }
                Logger.Add(appenders[useAppenderName]);
            }
        }

        #endregion
    }
}
