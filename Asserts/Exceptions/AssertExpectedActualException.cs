using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SoftCube.Asserts
{
    /// <summary>
    /// 期待値と実測値のアサート例外。
    /// </summary>
    public class AssertExpectedActualException : AssertException
    {
        #region プロパティ

        /// <summary>
        /// 期待値。
        /// </summary>
        public string Expected { get; }

        /// <summary>
        /// 期待値のタイトル。
        /// </summary>
        public string ExpectedTitle { get; }

        /// <summary>
        /// 実測値。
        /// </summary>
        public string Actual { get; }

        /// <summary>
        /// 実測値のタイトル。
        /// </summary>
        public string ActualTitle { get; }

        /// <summary>
        /// 例外メッセージ (期待値と実測値を含む)。
        /// </summary>
        public override string Message
        {
            get
            {
                var titleLength            = Math.Max(ExpectedTitle.Length, ActualTitle.Length) + 2;  // + the colon and space
                var formattedExpectedTitle = (ExpectedTitle + ":").PadRight(titleLength);
                var formattedActualTitle   = (ActualTitle + ":").PadRight(titleLength);

                return $"{base.Message}{Environment.NewLine}{formattedExpectedTitle}{Expected ?? "(null)"}{Environment.NewLine}{formattedActualTitle}{Actual ?? "(null)"}";
            }
        }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="userMessage">ユーザーメッセージ。</param>
        /// <param name="expectedTitle">期待値のタイトル。</param>
        /// <param name="actualTitle">実測値のタイトル。</param>
        public AssertExpectedActualException(object expected, object actual, string userMessage, string expectedTitle = null, string actualTitle = null)
            : this(expected, actual, userMessage, expectedTitle, actualTitle, null)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="message">メッセージ。</param>
        /// <param name="expectedTitle">期待値のタイトル。</param>
        /// <param name="actualTitle">実測値のタイトル。</param>
        /// <param name="innerException">内部例外。</param>
        public AssertExpectedActualException(object expected, object actual, string message, string expectedTitle, string actualTitle, Exception innerException)
            : base(message, innerException)
        {
            Actual        = actual == null ? null : ConvertToString(actual);
            ActualTitle   = actualTitle ?? "Actual";
            Expected      = expected == null ? null : ConvertToString(expected);
            ExpectedTitle = expectedTitle ?? "Expected";

            if (actual != null && expected != null && Actual == Expected && actual.GetType() != expected.GetType())
            {
                Actual += string.Format(CultureInfo.CurrentCulture, " ({0})", actual.GetType().FullName);
                Expected += string.Format(CultureInfo.CurrentCulture, " ({0})", expected.GetType().FullName);
            }
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// オブジェクトを文字列に変換する。
        /// </summary>
        /// <param name="object">オブジェクト。</param>
        /// <returns>文字列。</returns>
        private static string ConvertToString(object @object)
        {
            if (@object is string stringValue)
            {
                return stringValue;
            }

            var formattedValue = ArgumentFormatter.Format(@object);
            if (@object is IEnumerable)
            {
                formattedValue = string.Format("{0} {1}", ConvertToSimpleTypeName(@object.GetType().GetTypeInfo()), formattedValue);
            }

            return formattedValue;
        }

        /// <summary>
        /// 型情報をシンプルな型名に変換する。
        /// </summary>
        /// <param name="typeInfo">型情報。</param>
        /// <returns>シンプルな型名。</returns>
        private static string ConvertToSimpleTypeName(TypeInfo typeInfo)
        {
            if (!typeInfo.IsGenericType)
            {
                return typeInfo.Name;
            }

            var simpleNames = typeInfo.GenericTypeArguments.Select(type => ConvertToSimpleTypeName(type.GetTypeInfo()));
            var backTickIndex = typeInfo.Name.IndexOf('`');
            if (backTickIndex < 0)
            {
                backTickIndex = typeInfo.Name.Length;  // F# doesn't use backticks for generic type names
            }

            return string.Format("{0}<{1}>", typeInfo.Name.Substring(0, backTickIndex), string.Join(", ", simpleNames));
        }

        #endregion
    }
}