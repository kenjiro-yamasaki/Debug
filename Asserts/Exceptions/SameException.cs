namespace SoftCube.Asserts
{
    /// <summary>
    /// Same �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Same"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class SameException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public SameException(object expected, object actual)
            : base(expected, actual, "Assert.Same() Failure")
        {
        }

        #endregion
    }
}
