## Debugs
デバッグは、その名前の通り、以下の機能を提供します。  
Debugs, as the name implies, provides the following debugging features:  
* Assert
* Log
* Profile

## Overview
* 400+ の単体テスト。
* 使用方法を示す最小のサンプル。


## Assert
さまざまな Assert メソッドを提供します。
* 論理検証
  * Assert.True(condition)
  * Assert.False(condition)
* 等値検証
  * Assert.Equal(expected, actual)
  * Assert.Equal(expected, actual, precision)
  * Assert.NotEqual(expected, actual)
  * Assert.NotEqual(expected, actual, precision)
* 同一インスタンス検証
  * Assert.Same(expected, actual)
  * Assert.NotSame(expected, actual)
* 範囲の検証
  * Assert.InRange(actual, low, hight)
  * Assert.NotInRange(actual, low, hight)
* null の検証
  * Assert.Null(object)
  * Assert.NotNull(object)
* 文字列の検証
  * Assert.Contains(expectedSubString, actualString)
  * Assert.StartsWith(expectedStartString, actualString)
  * Assert.EndsWith(expectedEndString, actualString)
  * Assert.Matches(expectedRegex, actualString)
* その他
  
## Log  
## Profile
