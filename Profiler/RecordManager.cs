using System;
using System.Collections.Generic;

namespace SoftCube.Profiler
{
    /// <summary>
    /// プロファイル記録マネージャー。
    /// </summary>
    public static class RecordManager
    {
        #region 静的プロパティ

        /// <summary>
        /// プロファイル記録コレクション。
        /// </summary>
        public static IEnumerable<Record> Records => ProfileNameToRecord.Values;

        /// <summary>
        /// プロファイル名→プロファイル記録変換。
        /// </summary>
        private static Dictionary<string, Record> ProfileNameToRecord { get; } = new Dictionary<string, Record>();

        #endregion

        #region 静的メソッド

        #region プロファイル記録コレクション

        /// <summary>
        /// プロファイル記録が含まれているかを判断します。
        /// </summary>
        /// <param name="profileName">プロファイル名。</param>
        /// <returns>プロファイル記録が含まれているか。</returns>
        internal static bool ContainsRecord(string profileName)
        {
            if (profileName == null)
            {
                throw new ArgumentNullException(nameof(profileName));
            }

            return ProfileNameToRecord.ContainsKey(profileName);
        }

        /// <summary>
        /// プロファイル記録を追加します。
        /// </summary>
        /// <param name="record">プロファイル記録。</param>
        internal static void AddRecord(Record record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            ProfileNameToRecord.Add(record.Name, record);
        }

        /// <summary>
        /// プロファイル記録を取得します。
        /// </summary>
        /// <param name="profileName">プロファイル名。</param>
        /// <returns>プロファイル記録。</returns>
        internal static Record GetRecord(string profileName)
        {
            if (profileName == null)
            {
                throw new ArgumentNullException(nameof(profileName));
            }

            return ProfileNameToRecord[profileName];
        }

        #endregion

        ///// <summary>
        ///// プロファイル結果をログ出力する。
        ///// </summary>
        //public static void OutputLog()
        //{
        //    Logger.Trace("----- プロファイル結果 -----");
        //    foreach (Profile profile in profiles.Values.OrderByDescending(p => p.TotalTime))
        //    {
        //        Assert.IsTrue(profile != null);
        //        Assert.IsTrue(1 <= profile.Count);
        //        profile.OutputLog();
        //    }
        //}

        #endregion
    }
}
