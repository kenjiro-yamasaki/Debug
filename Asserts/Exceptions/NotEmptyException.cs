namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.NotEmpty"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.NotEmpty"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class NotEmptyException : AssertException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        public NotEmptyException()
            : base("Assert.NotEmpty() Failure")
        {
        }

        #endregion
    }
}
