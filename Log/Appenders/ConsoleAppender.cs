using SoftCube.Runtime;
using System;
using System.Xml.Linq;

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
        /// <param name="xappender">XML の appender 要素。</param>
        public ConsoleAppender(XElement xappender)
            : base(xappender)
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public override void Log(string log)
        {
            Console.Write(log);
        }

        #endregion
    }
}
