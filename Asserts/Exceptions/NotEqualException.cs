namespace SoftCube.Asserts
{
    /// <summary>
    /// NotEqual �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.NotEqual"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class NotEqualException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public NotEqualException(string expected, string actual)
            : base("Not " + expected, actual, "Assert.NotEqual() Failure")
        {
        }

        #endregion
    }
}
