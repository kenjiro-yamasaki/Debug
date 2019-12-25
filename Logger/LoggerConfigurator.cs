using SoftCube.Asserts;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SoftCube.Logger
{
    /// <summary>
    /// ロガーの構成。
    /// </summary>
    public class LoggerConfigurator : Attribute
    {
        #region プロパティ

        /// <summary>
        /// 構成ファイルパス。
        /// </summary>
        public string ConfigFilePath { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// ロガーを構成します。
        /// </summary>
        public void Configurate()
        {
            if (ConfigFilePath == null || !File.Exists(ConfigFilePath))
            {
                return;
            }
 
            // 構成ファイルを読み込み、ロガーを初期化します。
            Logger.ClearAndDisposeAppenders();

            var xml = XElement.Load(ConfigFilePath).Element("logger");
            foreach (var xappender in xml.Elements("appender"))
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

                var xparams  = xappender.Elements("param").ToDictionary(e => (string)e.Attribute("name"), e => (string)e.Attribute("value"));
                var appender = Activator.CreateInstance(appenderType, xparams) as Appender;
                Assert.NotNull(appender);

                Logger.Add(appender);
            }

            // プロセス終了時にアペンダーを破棄します。
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Logger.ClearAndDisposeAppenders();
        }

        #endregion
    }
}
