# contoso(Contoso University)

学生・講座を扱う EF6 のチュートリアルです(ASP.NET MVC 5、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [tigerbluejay/ContosoUniversity](https://github.com/tigerbluejay/ContosoUniversity) コミット `2ee1ad2a` の `ContosoUniversitySolution` |
| フォルダー | `corpora\work\ContosoUniversity` |
| ライセンス | 表示なし(手元で試すだけにし、配り直さないでください) |
| 追加で要るもの | なし(SQL Server はコンテナで動かします) |

## 1. GitHub から取得して変換する

```powershell
cd corpora\work
git clone --filter=blob:none --no-checkout --sparse https://github.com/tigerbluejay/ContosoUniversity.git ContosoUniversity-repo
git -C ContosoUniversity-repo sparse-checkout set ContosoUniversitySolution
git -C ContosoUniversity-repo checkout 2ee1ad2aa66fbef012fe280e4552176489dcf0f3
Move-Item ContosoUniversity-repo\ContosoUniversitySolution ContosoUniversity
Remove-Item ContosoUniversity-repo -Recurse -Force
cd ..\..
.\experiments\wf4c\convert-corpora.ps1 -Only contoso
```

## 2. Linux で動かす

`run-linux.ps1` は、SQL Server(`w2l-sql`)とアプリをコンテナで動かし、元のアプリの画面と比べます。`-Keep` を付けると、比べた後もアプリのコンテナが残ります。

```powershell
.\experiments\wf4c\run-linux.ps1 -App contoso\ContosoUniversity -Scenario corpora\regression\contoso.scenario.json -Golden corpora\parity\contoso.golden-webforms.json -SqlServer -Keep
```

- 記録では、12 画面のうち 9 が元のアプリと同じでした(2026-10-22)。
- 残ったコンテナで http://localhost:5095/Student を開けます。

## 3. 片付け

```powershell
docker rm -f w2l-contosouniversity w2l-sql
```
