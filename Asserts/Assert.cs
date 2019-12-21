using System;
using System.ComponentModel;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        /// <summary>
        /// このメソッドを呼び出さないでください。
        /// </summary>
        /// <exception cref="InvalidOperationException">このメソッドが呼び出された場合、投げられます。</exception>
        [Obsolete("This is an override of Object.Equals(). Call Assert.Equal() instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static bool Equals(object a, object b)
        {
            throw new InvalidOperationException("Assert.Equals should not be used");
        }

        /// <summary>
        /// このメソッドを呼び出さないでください。
        /// </summary>
        /// <exception cref="InvalidOperationException">このメソッドが呼び出された場合、投げられます。</exception>
        [Obsolete("This is an override of Object.ReferenceEquals(). Call Assert.Same() instead.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static bool ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used");
        }

        #endregion
    }
}
