using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// ProperSubset �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.ProperSubset"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class ProperSubsetException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public ProperSubsetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.ProperSubset() Failure")
        {
        }

        #endregion
    }
}
