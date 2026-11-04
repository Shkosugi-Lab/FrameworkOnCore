# mvcmovie(MvcMovie)

映画の一覧を扱う ASP.NET MVC 5 のチュートリアルです(C#、EF6、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [dotnet/AspNetDocs](https://github.com/dotnet/AspNetDocs) コミット `552c58dd` の `aspnet/mvc/overview/getting-started/introduction/sample/MvcMovie` |
| フォルダー | `corpora\work\MvcMovie` |
| ライセンス | コードは MIT、文書は CC BY 4.0 |
| 追加で要るもの | なし(SQL Server はコンテナで動かします) |

## 1. 取得して変換する

```powershell
.\corpora\fetch.ps1 -Only mvcmovie
.\experiments\wf4c\convert-corpora.ps1 -Only mvcmovie
```

## 2. Linux で動かす

`run-linux.ps1` は、SQL Server(`w2l-sql`)とアプリをコンテナで動かし、元のアプリの画面と比べます。`-Keep` を付けると、比べた後もアプリのコンテナが残ります。

```powershell
.\experiments\wf4c\run-linux.ps1 -App mvcmovie\MvcMovie -Scenario corpora\regression\mvcmovie.scenario.json -Golden corpora\parity\mvcmovie.golden-webforms.json -SqlServer -Keep
```

- 記録では、12 画面のうち 11 が元のアプリと同じでした(2026-10-22)。
- 残ったコンテナで http://localhost:5095/Movies を開けます。

## 3. 片付け

```powershell
docker rm -f w2l-mvcmovie w2l-sql
```
