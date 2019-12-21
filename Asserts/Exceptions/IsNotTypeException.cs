using System;

namespace SoftCube.Asserts
{
    /// <summary>
    /// <see cref="Assert.IsNotType"/> �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.IsNotType"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class IsNotTypeException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expectedTypeName">���Ғl (�^)�B</param>
        /// <param name="actualTypeName">�����l�B</param>
        public IsNotTypeException(Type expected, object actual)
            : base(expected, actual == null ? null : actual.GetType(), "Assert.IsNotType() Failure")
        {
        }

        #endregion
    }
}
