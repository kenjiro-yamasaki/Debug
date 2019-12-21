using System;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート例外。
    /// </summary>
    public class AssertException : Exception
    {
        #region プロパティ

        /// <summary>
        /// ユーザーメッセージ。
        /// </summary>
        public string UserMessage { get; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public AssertException()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="userMessage">ユーザーメッセージ。</param>
        public AssertException(string userMessage)
            : this(userMessage, null)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="userMessage">ユーザーメッセージ。</param>
        /// <param name="innerException">内部例外。</param>
        protected AssertException(string userMessage, Exception innerException)
            : base(userMessage, innerException)
        {
            UserMessage = userMessage;
        }

        #endregion
    }
}