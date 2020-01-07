using SoftCube.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace SoftCube.Profiling
{
    /// <summary>
    /// <see cref="Profiler"> の構成。
    /// </summary>
    public class Configuration : Attribute
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
        public Configuration()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// <see cref="Profiler"> を構成します。
        /// </summary>
        internal void Configurate()
        {
            if (ConfigFileName == null)
            {
                throw new InvalidOperationException($"プロファイル構成に失敗しました。{GetType().FullName}.{nameof(ConfigFileName)} が null です。");
            }

            var entryAssemblyPath = Assembly.GetEntryAssembly().Location;
            var configFilePath    = Path.Combine(Path.GetDirectoryName(entryAssemblyPath), ConfigFileName);
            if (!File.Exists(configFilePath))
            {
                throw new InvalidOperationException(
                    $"Profiling 構成に失敗しました。{configFilePath} が存在しません。" +
                    $"よくある原因は、次のとおりです。" +
                    $"(1) ファイルパスの綴りが間違っている。" +
                    $"(2) {ConfigFileName} のプロパティ＞出力ディレクトリーにコピーが「コピーしない」になっている (「新しい場合はコピーする」に変更してください)。");
            }

            // 構成ファイルを読み込み、プロファイラーを構成します。
            Debug.WriteLine($"{configFilePath} を読み込み、ロガーを構成します。");
            var xprofiler = XElement.Load(configFilePath).Element("profiling");
            if (xprofiler == null)
            {
                throw new IOException(
                    $"Profiling 構成ファイル {ConfigFileName} の書式が不正です。" +
                    $"profiling 要素を定義してください。");
            }

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
