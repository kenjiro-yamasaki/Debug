namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.True"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.True"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class TrueException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="acutual">�����l�B</param>
        /// <param name="message">���b�Z�[�W�B</param>
        public TrueException(bool? acutual, string message)
            : base("True", acutual == null ? "(null)" : acutual.ToString(), message ?? "Assert.True() Failure")
        {
        }

        #endregion
    }
}