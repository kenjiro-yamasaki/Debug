namespace SoftCube.Asserts
{
    /// <summary>
    /// NotNull �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.NotNull"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class NotNullException : AssertException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        public NotNullException()
            : base("Assert.NotNull() Failure")
        {
        }

        #endregion
    }
}