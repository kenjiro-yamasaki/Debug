using SoftCube.Configuration;
using SoftCube.Log;
using System;
using System.IO;
using System.Xml.Linq;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイラーの構成。
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
        /// ロガーを構成します。
        /// </summary>
        internal void Configurate()
        {
            if (ConfigFilePath == null || !File.Exists(ConfigFilePath))
            {
                return;
            }

            // 構成ファイルを読み込み、ロガーを初期化します。
            var xprofiler = XElement.Load(ConfigFilePath).Element("profiler");

            Profiler.LogLevel    = xprofiler.Property(nameof(Profiler.LogLevel)).ToLevel();
            Profiler.TitleFormat = xprofiler.Property(nameof(Profiler.TitleFormat));

            foreach (var format in xprofiler.Properties(nameof(EntryFormat)))
            {
                Profiler.Add(new EntryFormat(format));
            }
        }

        #endregion
    }
}
