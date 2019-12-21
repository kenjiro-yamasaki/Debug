using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace SoftCube.Asserts.UnitTests
{
    using XAssert = Xunit.Assert;

    public class ArgumentFormatterTests
    {
        #region null

        [Fact]
        public static void Format_Null_正しくフォーマットする()
        {
            XAssert.Equal("null", ArgumentFormatter.Format(null));
        }

        #endregion

        #region char

        [Theory]
        [InlineData(' ', "' '")]
        [InlineData('a', "'a'")]
        [InlineData('1', "'1'")]
        [InlineData('!', "'!'")]
        [InlineData('\t', @"'\t'")] // tab
        [InlineData('\n', @"'\n'")] // newline
        [InlineData('\'', @"'\''")] // single quote
        [InlineData('\v', @"'\v'")] // vertical tab
        [InlineData('\a', @"'\a'")] // alert
        [InlineData('\\', @"'\\'")] // backslash
        [InlineData('\b', @"'\b'")] // backspace
        [InlineData('\r', @"'\r'")] // carriage return
        [InlineData('\f', @"'\f'")] // formfeed
        [InlineData('©', "'©'")]
        [InlineData('╬', "'╬'")]
        [InlineData('ئ', "'ئ'")]
        [InlineData('\uD800', "0xd800")]
        [InlineData('\uDC00', "0xdc00")]
        [InlineData('\uFFFD', "'\uFFFD'")]
        [InlineData('\uFFFE', "0xfffe")]
        [InlineData(char.MinValue, @"'\0'")]
        [InlineData(char.MaxValue, "0xffff")]
        public static void Format_Char_正しくフォーマットする(char value, string expected)
        {
            XAssert.Equal(expected, ArgumentFormatter.Format(value));
        }

        #endregion

        #region string

        [Theory]
        [InlineData("Hello, world!", "\"Hello, world!\"")]
        [InlineData(@"""", @"""\""""")] // quotes should be escaped
        [InlineData("\uD800\uDFFF", "\"\uD800\uDFFF\"")] // valid surrogates should print normally
        [InlineData("\uFFFE", @"""\xfffe""")] // same for U+FFFE...
        [InlineData("\uFFFF", @"""\xffff""")] // and U+FFFF, which are non-characters
        [InlineData("\u001F", @"""\x1f""")] // non-escaped C0 controls should be 2 digits
        [InlineData("\0", @"""\0""")] // null
        [InlineData("\r", @"""\r""")] // carriage return
        [InlineData("\n", @"""\n""")] // line feed
        [InlineData("\a", @"""\a""")] // alert
        [InlineData("\b", @"""\b""")] // backspace
        [InlineData("\\", @"""\\""")] // backslash
        [InlineData("\v", @"""\v""")] // vertical tab
        [InlineData("\t", @"""\t""")] // tab
        [InlineData("\f", @"""\f""")] // formfeed
        [InlineData("----|----1----|----2----|----3----|----4----|----5-", "\"----|----1----|----2----|----3----|----4----|----5\"...")] // truncation
        public static void Format_String_正しくフォーマットする(string value, string expected)
        {
            XAssert.Equal(expected, ArgumentFormatter.Format(value));
        }

        #endregion

        #region decimal

        [Fact]
        public static void Format_Decimal_正しくフォーマットする()
        {
            XAssert.Equal(123.45M.ToString(), ArgumentFormatter.Format(123.45M));
        }

        #endregion

        #region DateTime、DateTimeOffset

        [Fact]
        public static void Format_DateTimeValue_正しくフォーマットする()
        {
            var now = DateTime.UtcNow;

            XAssert.Equal(now.ToString("o"), ArgumentFormatter.Format(now));
        }

        [Fact]
        public static void Format_DateTimeOffset_正しくフォーマットする()
        {
            var now = DateTimeOffset.UtcNow;

            XAssert.Equal(now.ToString("o"), ArgumentFormatter.Format(now));
        }

        #endregion

        #region 列挙子

        public enum NonFlagsEnum
        {
            Value0 = 0,
            Value1 = 1
        }

        [Theory]
        [InlineData(0, "Value0")]
        [InlineData(1, "Value1")]
        [InlineData(42, "42")]
        public static void Format_Flags属性が付かない列挙子_正しくフォーマットする(int enumValue, string expected)
        {
            var actual = ArgumentFormatter.Format((NonFlagsEnum)enumValue);

            XAssert.Equal(expected, actual);
        }

        [Flags]
        public enum FlagsEnum
        {
            Nothing = 0,
            Value1 = 1,
            Value2 = 2,
        }

        [Theory]
        [InlineData(0, "Nothing")]
        [InlineData(1, "Value1")]
        [InlineData(3, "Value1 | Value2")]
        [InlineData(7, "7")]
        public static void Format_Flags属性が付く列挙子_正しくフォーマットする(int enumValue, string expected)
        {
            var actual = ArgumentFormatter.Format((FlagsEnum)enumValue);

            XAssert.Equal(expected, actual);
        }

        #endregion

        #region Type

        [Theory]
        [InlineData(typeof(int), "typeof(int)")]
        [InlineData(typeof(long), "typeof(long)")]
        [InlineData(typeof(string), "typeof(string)")]
        [InlineData(typeof(List<int>), "typeof(System.Collections.Generic.List<int>)")]
        [InlineData(typeof(Dictionary<int, string>), "typeof(System.Collections.Generic.Dictionary<int, string>)")]
        [InlineData(typeof(List<>), "typeof(System.Collections.Generic.List<>)")]
        [InlineData(typeof(Dictionary<,>), "typeof(System.Collections.Generic.Dictionary<,>)")]
        public void Format_Type_正しくフォーマットする(Type type, string expectedResult)
        {
            XAssert.Equal(expectedResult, ArgumentFormatter.Format(type));
        }

        [Fact]
        public void Format_型引数が1つのジェネリックの型引数_正しくフォーマットする()
        {
            var typeInfo              = typeof(List<>).GetTypeInfo();
            var genericTypeParameters = typeInfo.GenericTypeParameters;
            var parameterType         = genericTypeParameters.First();

            XAssert.Equal("typeof(T)", ArgumentFormatter.Format(parameterType));
        }

        [Fact]
        public void Format_型引数が2つのジェネリックの型引数_正しくフォーマットする()
        {
            var typeInfo              = typeof(Dictionary<,>).GetTypeInfo();
            var genericTypeParameters = typeInfo.GenericTypeParameters;
            var parameterTKey         = genericTypeParameters.First();
            var parameterTValue       = genericTypeParameters.Last();

            XAssert.Equal("typeof(TKey)", ArgumentFormatter.Format(parameterTKey));
            XAssert.Equal("typeof(TValue)", ArgumentFormatter.Format(parameterTValue));
        }

        #endregion

        #region Task

        [Fact]
        public static async void Format_Task_正しくフォーマットする()
        {
            var task = Task.Run(() => { });
            await task;

            XAssert.Equal("Task { Status = RanToCompletion }", ArgumentFormatter.Format(task));
        }

        [Fact]
        public static void Format_GenericTask_正しくフォーマットする()
        {
            var source = new TaskCompletionSource<int>();
            source.SetException(new DivideByZeroException());

            XAssert.Equal("Task<int> { Status = Faulted }", ArgumentFormatter.Format(source.Task));
        }

        #endregion

        #region IEnumerable

        [Fact]
        public static void Format_IEnumerable_正しくフォーマットする()
        {
            var expected = $"[1, {2.3M}, \"Hello, world!\"]";

            XAssert.Equal(expected, ArgumentFormatter.Format(new object[] { 1, 2.3M, "Hello, world!" }));
        }

        [Fact]
        public static void Format_項目の多いIEnumerable_最初の数個の項目だけをフォーマットする()
        {
            XAssert.Equal("[0, 1, 2, 3, 4, ...]", ArgumentFormatter.Format(Enumerable.Range(0, int.MaxValue)));
        }

        [Fact]
        public static void Format_再帰するIEnumerable_最大ネスト深さでフォーマットを打ち切る()
        {
            var looping = new object[2];
            looping[0] = 42;
            looping[1] = looping;

            XAssert.Equal("[42, [42, [...]]]", ArgumentFormatter.Format(looping));
        }

        #endregion

        #region クラス

        #region シンプルなクラス

        public class SimpleClass
        {
            #pragma warning disable 414
            private string PrivateField = "Hello, world";
            #pragma warning restore 414
            public static int PublicStaticField = 2112;
            public decimal PublicProperty { get; private set; }
            public int PublicField = 42;

            public SimpleClass()
            {
                PublicProperty = 21.12M;
            }
        }

        [Fact]
        public static void Format_シンプルなクラス_正しくフォーマットする()
        {
            var expected = $"SimpleClass {{ PublicField = 42, PublicProperty = {21.12M} }}";

            XAssert.Equal(expected, ArgumentFormatter.Format(new SimpleClass()));
        }

        #endregion

        #region クラスを所有するクラス

        public class ClassWithSimpleClass
        {
            public SimpleClass SimpleClass = new SimpleClass();
            public char Char = 'A';
            public string String = "Hello, world!";
        }

        [Fact]
        public static void Format_クラスを所有するクラス_正しくフォーマットする()
        {
            var expected = $"ClassWithSimpleClass {{ Char = 'A', SimpleClass = SimpleClass {{ PublicField = 42, PublicProperty = {21.12M} }}, String = \"Hello, world!\" }}";

            XAssert.Equal(expected, ArgumentFormatter.Format(new ClassWithSimpleClass()));
        }

        #endregion

        #region 大きなクラス

        public class BigClass
        {
            public string Field2 = "Hello, world!";
            public decimal Property1 { get; set; }
            public object Property4 { get; set; }
            public object Property3 { get; set; }
            public int Field1 = 42;
            public Type Property2 { get; set; }

            public BigClass()
            {
                Property1 = 21.12M;
                Property2 = typeof(BigClass);
                Property3 = new DateTimeOffset(2014, 04, 17, 07, 45, 23, TimeSpan.Zero);
                Property4 = "Should not be shown";
            }
        }

        [Fact]
        public static void Format_大きなクラス_最大プロパティフィールド数でフォーマットを打ち切る()
        {
            var expected = $@"BigClass {{ Field1 = 42, Field2 = ""Hello, world!"", Property1 = {21.12}, Property2 = typeof(SoftCube.Asserts.UnitTests.ArgumentFormatterTests+BigClass), Property3 = 2014-04-17T07:45:23.0000000+00:00, ... }}";

            XAssert.Equal(expected, ArgumentFormatter.Format(new BigClass()));
        }

        #endregion

        #region ループするクラス

        public class LoopingClass
        {
            public LoopingClass Me;
            public LoopingClass() => Me = this;
        }

        [Fact]
        public static void Format_ループするクラス_最大ネスト深さでフォーマットを打ち切る()
        {
            XAssert.Equal("LoopingClass { Me = LoopingClass { Me = LoopingClass { ... } } }", ArgumentFormatter.Format(new LoopingClass()));
        }

        #endregion

        #region ToStringをオーバーライドするクラス

        public class ClassWithToString
        {
            public override string ToString() => "This is what you should show";
        }

        [Fact]
        public static void Format_ToStringをオーバーライドするクラス_ToStringを戻す()
        {
            XAssert.Equal("This is what you should show", ArgumentFormatter.Format(new ClassWithToString()));
        }

        #endregion

        #region 例外を投げるgetプロパティを所有するクラス

        public class ClassWithThrowingGetter
        {
            public string MyThrowingProperty { get { throw new NotImplementedException(); } }
        }

        [Fact]
        public static void Format_例外を投げるgetプロパティを所有するクラス_正しくフォーマットする()
        {
            XAssert.Equal("ClassWithThrowingGetter { MyThrowingProperty = (throws NotImplementedException) }", ArgumentFormatter.Format(new ClassWithThrowingGetter()));
        }

        #endregion

        [Fact]
        public static void Format_空のクラス_正しくフォーマットする()
        {
            XAssert.Equal("Object { }", ArgumentFormatter.Format(new object()));
        }

        #endregion
    }
}
