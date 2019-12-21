using System;
using System.Globalization;

namespace SoftCube.Asserts
{
    /// <summary>
    /// Matches �A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́A<see cref="Assert.Matches"/> �̎��s���ɓ������܂��B
    /// </remarks>
    public class MatchesException : AssertException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expectedRegexPattern">���Ғl (���K�\��)�B</param>
        /// <param name="actual">�����l�B</param>
        public MatchesException(object expectedRegexPattern, object actual)
            : base(string.Format(CultureInfo.CurrentCulture, "Assert.Matches() Failure:{2}Regex: {0}{2}Value: {1}", expectedRegexPattern, actual, Environment.NewLine))
        {
        }

        #endregion
    }
}