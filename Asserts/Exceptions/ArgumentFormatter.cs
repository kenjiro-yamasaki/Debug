using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoftCube.Asserts
{
    /// <summary>
    /// 引数フォーマッター。
    /// </summary>
    public static class ArgumentFormatter
    {
        #region 定数

        /// <summary>
        /// 最大深さ。
        /// </summary>
        private const int MaxDepth = 3;

        /// <summary>
        /// 最大列挙長さ。
        /// </summary>
        private const int MaxEnumerableLength = 5;

        /// <summary>
        /// 最大プロパティ・フィールド数。
        /// </summary>
        private const int MaxPropertyFieldCount = 5;

        /// <summary>
        /// 最大文字列長さ。
        /// </summary>
        private const int MaxStringLength = 50;

        /// <summary>
        /// 空の <see cref="object"/> 配列。
        /// </summary>
        private static readonly object[] EmptyObjects = new object[0];

        /// <summary>
        /// 空の <see cref="Type"/> 配列。
        /// </summary>
        private static readonly Type[] EmptyTypes = new Type[0];

        /// <summary>
        /// 型情報→文字列変換。
        /// </summary>
        private static readonly Dictionary<TypeInfo, string> TypeInfoToString = new Dictionary<TypeInfo, string>
        {
            { typeof(bool).GetTypeInfo(), "bool" },
            { typeof(byte).GetTypeInfo(), "byte" },
            { typeof(sbyte).GetTypeInfo(), "sbyte" },
            { typeof(char).GetTypeInfo(), "char" },
            { typeof(decimal).GetTypeInfo(), "decimal" },
            { typeof(double).GetTypeInfo(), "double" },
            { typeof(float).GetTypeInfo(), "float" },
            { typeof(int).GetTypeInfo(), "int" },
            { typeof(uint).GetTypeInfo(), "uint" },
            { typeof(long).GetTypeInfo(), "long" },
            { typeof(ulong).GetTypeInfo(), "ulong" },
            { typeof(object).GetTypeInfo(), "object" },
            { typeof(short).GetTypeInfo(), "short" },
            { typeof(ushort).GetTypeInfo(), "ushort" },
            { typeof(string).GetTypeInfo(), "string" },
        };

        #endregion

        /// <summary>
        /// 値をフォーマットします。
        /// </summary>
        /// <param name="value">値。</param>
        /// <returns>フォーマットされた値。</returns>
        public static string Format(object value)
        {
            return Format(value, 1);
        }

        /// <summary>
        /// 値をフォーマットします。
        /// </summary>
        /// <param name="value">値。</param>
        /// <param name="depth">ネストの深さ。</param>
        /// <returns>フォーマットされた値。</returns>
        private static string Format(object value, int depth)
        {
            if (value == null)
            {
                return "null";
            }

            var type     = value.GetType();
            var typeInfo = type.GetTypeInfo();

            try
            {
                switch (value)
                {
                    case char @char:
                        if (@char == '\'')
                        {
                            return @"'\''";
                        }

                        var (success, escapeSequence) = TryGetEscapeSequence(@char);
                        if (success)
                        {
                            return $"'{escapeSequence}'";
                        }
                        if (char.IsLetterOrDigit(@char) || char.IsPunctuation(@char) || char.IsSymbol(@char) || @char == ' ')
                        {
                            return $"'{@char}'";
                        }
                        return $"0x{(int)@char:x4}";

                    case DateTime _:
                    case DateTimeOffset _:
                        return $"{value:o}";

                    case string @string:
                        return FormatString(@string);

                    case Task task:
                        var typeArguments = typeInfo.GenericTypeArguments;
                        var typeName = typeArguments.Length == 0 ? "Task" : $"Task<{string.Join(",", typeArguments.Select(FormatTypeName))}>";
                        return $"{typeName} {{ Status = {task.Status} }}";

                    case Type typeValue:
                        return $"typeof({FormatTypeName(typeValue)})";

                    case IEnumerable enumerable:
                        return FormatEnumerable(enumerable.Cast<object>(), depth);

                    default:
                        if (typeInfo.IsEnum)
                        {
                            return value.ToString().Replace(", ", " | ");
                        }

                        // ToStringをオーバーライドしている場合、ToStringを戻す。
                        var toString = type.GetRuntimeMethod("ToString", EmptyTypes);
                        if (toString != null && toString.DeclaringType != typeof(object))
                        {
                            return (string)toString.Invoke(value, EmptyObjects);
                        }

                        if (typeInfo.IsValueType)
                        {
                            return Convert.ToString(value, CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            return FormatReferenceValue(value, depth, type);
                        }
                }
            }
            catch (Exception ex)
            {
                // 値のフォーマット処理が例外を発生させる場合がある(例えば、ToStringが例外を発生させる場合など)。
                // このような場合、プログラムを停止させないために例外をキャッチします。
                return $"{ex.GetType().Name} was thrown formatting an object of type \"{type}\"";
            }
        }

        /// <summary>
        /// 文字列の値をフォーマットします。
        /// </summary>
        /// <param name="string">文字列の値。</param>
        /// <returns>フォーマットされた文字列の値。</returns>
        private static string FormatString(string @string)
        {
            var builder = new StringBuilder(@string.Length);
            for (int index = 0; index < @string.Length; index++)
            {
                var @char = @string[index];
                var (success, escapeSequence) = TryGetEscapeSequence(@char);
                if (success)
                {
                    builder.Append(escapeSequence);
                }
                else if (@char < 32)
                {
                    builder.AppendFormat(@"\x{0}", (+@char).ToString("x2"));
                }
                else if (char.IsSurrogatePair(@string, index))
                {
                    builder.Append(@char);
                    builder.Append(@string[++index]);
                }
                else if (char.IsSurrogate(@char) || @char == '\uFFFE' || @char == '\uFFFF')
                {
                    builder.AppendFormat(@"\x{0}", (+@char).ToString("x4"));
                }
                else
                {
                    builder.Append(@char);
                }
            }

            @string = builder.ToString();
            @string = @string.Replace(@"""", @"\""");
            if (MaxStringLength < @string.Length)
            {
                string displayed = @string.Substring(0, MaxStringLength);
                return $"\"{displayed}\"...";
            }
            return $"\"{@string}\"";
        }

        /// <summary>
        /// フォーマットされた値をラップし、取得します。
        /// </summary>
        /// <param name="getter">値を取得するデリゲート。</param>
        /// <param name="depth">ネスト深さ。</param>
        /// <returns>ラップし、フォーマットされた値。</returns>
        private static string WrapAndGetFormattedValue(Func<object> getter, int depth)
        {
            try
            {
                return Format(getter(), depth + 1);
            }
            catch (Exception ex)
            {
                while (true)
                {
                    if (ex is TargetInvocationException tiex)
                    {
                        ex = tiex.InnerException;
                    }
                    else
                    {
                        break;
                    }
                }
                return $"(throws {ex.GetType().Name})";
            }
        }

        /// <summary>
        /// 参照型の値をフォーマットします。
        /// </summary>
        /// <param name="value">参照型の値。</param>
        /// <param name="depth">ネストの深さ。</param>
        /// <param name="type">参照型。</param>
        /// <returns>フォーマットされた参照型の値。</returns>
        private static string FormatReferenceValue(object value, int depth, Type type)
        {
            if (depth == MaxDepth)
            {
                return $"{type.Name} {{ ... }}";
            }

            var fields     = type.GetRuntimeFields().Where(f => f.IsPublic && !f.IsStatic).Select(f => new { Name = f.Name, Value = WrapAndGetFormattedValue(() => f.GetValue(value), depth) });
            var properties = type.GetRuntimeProperties().Where(p => p.GetMethod != null && p.GetMethod.IsPublic && !p.GetMethod.IsStatic).Select(p => new { Name = p.Name, Value = WrapAndGetFormattedValue(() => p.GetValue(value), depth) });
            var parameters = fields.Concat(properties).OrderBy(p => p.Name).Take(MaxPropertyFieldCount + 1).ToList();

            if (parameters.Any())
            {
                var formattedParameters = string.Join(", ", parameters.Take(MaxPropertyFieldCount).Select(p => $"{p.Name} = {p.Value}"));
                if (parameters.Count > MaxPropertyFieldCount)
                {
                    formattedParameters += ", ...";
                }
                return $"{type.Name} {{ {formattedParameters} }}";
            }
            else
            {
                return $"{type.Name} {{ }}";
            }
        }

        /// <summary>
        /// 反復型の値 (コレクション) をフォーマットします。
        /// </summary>
        /// <param name="values">反復型の値 (コレクション)。</param>
        /// <param name="depth">ネストの深さ。</param>
        /// <returns>フォーマットされた反復型の値 (コレクション)。</returns>
        private static string FormatEnumerable(IEnumerable<object> values, int depth)
        {
            if (depth == MaxDepth)
            {
                return "[...]";
            }

            var omittedValues   = values.Take(MaxEnumerableLength + 1).ToList();
            var formattedValues = string.Join(", ", omittedValues.Take(MaxEnumerableLength).Select(x => Format(x, depth + 1)));

            if (MaxEnumerableLength < omittedValues.Count)
            {
                formattedValues += ", ...";
                return $"[{formattedValues}]";
            }
            else
            {
                return $"[{formattedValues}]";
            }
        }

        /// <summary>
        /// 型名をフォーマットします。
        /// </summary>
        /// <param name="type">型。</param>
        /// <returns>フォーマットされた型。</returns>
        private static string FormatTypeName(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            // Deconstruct and re-construct array
            var arraySuffix = "";
            while (typeInfo.IsArray)
            {
                var rank = typeInfo.GetArrayRank();
                arraySuffix += $"[{new string(',', rank - 1)}]";
                typeInfo = typeInfo.GetElementType().GetTypeInfo();
            }

            // Map C# built-in type names
            string result;
            if (TypeInfoToString.TryGetValue(typeInfo, out result))
            {
                return result + arraySuffix;
            }

            // Strip off generic suffix
            // catch special case of generic parameters not being bound to a specific type:
            var name = typeInfo.FullName;
            if (name == null)
            {
                return typeInfo.Name;
            }

            var tickIndex = name.IndexOf('`');
            if (0 < tickIndex)
            {
                name = name.Substring(0, tickIndex);
            }
            if (typeInfo.IsGenericTypeDefinition)
            {
                name = $"{name}<{new string(',', typeInfo.GenericTypeParameters.Length - 1)}>";
            }
            else if (typeInfo.IsGenericType)
            {
                if (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    name = FormatTypeName(typeInfo.GenericTypeArguments[0]) + "?";
                }
                else
                {
                    name = $"{name}<{string.Join(", ", typeInfo.GenericTypeArguments.Select(FormatTypeName))}>";
                }
            }
            return name + arraySuffix;
        }

        /// <summary>
        /// エスケープ文字の変換を試みます。
        /// </summary>
        /// <param name="char">文字。</param>
        /// <returns>エスケープ文字かを示す値, エスケープ文字の変換文字列。</returns>
        private static (bool Success, string Value) TryGetEscapeSequence(char @char)
        {
            switch (@char)
            {
                case '\t':
                    return (true, @"\t");

                case '\n':
                    return (true, @"\n");

                case '\v':
                    return (true, @"\v");

                case '\a':
                    return (true, @"\a");

                case '\r':
                    return (true, @"\r");

                case '\f':
                    return (true, @"\f");

                case '\b':
                    return (true, @"\b");

                case '\0':
                    return (true, @"\0");

                case '\\':
                    return (true, @"\\");

                default:
                    return (false, null);
            }
        }
    }
}
