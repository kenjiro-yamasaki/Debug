namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.Null"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Null"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class NullException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="actual">�����l�B</param>
        public NullException(object actual)
            : base(null, actual, "Assert.Null() Failure")
        {
        }

        #endregion
    }
}