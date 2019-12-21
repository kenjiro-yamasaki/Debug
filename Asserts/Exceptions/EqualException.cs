using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Equal アサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、<see cref="Assert.Equal"/> の失敗時に投げられます。
    /// </remarks>
    public class EqualException : AssertExpectedActualException
    {
        #region 静的フィールド

        /// <summary>
        /// エスケープ文字→エンコードエスケープ文字の変換。
        /// </summary>
        private static readonly Dictionary<char, string> Encodings = new Dictionary<char, string>
        {
            { '\r', "\\r" },
            { '\n', "\\n" },
            { '\t', "\\t" },
            { '\0', "\\0" }
        };

        #endregion

        #region プロパティ

        /// <summary>
        /// 期待値の差分インデックス。
        /// 差分インデックスが提供されない場合、-1。
        /// </summary>
        /// <remarks>
        /// 期待値文字列と実測値文字列を比較した差分の先頭を示します。
        /// </remarks>
        public int ExpectedIndex { get; private set; }

        /// <summary>
        /// 実測値の差分インデックス。
        /// 差分インデックスが提供されない場合、-1。
        /// </summary>
        /// <remarks>
        /// 期待値文字列と実測値文字列を比較した差分の先頭を示します。
        /// </remarks>
        public int ActualIndex { get; private set; }

        /// <summary>
        /// メッセージ。
        /// </summary>
        public override string Message
        {
            get
            {
                if (message == null)
                {
                    message = CreateMessage();
                }

                return message;
            }
        }
        private string message;

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public EqualException(object expected, object actual)
            : base(expected, actual, "Assert.Equal() Failure")
        {
            ActualIndex   = -1;
            ExpectedIndex = -1;
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        /// <param name="expectedIndex">期待値の差分インデックス。</param>
        /// <param name="actualIndex">実測値の差分インデックス。</param>
        public EqualException(string expected, string actual, int expectedIndex, int actualIndex)
            : base(expected, actual, "Assert.Equal() Failure")
        {
            ActualIndex   = actualIndex;
            ExpectedIndex = expectedIndex;
        }

        #endregion

        #region 静的メソッド

        /// <summary>
        /// 値とポインターの表記文字列を取得します。
        /// </summary>
        /// <param name="value">値。</param>
        /// <param name="pointerIndex">ポインターインデックス。</param>
        /// <param name="pointer">ポインター。</param>
        /// <returns>値の表記文字列, ポインターの表記文字列。</returns>
        private static (string Value, string Pointer) GetPrintedValueAndPointer(string value, int pointerIndex, char pointer)
        {
            int start          = Math.Max(pointerIndex - 20, 0);
            int end            = Math.Min(pointerIndex + 41, value.Length);
            var printedValue   = new StringBuilder(100);
            var printedPointer = new StringBuilder(100);

            if (0 < start)
            {
                printedValue.Append("···");
                printedPointer.Append("   ");
            }

            for (int index = start; index < end; ++index)
            {
                char c = value[index];

                // 値の表記文字列を追加します。
                int paddingLength = 1;
                {
                    string encoding;
                    if (Encodings.TryGetValue(c, out encoding))
                    {
                        printedValue.Append(encoding);
                        paddingLength = encoding.Length;
                    }
                    else
                    {
                        printedValue.Append(c);
                    }
                }

                // ポインターの表記文字列を追加します。
                if (index < pointerIndex)
                {
                    printedPointer.Append(' ', paddingLength);
                }
                else if (index == pointerIndex)
                {
                    printedPointer.AppendFormat("{0} (pos {1})", pointer, pointerIndex);
                }
            }

            if (value.Length == pointerIndex)
            {
                printedPointer.AppendFormat("{0} (pos {1})", pointer, pointerIndex);
            }
            if (end < value.Length)
            {
                printedValue.Append("···");
            }

            return (printedValue.ToString(), printedPointer.ToString());
        }

        #endregion

        #region メソッド

        /// <summary>
        /// メッセージを生成します。
        /// </summary>
        /// <returns>メッセージ。</returns>
        private string CreateMessage()
        {
            if (ExpectedIndex == -1)
            {
                return base.Message;
            }

            var printedExpected = GetPrintedValueAndPointer(Expected, ExpectedIndex, '↓');
            var printedActual   = GetPrintedValueAndPointer(Actual, ActualIndex, '↑');

            return string.Format(
                CultureInfo.CurrentCulture,
                "{1}{0}          {2}{0}Expected: {3}{0}Actual:   {4}{0}          {5}",
                Environment.NewLine,
                UserMessage,
                printedExpected.Pointer,
                printedExpected.Value ?? "(null)",
                printedActual.Value ?? "(null)",
                printedActual.Pointer);
        }

        #endregion
    }
}