using System;

namespace SoftCube.Asserts
{
    /// <summary>
    /// �A�T�[�g��O�B
    /// </summary>
    public class AssertException : Exception
    {
        #region �v���p�e�B

        /// <summary>
        /// ���[�U�[���b�Z�[�W�B
        /// </summary>
        public string UserMessage { get; }

        #endregion

        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        public AssertException()
        {
        }

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="userMessage">���[�U�[���b�Z�[�W�B</param>
        public AssertException(string userMessage)
            : this(userMessage, null)
        {
        }

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="userMessage">���[�U�[���b�Z�[�W�B</param>
        /// <param name="innerException">������O�B</param>
        protected AssertException(string userMessage, Exception innerException)
            : base(userMessage, innerException)
        {
            UserMessage = userMessage;
        }

        #endregion
    }
}