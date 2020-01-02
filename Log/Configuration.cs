using SoftCube.Asserts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SoftCube.Log
{
    /// <summary>
    /// <see cref="Logger"/> の構成。
    /// </summary>
    public class Configuration : Attribute
    {
        #region プロパティ

        /// <summary>
        /// 構成ファイルパス。
        /// </summary>
        public string ConfigFilePath { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="Logger"/> を構成します。
        /// </summary>
        internal void Configurate()
        {
            if (ConfigFilePath == null)
            {
                throw new InvalidOperationException($"ログ構成に失敗しました。{GetType().FullName}.{nameof(ConfigFilePath)} が null です。");
            }
            if (!File.Exists(ConfigFilePath))
            {
                throw new InvalidOperationException(
                    $"ログ構成に失敗しました。{ConfigFilePath} が存在しません。" +
                    $"よくある原因は、次のとおりです。" +
                    $"(1) ファイルパスの綴りが間違っている。" +
                    $"(2) {ConfigFilePath} のプロパティ＞出力ディレクトリーにコピーが「コピーしない」になっている (「新しい場合はコピーする」に変更してください)。");
            }

            // 構成ファイルを読み込み、ロガーを構成します。
            var xlogger = XElement.Load(ConfigFilePath).Element("logger");

            var appenders = new Dictionary<string, Appender>();
            foreach (var xappender in xlogger.Elements("appender"))
            {
                var appenderName     = (string)xappender.Attribute("name");
                var appenderTypeName = (string)xappender.Attribute("type");
                var appenderType     = Type.GetType(appenderTypeName);
                if (appenderType == null)
                {
                    throw new IOException(
                        $"ロガー構成ファイル {ConfigFilePath} の書式が不正です。{Environment.NewLine}" +
                        $"appender タグの type 属性に {appenderTypeName} を指定することはできません。{Environment.NewLine}" +
                        $"この属性には appender の正確な型名 (例：{typeof(FileAppender).FullName}) を指定してください。");
                }

                var appender = Activator.CreateInstance(appenderType, xappender) as Appender;
                Assert.NotNull(appender);

                appenders.Add(appenderName, appender);
            }

            foreach (var xuseAppender in xlogger.Elements("use-appender"))
            {
                var useAppenderName = (string)xuseAppender.Attribute("name");
                if (appenders.ContainsKey(useAppenderName))
                {
                    Logger.Add(appenders[useAppenderName]);
                }
            }
        }

        #endregion
    }
}
