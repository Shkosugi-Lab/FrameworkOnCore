# yaf(YAF.NET 3.2.15)

掲示板です(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提で、コマンドは作業フォルダー(下の 0)で実行します。

**いまは変換までです。** 変換とビルドは通りますが、起動すると `/` が 500 になります(SqlClient のプロバイダーが見つからないため)。このため、動かす手順はありません。

| | |
|---|---|
| 取得元 | [YAFNET/YAFNET](https://github.com/YAFNET/YAFNET) タグ `v3.2.15` |
| ライセンス | Apache-2.0 |
| 追加で要るもの | なし |

## 0. FrameworkOnCore を取る

作業用の空のフォルダー(例: `C:\foc`。パスは短くします)を作って移り、そこに FrameworkOnCore を取ります。以降のコマンドは、すべてこのフォルダーで、ここからの相対パスで実行します。ほかのコーパスで済ませてあれば、この手順は飛ばします。

```powershell
New-Item -ItemType Directory -Force C:\foc | Out-Null
cd C:\foc
git clone https://github.com/Shkosugi-Lab/FrameworkOnCore.git
.\FrameworkOnCore\experiments\wf4c\casefs\build.ps1     # Docker Desktop を起動してから(Linux 用。一度だけ)
```

## 1. GitHub から取得する

```powershell
curl.exe -L -o yaf.zip https://github.com/YAFNET/YAFNET/archive/refs/tags/v3.2.15.zip
tar -xf yaf.zip          # YAFNET-3.2.15 ができる
```

## 2. 変換する

```powershell
.\FrameworkOnCore\converter.ps1 .\YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj --out .\out\yaf --root .\YAFNET-3.2.15 --build-original auto
```

**Studio で入れる値**([使い方](setup.md#42-studio-で))

| 欄 | 値 |
|---|---|
| プロジェクト | `.\YAFNET-3.2.15\yafsrc\YetAnotherForum.NET\YAF-SqlServer.csproj` |
| リポジトリのフォルダー | `.\YAFNET-3.2.15` |
| 構成 | `Release` |

変換の結果は `.\out\yaf\CONVERSION-REPORT.md`(Studio では変換レポート)にあります。未解決が 1 件、スタブ(コンパイルを通すための仮の実装)が 43 件です。

500 になるところを見るときは、Windows で起動します。

```powershell
Push-Location .\out\yaf\yafsrc\YetAnotherForum.NET
dotnet bin\YAF.dll --urls http://localhost:5000
Pop-Location                  # Ctrl+C で止めたあと、作業フォルダーに戻る
```
