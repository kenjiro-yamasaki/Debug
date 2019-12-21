using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.InRange"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.InRange"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class InRangeException : AssertException
    {
        #region �v���W�F�N�g

        /// <summary>
        /// �����l�B
        /// </summary>
        public string Actual { get; }

        /// <summary>
        /// ����l�B
        /// </summary>
        public string High { get; }

        /// <summary>
        /// �����l�B
        /// </summary>
        public string Low { get; }

        /// <summary>
        /// ���b�Z�[�W�B
        /// </summary>
        public override string Message => string.Format(CultureInfo.CurrentCulture, "{0}\r\nRange:  ({1} - {2})\r\nActual: {3}", base.Message, Low, High, Actual ?? "(null)");

        #endregion

        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="actual">�����l�B</param>
        /// <param name="low">�����l�B</param>
        /// <param name="high">����l�B</param>
        public InRangeException(object actual, object low, object high)
            : base("Assert.InRange() Failure")
        {
            Actual = actual == null ? null : actual.ToString();
            Low    = low == null ? null : low.ToString();
            High   = high == null ? null : high.ToString();
        }

        #endregion
    }
}
