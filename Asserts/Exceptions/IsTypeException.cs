namespace SoftCube.Asserts
{
    /// <summary>
    /// IsType �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.IsType"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class IsTypeException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expectedTypeName">���Ғl (�^��)�B</param>
        /// <param name="actualTypeName">�����l (�^��)�B</param>
        public IsTypeException(string expectedTypeName, string actualTypeName)
            : base(expectedTypeName, actualTypeName, "Assert.IsType() Failure")
        {
        }

        #endregion
    }
}