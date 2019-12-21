using System;
using System.Collections;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Empty �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Empty"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class EmptyException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="collection">���؂Ɏ��s�����R���N�V�����B</param>
        public EmptyException(IEnumerable collection)
            : base("<empty>", ArgumentFormatter.Format(collection), "Assert.Empty() Failure")
        {
        }

        #endregion
    }
}
