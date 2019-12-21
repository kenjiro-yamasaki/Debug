using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Superset �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Superset"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class SupersetException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl�B</param>
        /// <param name="actual">�����l�B</param>
        public SupersetException(IEnumerable expected, IEnumerable actual)
            : base(expected, actual, "Assert.Superset() Failure")
        {
        }

        #endregion
    }
}