# yaf(YAF.NET 3.2.15)

掲示板です(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

**いまは変換までです。** 変換とビルドは通りますが、起動すると `/` が 500 になります(SqlClient のプロバイダーが見つからないため)。このため、Linux で動かす手順はありません。

| | |
|---|---|
| 取得元 | [YAFNET/YAFNET](https://github.com/YAFNET/YAFNET) タグ `v3.2.15` |
| フォルダー | `corpora\work\YAFNET-3.2.15` |
| ライセンス | Apache-2.0 |
| 追加で要るもの | なし |

## GitHub から取得して変換する

```powershell
cd corpora\work
curl.exe -L -o yaf.zip https://github.com/YAFNET/YAFNET/archive/refs/tags/v3.2.15.zip
tar -xf yaf.zip          # YAFNET-3.2.15 ができる
cd ..\..
.\experiments\wf4c\convert-corpora.ps1 -Only yaf
```

変換の結果は `experiments\wf4c\yaf\CONVERSION-REPORT.md` にあります。未解決が 1 件、スタブ(コンパイルを通すための仮の実装)が 43 件です。
