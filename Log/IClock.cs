using System;

namespace SoftCube.Log
{
    /// <summary>
    /// クロック。
    /// </summary>
    public interface IClock
    {
        #region プロパティ

        /// <summary>
        /// コンピューター上の現在の日時 (現地時刻)。
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// コンピューター上の現在の日時 (世界協定時刻)。
        /// </summary>
        DateTime UtcNow { get; }

        #endregion
    }
}
