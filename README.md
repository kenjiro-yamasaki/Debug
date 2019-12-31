## Debugs
**Debugs** は、その名前の通り、以下のデバッグに関する機能を提供します。  
* Assert
* Log
* Profile

## Overview
* [マイクロソフト Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/) に準拠。
* 400+ の単体テスト。
* 使用方法を示す最小のサンプル。


## Assert
「起こるはずがない」と思っていることがあれば、それをチェックするコードを追加してください。
Assert を用いるのが最も簡単な方法です。**Debugs** は、さまざまな Assert メソッドを提供します。
* 論理検証
* 等値検証
* 同一インスタンス検証
* 範囲の検証
* null の検証
* 文字列の検証
* その他

```C#
using SoftCube.Asserts;
...

void Log(string message)
{
    Assert.NotNull(message);
    ...
}
```



## Log  
## Profile
