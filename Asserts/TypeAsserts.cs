using System;
using System.Reflection;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public partial class Assert
    {
        #region 静的メソッド

        /// <summary>
        /// 指定オブジェクトが指定型に代入可能かを検証します。
        /// </summary>
        /// <typeparam name="TExpected">指定型。</typeparam>
        /// <param name="object">指定オブジェクト。</param>
        /// <returns>(検証に成功した場合) 指定型にキャストした指定オブジェクト。</returns>
        /// <exception cref="IsAssignableFromException">指定オブジェクトが指定型に代入可能ではない場合、投げられます。</exception>
        public static TExpected IsAssignableFrom<TExpected>(object @object)
        {
            IsAssignableFrom(typeof(TExpected), @object);
            return (TExpected)@object;
        }

        /// <summary>
        /// 指定オブジェクトが指定型かを検証します。
        /// </summary>
        /// <param name="expectedType">指定型。</param>
        /// <param name="object">指定オブジェクト。</param>
        /// <exception cref="IsAssignableFromException">指定オブジェクトが指定型に代入可能ではない場合、投げられます。</exception>
        public static void IsAssignableFrom(Type expectedType, object @object)
        {
            GuardArgumentNotNull(nameof(expectedType), expectedType);

            if (@object == null || !expectedType.GetTypeInfo().IsAssignableFrom(@object.GetType().GetTypeInfo()))
            {
                throw new IsAssignableFromException(expectedType, @object);
            }
        }

        /// <summary>
        /// 指定オブジェクトが (派生型ではなく) 正確に指定型かを検証します。
        /// </summary>
        /// <typeparam name="TExpected">指定型。</typeparam>
        /// <param name="object">指定オブジェクト。</param>
        /// <returns>(検証に成功した場合) 指定型にキャストした指定オブジェクト。</returns>
        /// <exception cref="IsTypeException">指定オブジェクトが (派生型ではなく) 正確に指定型ではない場合、投げられます。</exception>
        public static TExpected IsType<TExpected>(object @object)
        {
            IsType(typeof(TExpected), @object);
            return (TExpected)@object;
        }

        /// <summary>
        /// 指定オブジェクトが (派生型ではなく) 正確に指定型かを検証します。
        /// </summary>
        /// <param name="expectedType">指定型。</param>
        /// <param name="object">指定オブジェクト。</param>
        /// <exception cref="IsTypeException">指定オブジェクトが (派生型ではなく) 正確に指定型ではない場合、投げられます。</exception>
        public static void IsType(Type expectedType, object @object)
        {
            GuardArgumentNotNull(nameof(expectedType), expectedType);

            if (@object == null)
            {
                throw new IsTypeException(expectedType.FullName, null);
            }

            var actualType = @object.GetType();
            if (expectedType != actualType)
            {
                var expectedTypeName = expectedType.FullName;
                var actualTypeName   = actualType.FullName;

                if (expectedTypeName == actualTypeName)
                {
                    expectedTypeName += string.Format(" ({0})", expectedType.GetTypeInfo().Assembly.GetName().FullName);
                    actualTypeName   += string.Format(" ({0})", actualType.GetTypeInfo().Assembly.GetName().FullName);
                }

                throw new IsTypeException(expectedTypeName, actualTypeName);
            }
        }

        /// <summary>
        /// 指定オブジェクトが正確に指定型ではないかを検証します。
        /// </summary>
        /// <typeparam name="TExpected">指定型。</typeparam>
        /// <param name="object">指定オブジェクト。</param>
        /// <exception cref="IsNotTypeException">指定オブジェクトが正確に指定型である場合、投げられます。</exception>
        public static void IsNotType<TExpected>(object @object)
        {
            IsNotType(typeof(TExpected), @object);
        }

        /// <summary>
        /// 指定オブジェクトが正確に指定型ではないかを検証します。
        /// </summary>
        /// <param name="expectedType">指定型。</param>
        /// <param name="object">指定オブジェクト。</param>
        /// <exception cref="IsNotTypeException">指定オブジェクトが正確に指定型である場合、投げられます。</exception>
        public static void IsNotType(Type expectedType, object @object)
        {
            GuardArgumentNotNull(nameof(expectedType), expectedType);

            if (@object != null && expectedType.Equals(@object.GetType()))
            {
                throw new IsNotTypeException(expectedType, @object);
            }
        }

        #endregion
    }
}