using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// ProperSuperset �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.ProperSuperset"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class ProperSupersetException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public ProperSupersetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.ProperSuperset() Failure")
        {
        }

        #endregion
    }
}
