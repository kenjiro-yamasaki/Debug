﻿using System;

namespace SoftCube.Logging
{
    /// <summary>
    /// ログレベル。
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// トレースログ。
        /// </summary>
        /// <remarks>
        /// デバッグログよりも、更に詳細な情報。
        /// </remarks>
        Trace = 0,

        /// <summary>
        /// デバッグログ。
        /// </summary>
        /// <remarks>
        /// システムの動作状況に関する詳細な情報。
        /// </remarks>
        Debug = 1,

        /// <summary>
        /// 情報ログ。
        /// </summary>
        /// <remarks>
        /// 実行時の何らかの注目すべき事象（開始や終了など)。
        /// メッセージ内容は簡潔に止めるべき。
        /// </remarks>
        Info = 2,

        /// <summary>
        /// 警告ログ。
        /// </summary>
        /// <remarks>
        /// 廃止となったAPIの使用、APIの不適切な使用、エラーに近い事象など。
        /// 実行時に生じた異常とは言い切れないが正常とも異なる何らかの予期しない問題。
        /// </remarks>
        Warning = 3,

        /// <summary>
        /// エラーログ。
        /// </summary>
        /// <remarks>
        /// 予期しない実行時エラー。
        /// </remarks>
        Error = 4,

        /// <summary>
        /// 致命的なエラーログ。
        /// </summary>
        /// <remarks>
        /// プログラムの異常終了を伴うようなもの。
        /// </remarks>
        Fatal = 5,
    }

    /// <summary>
    /// <see cref="Level"/> の拡張メソッド。
    /// </summary>
    public static class LevelExtensions
    {
        #region メソッド

        /// <summary>
        /// ログレベルを表示名に変換します。
        /// </summary>
        /// <param name="level">ログレベル。</param>
        /// <returns>表示名。</returns>
        public static string ToDisplayName(this Level level)
        {
            switch (level)
            {
                case Level.Trace:
                    return nameof(Level.Trace);

                case Level.Debug:
                    return nameof(Level.Debug);

                case Level.Info:
                    return nameof(Level.Info);

                case Level.Warning:
                    return nameof(Level.Warning);

                case Level.Error:
                    return nameof(Level.Error);

                case Level.Fatal:
                    return nameof(Level.Fatal);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// ログレベルの表示名をログレベルに変換します。
        /// </summary>
        /// <param name="displayName">ログレベルの表示名。</param>
        /// <returns>ログレベル。</returns>
        public static Level ToLevel(this string displayName)
        {
            switch (displayName)
            {
                case nameof(Level.Trace):
                    return Level.Trace;

                case nameof(Level.Debug):
                    return Level.Debug;

                case nameof(Level.Info):
                    return Level.Info;

                case nameof(Level.Warning):
                    return Level.Warning;

                case nameof(Level.Error):
                    return Level.Error;

                case nameof(Level.Fatal):
                    return Level.Fatal;

                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}
