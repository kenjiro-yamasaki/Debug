namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.False"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.False"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class FalseException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="acutual">�����l�B</param>
        /// <param name="message">���b�Z�[�W�B</param>
        public FalseException(bool? acutual, string message)
            : base("False", acutual == null ? "(null)" : acutual.ToString(), message ?? "Assert.False() Failure")
        {
        }

        #endregion
    }
}