using System;

namespace SoftCube.Log
{
    /// <summary>
    /// クロック。
    /// </summary>
    public class Clock : IClock
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
        public Clock()
        {
        }

        #endregion
    }
}
