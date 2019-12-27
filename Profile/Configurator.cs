using SoftCube.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイラーの構成。
    /// </summary>
    public class Configurator : Attribute
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
            Profiler.LogLevel    = xprofiler.Param(nameof(Profiler.LogLevel)).ToLevel();
            Profiler.TitleFormat = xprofiler.Param(nameof(Profiler.TitleFormat));

            foreach (var format in xprofiler.Params(nameof(EntryFormat)))
            {
                Profiler.Add(new EntryFormat(format));
            }
        }

        #endregion
    }

    internal static class XElementExtensions
    {
        public static string Param(this XElement xelement, string name)
        {
            return (string)xelement.Elements("param").Single(e => (string)e.Attribute("name") == name).Attribute("value");
        }

        public static IEnumerable<string> Params(this XElement xelement, string name)
        {
            return xelement.Elements("param").Where(e => (string)e.Attribute("name") == name).Select(e => (string)e.Attribute("value"));
        }
    }
}
