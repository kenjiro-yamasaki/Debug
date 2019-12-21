namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.DoesNotContain"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.DoesNotContain"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class DoesNotContainException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public DoesNotContainException(object expected, object actual)
            : base(expected, actual, "Assert.DoesNotContain() Failure", "Found", "In value")
        {
        }

        #endregion
    }
}