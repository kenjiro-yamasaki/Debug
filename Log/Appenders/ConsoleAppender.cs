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
            : this(new Clock())
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

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="clock">クロック。</param>
        internal ConsoleAppender(IClock clock)
            : base(clock)
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
