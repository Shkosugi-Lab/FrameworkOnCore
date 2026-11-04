# mojo(mojoPortal 3.1.6)

CMS です(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [i7MEDIA/mojoportal](https://github.com/i7MEDIA/mojoportal) タグ `v3.1.6` |
| フォルダー | `corpora\work\mojoportal-3.1.6` |
| ライセンス | EPL-1.0 |
| 追加で要るもの | Build Tools、SQL Server Express。ディスク約 5 GB |

## 1. GitHub から取得して変換する

元のアプリをビルドしてから変換します。

```powershell
cd corpora\work
curl.exe -L -o mojo.zip https://github.com/i7MEDIA/mojoportal/archive/refs/tags/v3.1.6.zip
tar -xf mojo.zip          # mojoportal-3.1.6 ができる
cd ..\..
Start-Service 'MSSQL$SQLEXPRESS'                              # 管理者の PowerShell で
.\experiments\wf4c\convert-corpora.ps1 -Only mojo
```

## 2. Linux で動かす

サイトと SQL Server をコンテナで動かします。DB `mojo_linux` は空の状態から作り直され、最初に開くセットアップの画面がテーブルを作ります。

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site mojo\site -Dll mojoPortal.Web -Database mojo_linux -Paths '/Setup/Default.aspx', '/' -CaseInsensitive -Keep
```

- 確認: 2 つのページが 200 になり、トップのタイトルが `Home - mojoPortal` です(記録 2026-10-11)。
- そのまま http://localhost:5098/ を開けます。

## 3. 片付け

```powershell
docker rm -f w2l-site-mojo w2l-sql
```
