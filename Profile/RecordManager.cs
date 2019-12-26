using SoftCube.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCube.Profile
{
    /// <summary>
    /// プロファイル記録マネージャー。
    /// </summary>
    internal static class RecordManager
    {
        #region 静的プロパティ

        /// <summary>
        /// プロファイル記録コレクション。
        /// </summary>
        private static IEnumerable<Record> Records => ProfileNameToRecord.Values;

        /// <summary>
        /// プロファイル名→プロファイル記録変換。
        /// </summary>
        private static Dictionary<string, Record> ProfileNameToRecord { get; } = new Dictionary<string, Record>();

        #endregion

        #region 静的コンストラクター

        static RecordManager()
        {
            Logger.Exiting += (s, e) => Log();
        }

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

        /// <summary>
        /// プロファイル結果をログ出力します。
        /// </summary>
        public static void Log()
        {
            Logger.Trace("----- プロファイル結果 -----");
            foreach (var record in Records.OrderByDescending(p => p.TotalSeconds))
            {
                record.Log();
            }
        }

        #endregion
    }
}
