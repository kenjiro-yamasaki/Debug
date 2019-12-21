namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.Contains"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Contains"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class ContainsException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public ContainsException(object expected, object actual)
            : base(expected, actual, "Assert.Contains() Failure", "Not found", "In value")
        {
        }

        #endregion
    }
}