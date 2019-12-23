﻿using SoftCube.Runtime;
using System.Collections.Generic;
using System.Text;

namespace SoftCube.Logger.Appenders
{
    /// <summary>
    /// 文字列アペンダー。
    /// </summary>
    public class StringAppender : Appender
    {
        #region プロパティ

        /// <summary>
        /// 文字列ビルダー。
        /// </summary>
        private StringBuilder StringBuilder { get; } = new StringBuilder();

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public StringAppender()
            : this(new SystemClock())
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public StringAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="params">パラメーター名→値変換。</param>
        public StringAppender(IReadOnlyDictionary<string, string> @params)
            : base(@params)
        {
        }

        #endregion

        #region メソッド

        #region ログ出力

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="log">ログ。</param>
        public override void Log(Level level, string log)
        {
            lock (StringBuilder)
            {
                StringBuilder.Append(log);
            }
        }

        #endregion

        #region 変換

        /// <summary>
        /// 文字列に変換する。
        /// </summary>
        /// <returns>文字列。</returns>
        public override string ToString()
        {
            lock (StringBuilder)
            {
                return StringBuilder.ToString();
            }
        }

        #endregion

        #endregion
    }
}
