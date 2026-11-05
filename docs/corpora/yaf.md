# yaf(YAF.NET 3.2.15)

掲示板です(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは `C:\foc` で実行します。

**いまは変換までです。** 変換とビルドは通りますが、起動すると `/` が 500 になります(SqlClient のプロバイダーが見つからないため)。このため、動かす手順はありません。

| | |
|---|---|
| 取得元 | [YAFNET/YAFNET](https://github.com/YAFNET/YAFNET) タグ `v3.2.15` |
| ライセンス | Apache-2.0 |
| 追加で要るもの | なし |

## 1. GitHub から取得する

```powershell
curl.exe -L -o yaf.zip https://github.com/YAFNET/YAFNET/archive/refs/tags/v3.2.15.zip
tar -xf yaf.zip          # YAFNET-3.2.15 ができる
```

## 2. 変換する

```powershell
C:\src\FrameworkOnCore\converter.ps1 C:\foc\YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj --out C:\foc\out\yaf --root C:\foc\YAFNET-3.2.15 --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `C:\foc\YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj` |
| リポジトリのフォルダー | `C:\foc\YAFNET-3.2.15` |
| 構成 | `Release` |

変換の結果は `C:\foc\out\yaf\CONVERSION-REPORT.md`(Studio では変換レポート)にあります。未解決が 1 件、スタブ(コンパイルを通すための仮の実装)が 43 件です。

500 になるところを見るときは、Windows で起動します。

```powershell
cd C:\foc\out\yaf\yafsrc\YetAnotherForum.NET
dotnet bin\YAF.dll --urls http://localhost:5000
```
