using System.Xml.Linq;

namespace SoftCube.Logging
{
    /// <summary>
    /// 診断アペンダー。
    /// </summary>
    public class DiagnosticsAppender : Appender
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public DiagnosticsAppender()
            : this(new Clock())
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="xappender">XML の appender 要素。</param>
        public DiagnosticsAppender(XElement xappender)
            : base(xappender)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="clock">クロック。</param>
        internal DiagnosticsAppender(IClock clock)
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
            System.Diagnostics.Trace.Write(log);
        }

        #endregion
    }
}
