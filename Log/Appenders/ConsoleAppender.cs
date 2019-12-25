using SoftCube.Runtime;
using System;
using System.Collections.Generic;

namespace SoftCube.Log
{
    /// <summary>
    /// コンソールアペンダー。
    /// </summary>
    public class ConsoleAppender : Appender
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public ConsoleAppender()
            : this(new SystemClock())
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public ConsoleAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xparams">パラメーター名→値変換。</param>
        public ConsoleAppender(IReadOnlyDictionary<string, string> xparams)
            : base(xparams)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <param name="log">ログ。</param>
        public override void Log(Level level, string log)
        {
            Console.WriteLine(log);
        }

        #endregion
    }
}
