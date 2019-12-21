using System;

namespace SoftCube.Runtime
{
    /// <summary>
    /// システムクロック。
    /// </summary>
    public class SystemClock : ISystemClock
    {
        #region プロパティ

        /// <summary>
        /// コンピューター上の現在の日時 (現地時刻)。
        /// </summary>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// コンピューター上の現在の日時 (世界協定時刻)。
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public SystemClock()
        {
        }

        #endregion
    }
}
