using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.Subset"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Subset"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class SubsetException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public SubsetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.Subset() Failure")
        {
        }

        #endregion
    }
}