using System;

namespace SoftCube.Logger.Config
{
    /// <summary>
    /// XML 構成属性。
    /// </summary>
    public class XmlConfiguratorAttribute : Attribute
    {
        #region プロパティ

        /// <summary>
        /// 構成ファイル名。
        /// </summary>
        public string ConfigFileName { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="configFilePath">構成ファイル名。</param>
        public XmlConfiguratorAttribute(string configFilePath)
        {
            ConfigFileName = configFilePath ?? throw new ArgumentNullException(nameof(configFilePath));
        }

        #endregion
    }
}
